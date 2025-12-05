using UnityEngine;

public class PlayerMachado : MonoBehaviour
{
    public Animator animator;
    private bool machadoDesbloqueado = false;
    private CharacterController2D playerController;

    // Área de detecção do machado - SÓ PARA BAIXO
    public float alcanceMachado = 1.5f;
    public int danoMachado = 2;
    public LayerMask layerInimigos;

    private const string MACHADO_PREF_KEY = "MachadoDesbloqueado";
    private bool estaAtacando = false;

    void Start()
    {
        playerController = GetComponent<CharacterController2D>();
        machadoDesbloqueado = PlayerPrefs.GetInt(MACHADO_PREF_KEY, 0) == 1;
        Debug.Log($"🔧 PlayerMachado Iniciado - Machado Desbloqueado: {machadoDesbloqueado}");
    }

    void Update()
    {
        // Tecla 9 para desbloquear machado (teste)
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            DesbloquearMachado();
            Debug.Log("🪓 Machado desbloqueado via tecla 9!");
        }

        if (machadoDesbloqueado && Input.GetMouseButtonDown(0) && !estaAtacando) // botão esquerdo
        {
            Debug.Log("🖱️ Botão do mouse pressionado - Iniciando ataque com machado PARA BAIXO");
            UsarMachado();
        }
        else if (Input.GetMouseButtonDown(0) && !machadoDesbloqueado)
        {
            Debug.Log("❌ Machado não desbloqueado ainda! Pressione 9 para desbloquear");
        }
    }

    public void DesbloquearMachado()
    {
        if (!machadoDesbloqueado)
        {
            machadoDesbloqueado = true;
            PlayerPrefs.SetInt(MACHADO_PREF_KEY, 1);
            PlayerPrefs.Save();
            Debug.Log("🪓 Machado desbloqueado!");
        }
    }

    // MÉTODO PARA BOTÃO MOBILE (OnClick)
    public void UsarMachadoMobile()
    {
        if (machadoDesbloqueado && !estaAtacando)
        {
            Debug.Log("📱 Machado usado via botão mobile!");
            UsarMachado();
        }
        else if (!machadoDesbloqueado)
        {
            Debug.Log("❌ Machado não desbloqueado!");
        }
    }

    private void UsarMachado()
    {
        if (animator != null)
        {
            estaAtacando = true;

            Debug.Log("🎬 Iniciando animação do machado EXCLUSIVA PARA BAIXO...");

            // PARA tudo e força direção para baixo
            playerController.SetSpeed(0f);

            // Reseta todas as direções e força para baixo
            animator.SetFloat("Horizontal", 0f);
            animator.SetFloat("Vertical", -1f);
            animator.SetFloat("lastHorizontal", 0f);
            animator.SetFloat("lastVertical", -1f);

            // Usa um trigger específico só para o machado baixo
            animator.SetTrigger("MachadoBaixo");

            Debug.Log("🚶 Movimento parado durante ataque - DIREÇÃO TRAVADA EM BAIXO");

            // Detecta inimigos APENAS EMBAIXO
            Invoke(nameof(DetectarInimigosAbaixo), 0.3f); // Delay para sincronizar com animação

            // Tempo fixo para o ataque
            Invoke(nameof(FinalizarAtaque), 0.6f);
        }
        else
        {
            Debug.LogError("❌ Animator não encontrado!");
        }
    }

    private void DetectarInimigosAbaixo()
    {
        Debug.Log("🔍 Detectando inimigos APENAS EMBAIXO do jogador...");

        // Posição exata abaixo do jogador
        Vector2 posicaoAtaque = (Vector2)transform.position + Vector2.down * alcanceMachado * 0.8f;

        Debug.Log($"🎯 Área de ataque: {posicaoAtaque}");

        // Detecta inimigos em uma área CIRCULAR apenas abaixo
        Collider2D[] inimigosAtingidos = Physics2D.OverlapCircleAll(posicaoAtaque, alcanceMachado * 0.3f, layerInimigos);

        Debug.Log($"👥 Inimigos na área de baixo: {inimigosAtingidos.Length}");

        int inimigosAcertados = 0;
        foreach (Collider2D inimigo in inimigosAtingidos)
        {
            if (inimigo.CompareTag("Inimigo"))
            {
                InimigoAvançado scriptInimigo = inimigo.GetComponent<InimigoAvançado>();
                if (scriptInimigo != null)
                {
                    Debug.Log($"💥 Machado acertou {inimigo.name} ABAIXO!");
                    scriptInimigo.TomarDano(danoMachado, true);
                    inimigosAcertados++;
                }
            }
        }

        Debug.Log($"🎯 Total de inimigos acertados EMBAIXO: {inimigosAcertados}");

        // Debug visual
        Debug.DrawLine(transform.position, posicaoAtaque, Color.red, 1f);
    }

    private void FinalizarAtaque()
    {
        estaAtacando = false;
        playerController.SetSpeed(5f);
        Debug.Log("🚶 Movimento retomado - Ataque finalizado");
    }

    // Gizmos para visualizar a área de ataque APENAS EMBAIXO
    private void OnDrawGizmosSelected()
    {
        Vector2 posicaoAtaque = (Vector2)transform.position + Vector2.down * alcanceMachado * 0.8f;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(posicaoAtaque, alcanceMachado * 0.3f);

        // Linha mostrando a direção do ataque
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, posicaoAtaque);
    }

    void OnDrawGizmos()
    {
        // Gizmo sempre visível (mais transparente)
        Vector2 posicaoAtaque = (Vector2)transform.position + Vector2.down * alcanceMachado * 0.8f;

        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Gizmos.DrawWireSphere(posicaoAtaque, alcanceMachado * 0.3f);
    }
}