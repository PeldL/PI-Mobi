using UnityEngine;

public class FolhaVoando : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    public float velocidade = 2f;
    public float amplitudeOscilacao = 1f;
    public float frequenciaOscilacao = 1f;

    [Header("Rotação")]
    public float velocidadeRotacao = 30f;
    public float variacaoRotacao = 10f;

    [Header("Trajetória Diagonal")]
    [Range(0f, 1f)]
    public float alturaInicial = 0.8f;
    [Range(0f, 1f)]
    public float alturaFinal = 0.2f;
    public bool direitaParaEsquerda = false;

    [Header("Gizmos")]
    public bool mostrarGizmos = true;
    public Color corTrajetoria = Color.green;
    public Color corDirecaoAtual = Color.red;
    public Color corLimitesTela = Color.yellow;
    public Color corTrajetoriaPrevista = new Color(1f, 0.5f, 0f, 0.7f);

    private Vector3 direcaoAtual;
    private float tempo;
    private float rotacaoAtual;
    private float offsetRotacao;
    private Vector3[] trajetoriaPassada = new Vector3[100];
    private int indiceTrajetoria;
    private Vector3 pontoInicial;
    private Vector3 pontoFinal;

    void Start()
    {
        ConfigurarTrajetoria();
        PosicionarNoInicio();

        offsetRotacao = Random.Range(0f, 360f);
        rotacaoAtual = Random.Range(-velocidadeRotacao, velocidadeRotacao);

        // Inicializa array de trajetória
        for (int i = 0; i < trajetoriaPassada.Length; i++)
        {
            trajetoriaPassada[i] = transform.position;
        }
    }

    void ConfigurarTrajetoria()
    {
        if (Camera.main != null)
        {
            // Define pontos inicial e final baseado na direção escolhida
            if (direitaParaEsquerda)
            {
                // Direita -> Esquerda (descendo)
                pontoInicial = Camera.main.ViewportToWorldPoint(new Vector3(1.1f, alturaInicial, 10f));
                pontoFinal = Camera.main.ViewportToWorldPoint(new Vector3(-0.1f, alturaFinal, 10f));
            }
            else
            {
                // Esquerda -> Direita (descendo) - PADRÃO
                pontoInicial = Camera.main.ViewportToWorldPoint(new Vector3(-0.1f, alturaInicial, 10f));
                pontoFinal = Camera.main.ViewportToWorldPoint(new Vector3(1.1f, alturaFinal, 10f));
            }

            // Calcula direção inicial
            direcaoAtual = (pontoFinal - pontoInicial).normalized;
        }
    }

    void PosicionarNoInicio()
    {
        transform.position = pontoInicial;
    }

    void Update()
    {
        tempo += Time.deltaTime;

        // Armazena posição atual na trajetória
        trajetoriaPassada[indiceTrajetoria] = transform.position;
        indiceTrajetoria = (indiceTrajetoria + 1) % trajetoriaPassada.Length;

        // Movimento principal na direção da trajetória
        Vector3 movimento = direcaoAtual * velocidade * Time.deltaTime;

        // Oscilação para simular o balanço da folha (perpendicular à direção principal)
        Vector3 direcaoPerpendicular = new Vector3(-direcaoAtual.y, direcaoAtual.x, 0);
        float oscilacao = Mathf.Sin(tempo * frequenciaOscilacao + offsetRotacao) * amplitudeOscilacao;
        movimento += direcaoPerpendicular * oscilacao * Time.deltaTime;

        // Aplica o movimento
        transform.position += movimento;

        // Rotação oscilante
        float rotacaoOscilante = Mathf.Sin(tempo * frequenciaOscilacao * 0.5f + offsetRotacao) * variacaoRotacao;
        rotacaoAtual += (rotacaoOscilante + Random.Range(-2f, 2f)) * Time.deltaTime;

        // Aplica rotação suave
        transform.Rotate(0, 0, rotacaoAtual * Time.deltaTime);

        // Verifica se chegou ao final da trajetória
        VerificarFimTrajetoria();
    }

    void VerificarFimTrajetoria()
    {
        Vector3 viewportPos = Camera.main.WorldToViewportPoint(transform.position);

        // Verifica se saiu da área visível (com margem)
        bool saiuDaTela = viewportPos.x > 1.2f || viewportPos.x < -0.2f ||
                          viewportPos.y > 1.2f || viewportPos.y < -0.2f;

        // Verifica se passou do ponto final (considerando a direção)
        bool passouDoPontoFinal = false;
        if (direitaParaEsquerda)
        {
            passouDoPontoFinal = viewportPos.x < -0.1f;
        }
        else
        {
            passouDoPontoFinal = viewportPos.x > 1.1f;
        }

        if (saiuDaTela || passouDoPontoFinal)
        {
            ReiniciarTrajetoria();
        }
    }

    void ReiniciarTrajetoria()
    {
        // Recupera configuração inicial (pode ter mudado no Inspector)
        ConfigurarTrajetoria();

        // Reposiciona no início
        transform.position = pontoInicial;

        // Reseta algumas variáveis
        tempo = 0f;
        offsetRotacao = Random.Range(0f, 360f);
        rotacaoAtual = Random.Range(-velocidadeRotacao, velocidadeRotacao);

        // Limpa trajetória
        for (int i = 0; i < trajetoriaPassada.Length; i++)
        {
            trajetoriaPassada[i] = transform.position;
        }
    }

    void OnDrawGizmos()
    {
        if (!mostrarGizmos) return;

        // Atualiza pontos da trajetória se estiver no Editor
        if (!Application.isPlaying)
        {
            ConfigurarTrajetoria();
        }

        // Desenha trajetória passada
        Gizmos.color = corTrajetoria;
        for (int i = 0; i < trajetoriaPassada.Length - 1; i++)
        {
            int currentIndex = (indiceTrajetoria + i) % trajetoriaPassada.Length;
            int nextIndex = (indiceTrajetoria + i + 1) % trajetoriaPassada.Length;

            if (trajetoriaPassada[currentIndex] != Vector3.zero && trajetoriaPassada[nextIndex] != Vector3.zero)
            {
                Gizmos.DrawLine(trajetoriaPassada[currentIndex], trajetoriaPassada[nextIndex]);
            }
        }

        // Desenha direção atual
        Gizmos.color = corDirecaoAtual;
        Vector3 direcaoVisual = transform.position + direcaoAtual * 2f;
        Gizmos.DrawLine(transform.position, direcaoVisual);
        Gizmos.DrawWireSphere(direcaoVisual, 0.1f);

        // Desenha trajetória principal planejada
        Gizmos.color = corTrajetoriaPrevista;
        Gizmos.DrawLine(pontoInicial, pontoFinal);

        // Desenha pontos inicial e final
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(pontoInicial, 0.2f);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(pontoFinal, 0.2f);

        // Desenha limites da tela
        DesenharLimitesTela();
    }

    void DesenharLimitesTela()
    {
        Gizmos.color = corLimitesTela;

        if (Camera.main != null)
        {
            Vector3 bottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(-0.2f, -0.2f, 10f));
            Vector3 topLeft = Camera.main.ViewportToWorldPoint(new Vector3(-0.2f, 1.2f, 10f));
            Vector3 topRight = Camera.main.ViewportToWorldPoint(new Vector3(1.2f, 1.2f, 10f));
            Vector3 bottomRight = Camera.main.ViewportToWorldPoint(new Vector3(1.2f, -0.2f, 10f));

            Gizmos.DrawLine(bottomLeft, topLeft);
            Gizmos.DrawLine(topLeft, topRight);
            Gizmos.DrawLine(topRight, bottomRight);
            Gizmos.DrawLine(bottomRight, bottomLeft);
        }
    }

    // Método público para reiniciar manualmente
    public void ReiniciarFolha()
    {
        ReiniciarTrajetoria();
    }

    // Método para mudar a trajetória em tempo de execução
    public void ConfigurarNovaTrajetoria(float novaAlturaInicial, float novaAlturaFinal, bool novaDirecao)
    {
        alturaInicial = novaAlturaInicial;
        alturaFinal = novaAlturaFinal;
        direitaParaEsquerda = novaDirecao;
        ReiniciarTrajetoria();
    }
}