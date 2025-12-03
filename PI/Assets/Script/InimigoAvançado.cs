using UnityEngine;

public class InimigoAvançado : MonoBehaviour
{
    [Header("Configurações Básicas")]
    public float velocidade = 4f;
    public float distanciaDetecaoPlayer = 4f;
    public float distanciaAtaque = 1.5f;
    public int vidaMaxima = 3;
    public int danoAtaque = 1;

    [Header("Sistema de Patrulha")]
    public bool usarPatrulha = true;
    public float raioPatrulha = 3f;
    public float tempoEntreMovimentos = 2f;
    public float distanciaMinimaDestino = 0.5f;

    [Header("Configurações de Colisão")]
    public LayerMask camadaParedes;
    public float distanciaDetecaoParede = 0.8f;
    public float distanciaEvitacao = 1.2f; // Maior distância para evitar colisões

    [Header("Suavização de Movimento")]
    public float suavizacaoMovimento = 5f;
    public float suavizacaoRotacao = 8f;
    public bool usarInterpolacao = true;

    [Header("Referências")]
    public Transform player;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private int vidaAtual;

    // Estados
    private enum Estado { Patrulhando, Perseguindo, Atacando, Morto }
    private Estado estadoAtual = Estado.Patrulhando;

    // Variáveis de movimento
    private Vector2 pontoInicial;
    private Vector2 destinoPatrulha;
    private float tempoUltimoMovimento;
    private bool playerVisivel = false;

    // Variáveis para suavização
    private Vector2 velocidadeAtual;
    private Vector2 direcaoSuavizada;
    private Vector2 velocidadeDesejada;

    // Variáveis para evitação de colisão
    private bool obstaculoProximo = false;
    private Vector2 direcaoEvitacao;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        if (rb != null)
        {
            rb.gravityScale = 0;
            rb.freezeRotation = true;
            rb.linearDamping = 0;

            if (usarInterpolacao)
            {
                rb.interpolation = RigidbodyInterpolation2D.Interpolate;
            }
        }

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }

        vidaAtual = vidaMaxima;
        pontoInicial = transform.position;
        EscolherNovoDestinoPatrulha();

        velocidadeAtual = Vector2.zero;
        direcaoSuavizada = Vector2.right;
    }

    void Update()
    {
        if (player == null) return;

        VerificarPlayer();
        AtualizarEstado();
        AtualizarAnimacoes();
    }

    void FixedUpdate()
    {
        if (estadoAtual == Estado.Morto) return;

        // 1. Verificar obstáculos primeiro
        VerificarObstaculosProximos();

        // 2. Calcular velocidade desejada baseada no estado
        CalcularVelocidadeDesejada();

        // 3. Aplicar movimento suavizado
        AplicarMovimentoSuavizado();

        // 4. Atualizar rotação suavizada
        AtualizarRotacaoSuavizada();
    }

    void VerificarObstaculosProximos()
    {
        obstaculoProximo = false;
        Vector2 direcaoAtual = direcaoSuavizada;

        // Verificar em múltiplas direções ao redor do inimigo
        float[] angulos = { 0f, 45f, 90f, -45f, -90f, 135f, -135f, 180f };
        float menorDistancia = Mathf.Infinity;
        Vector2 direcaoObstaculo = Vector2.zero;

        foreach (float angulo in angulos)
        {
            Vector2 direcaoTeste = Quaternion.Euler(0, 0, angulo) * direcaoAtual;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direcaoTeste, distanciaEvitacao, camadaParedes);

            if (hit.collider != null && !hit.collider.CompareTag("Player"))
            {
                obstaculoProximo = true;
                if (hit.distance < menorDistancia)
                {
                    menorDistancia = hit.distance;
                    direcaoObstaculo = hit.normal;
                }
            }
        }

        if (obstaculoProximo)
        {
            // Calcular direção de evitação (perpendicular ao obstáculo)
            direcaoEvitacao = CalcularDirecaoEvitacao(direcaoObstaculo);
        }
    }

    Vector2 CalcularDirecaoEvitacao(Vector2 normalObstaculo)
    {
        // Calcular direções perpendiculares à normal do obstáculo
        Vector2 perpendicular1 = new Vector2(-normalObstaculo.y, normalObstaculo.x);
        Vector2 perpendicular2 = new Vector2(normalObstaculo.y, -normalObstaculo.x);

        // Escolher a direção que mais se alinha com a direção atual
        float dot1 = Vector2.Dot(perpendicular1, direcaoSuavizada);
        float dot2 = Vector2.Dot(perpendicular2, direcaoSuavizada);

        return dot1 > dot2 ? perpendicular1 : perpendicular2;
    }

    void CalcularVelocidadeDesejada()
    {
        // Se há obstáculo próximo, priorizar evitação
        if (obstaculoProximo)
        {
            velocidadeDesejada = direcaoEvitacao * velocidade * 0.6f;
            direcaoSuavizada = Vector2.Lerp(direcaoSuavizada, direcaoEvitacao, suavizacaoRotacao * 2f * Time.fixedDeltaTime);
            return;
        }

        switch (estadoAtual)
        {
            case Estado.Patrulhando:
                CalcularVelocidadePatrulha();
                break;
            case Estado.Perseguindo:
                CalcularVelocidadePerseguicao();
                break;
            case Estado.Atacando:
                velocidadeDesejada = Vector2.zero;
                break;
        }
    }

    void CalcularVelocidadePatrulha()
    {
        if (!usarPatrulha)
        {
            velocidadeDesejada = Vector2.zero;
            return;
        }

        float distanciaParaDestino = Vector2.Distance(transform.position, destinoPatrulha);
        bool deveMover = distanciaParaDestino > distanciaMinimaDestino;
        bool tempoMudarDestino = Time.time - tempoUltimoMovimento > tempoEntreMovimentos;

        if (!deveMover || tempoMudarDestino)
        {
            EscolherNovoDestinoPatrulha();
            tempoUltimoMovimento = Time.time;
        }

        if (deveMover)
        {
            Vector2 direcao = (destinoPatrulha - (Vector2)transform.position).normalized;

            if (!HaParedeNoCaminho(direcao))
            {
                velocidadeDesejada = direcao * velocidade * 0.7f;
                direcaoSuavizada = Vector2.Lerp(direcaoSuavizada, direcao, suavizacaoRotacao * Time.fixedDeltaTime);
            }
            else
            {
                EscolherNovoDestinoPatrulha();
                velocidadeDesejada = Vector2.zero;
            }
        }
        else
        {
            velocidadeDesejada = Vector2.zero;
        }
    }

    void CalcularVelocidadePerseguicao()
    {
        if (player == null)
        {
            velocidadeDesejada = Vector2.zero;
            return;
        }

        Vector2 direcaoPlayer = (player.position - transform.position).normalized;

        if (!HaParedeNoCaminho(direcaoPlayer))
        {
            velocidadeDesejada = direcaoPlayer * velocidade;
            direcaoSuavizada = Vector2.Lerp(direcaoSuavizada, direcaoPlayer, suavizacaoRotacao * Time.fixedDeltaTime);
        }
        else
        {
            Vector2 direcaoAlternativa = CalcularDirecaoAlternativa(direcaoPlayer);
            velocidadeDesejada = direcaoAlternativa * velocidade * 0.8f;
            direcaoSuavizada = Vector2.Lerp(direcaoSuavizada, direcaoAlternativa, suavizacaoRotacao * Time.fixedDeltaTime);
        }
    }

    void AplicarMovimentoSuavizado()
    {
        if (rb == null) return;

        Vector2 novaVelocidade = Vector2.SmoothDamp(
            rb.linearVelocity,
            velocidadeDesejada,
            ref velocidadeAtual,
            suavizacaoMovimento * Time.fixedDeltaTime,
            Mathf.Infinity,
            Time.fixedDeltaTime
        );

        rb.linearVelocity = novaVelocidade;
    }

    void AtualizarRotacaoSuavizada()
    {
        if (spriteRenderer == null) return;

        if (direcaoSuavizada.magnitude > 0.1f)
        {
            bool deveVirar = direcaoSuavizada.x < 0;
            spriteRenderer.flipX = deveVirar;
        }
    }

    void VerificarPlayer()
    {
        if (player == null) return;

        float distancia = Vector2.Distance(transform.position, player.position);
        bool playerDentroDaVisao = distancia <= distanciaDetecaoPlayer;

        if (playerDentroDaVisao)
        {
            Vector2 direcaoPlayer = (player.position - transform.position).normalized;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direcaoPlayer, distancia, camadaParedes);
            playerVisivel = hit.collider == null || hit.collider.CompareTag("Player");
        }
        else
        {
            playerVisivel = false;
        }

        Debug.DrawRay(transform.position, (player.position - transform.position).normalized * distancia,
                     playerVisivel ? Color.green : Color.gray);
    }

    void AtualizarEstado()
    {
        if (estadoAtual == Estado.Morto) return;

        float distanciaPlayer = Vector2.Distance(transform.position, player.position);

        if (playerVisivel && distanciaPlayer <= distanciaAtaque)
        {
            estadoAtual = Estado.Atacando;
        }
        else if (playerVisivel && distanciaPlayer <= distanciaDetecaoPlayer)
        {
            estadoAtual = Estado.Perseguindo;
        }
        else
        {
            estadoAtual = Estado.Patrulhando;
        }
    }

    bool HaParedeNoCaminho(Vector2 direcao)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direcao, distanciaDetecaoParede, camadaParedes);
        return hit.collider != null && !hit.collider.CompareTag("Player");
    }

    Vector2 CalcularDirecaoAlternativa(Vector2 direcaoOriginal)
    {
        Vector2[] direcoesTeste = {
            new Vector2(direcaoOriginal.y, -direcaoOriginal.x).normalized,
            new Vector2(-direcaoOriginal.y, direcaoOriginal.x).normalized,
            new Vector2(1, 0),
            new Vector2(-1, 0),
            new Vector2(0, 1),
            new Vector2(0, -1)
        };

        foreach (Vector2 dir in direcoesTeste)
        {
            if (!HaParedeNoCaminho(dir))
            {
                return dir;
            }
        }

        return -direcaoOriginal;
    }

    void EscolherNovoDestinoPatrulha()
    {
        int tentativas = 0;
        Vector2 novoDestino;
        bool destinoValido = false;

        do
        {
            Vector2 direcaoAleatoria = Random.insideUnitCircle.normalized;
            float distanciaAleatoria = Random.Range(1f, raioPatrulha);
            novoDestino = pontoInicial + direcaoAleatoria * distanciaAleatoria;

            tentativas++;
            Vector2 direcaoParaDestino = (novoDestino - (Vector2)transform.position).normalized;
            destinoValido = !HaParedeNoCaminho(direcaoParaDestino);
        }
        while (tentativas < 5 && !destinoValido);

        destinoPatrulha = novoDestino;
    }

    void AtualizarAnimacoes()
    {
        if (animator == null) return;

        bool estaAndando = rb.linearVelocity.magnitude > 0.1f;
        bool estaAtacando = estadoAtual == Estado.Atacando;

        animator.SetBool("Andando", estaAndando);
        animator.SetBool("Atacando", estaAtacando);

        float velocidadeSuavizada = rb.linearVelocity.magnitude / velocidade;
        animator.SetFloat("Velocidade", velocidadeSuavizada);

        if (direcaoSuavizada.magnitude > 0.1f)
        {
            animator.SetFloat("Horizontal", direcaoSuavizada.x);
            animator.SetFloat("Vertical", direcaoSuavizada.y);
        }
    }

    public void TomarDano(int dano, bool danoMachado = false)
    {
        if (estadoAtual == Estado.Morto) return;

        vidaAtual -= dano;

        if (spriteRenderer != null)
        {
            StartCoroutine(FlashDano());
        }

        if (danoMachado && rb != null && player != null)
        {
            Vector2 direcaoKnockback = (transform.position - player.position).normalized;
            rb.AddForce(direcaoKnockback * 5f, ForceMode2D.Impulse);
        }

        if (vidaAtual <= 0)
        {
            Morrer();
        }
    }

    private System.Collections.IEnumerator FlashDano()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.white;
    }

    void Morrer()
    {
        estadoAtual = Estado.Morto;

        if (animator != null)
        {
            animator.SetTrigger("Morrer");
        }

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
        }

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        Destroy(gameObject, 1f);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && estadoAtual == Estado.Atacando)
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(danoAtaque);
            }
        }
    }

    // Reação imediata a colisões
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & camadaParedes) != 0)
        {
            // Empurrar para longe da colisão
            Vector2 direcaoFuga = (transform.position - collision.transform.position).normalized;
            rb.AddForce(direcaoFuga * 3f, ForceMode2D.Impulse);

            // Escolher nova direção imediatamente
            EscolherNovoDestinoPatrulha();
        }
    }

    void OnDrawGizmosSelected()
    {
        // Área de detecção do player
        Gizmos.color = playerVisivel ? Color.red : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distanciaDetecaoPlayer);

        // Área de ataque
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanciaAtaque);

        // Área de patrulha
        if (usarPatrulha)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(pontoInicial, raioPatrulha);

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(destinoPatrulha, 0.2f);
            Gizmos.DrawLine(transform.position, destinoPatrulha);
        }

        // Direção suavizada
        if (Application.isPlaying)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, direcaoSuavizada * 1.5f);

            Gizmos.color = Color.magenta;
            Gizmos.DrawRay(transform.position, velocidadeDesejada.normalized * 1f);
        }

        // Área de evitação de obstáculos
        Gizmos.color = obstaculoProximo ? Color.red : Color.white;
        Gizmos.DrawWireSphere(transform.position, distanciaEvitacao);

        // Raios de detecção de obstáculos
        if (Application.isPlaying)
        {
            Gizmos.color = Color.yellow;
            float[] angulos = { 0f, 45f, 90f, -45f, -90f, 135f, -135f, 180f };
            foreach (float angulo in angulos)
            {
                Vector2 direcaoTeste = Quaternion.Euler(0, 0, angulo) * direcaoSuavizada;
                Gizmos.DrawRay(transform.position, direcaoTeste * distanciaEvitacao);
            }
        }
    }
}