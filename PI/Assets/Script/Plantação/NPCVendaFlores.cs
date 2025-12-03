using UnityEngine;
using TMPro;
using System.Collections;

public class NPCVendaFlores : MonoBehaviour
{
    [Header("Configurações do NPC")]
    [SerializeField] private float interacaoDistancia = 3f;
    [SerializeField] private KeyCode teclaInteracao = KeyCode.E;
    [SerializeField] private GameObject painelVenda;
    [SerializeField] private Transform jogador;

    [Header("UI Elements - TextMeshPro")]
    [SerializeField] private TextMeshProUGUI textoStatus;
    [SerializeField] private UnityEngine.UI.Button botaoVender;
    [SerializeField] private UnityEngine.UI.Button botaoFechar;
    [SerializeField] private TextMeshProUGUI textoTotalMoedas;
    [SerializeField] private TextMeshProUGUI textoInstrucao;

    [Header("Itens para Venda")]
    [SerializeField] private ItemData[] floresParaVenda;
    [SerializeField] private int precoPorFlor = 3;

    private bool jogadorPerto = false;
    private BotaoVenderItem vendedor;
    private bool jogoPausado = false;
    private CursorLockMode cursorEstadoAnterior;
    private bool cursorVisivelAnterior;

    void Start()
    {
        // Configurar referências
        if (painelVenda != null)
            painelVenda.SetActive(false);

        if (botaoVender != null)
            botaoVender.onClick.AddListener(VenderFlores);

        if (botaoFechar != null)
            botaoFechar.onClick.AddListener(FecharPainelVenda);

        // Configurar texto dos botões se existirem
        TextMeshProUGUI textoBotaoVender = botaoVender.GetComponentInChildren<TextMeshProUGUI>();
        if (textoBotaoVender != null)
        {
            textoBotaoVender.text = "VENDER FLORES";
        }

        TextMeshProUGUI textoBotaoFechar = botaoFechar.GetComponentInChildren<TextMeshProUGUI>();
        if (textoBotaoFechar != null)
        {
            textoBotaoFechar.text = "FECHAR";
        }

        // Adicionar o componente vendedor se não existir
        vendedor = GetComponent<BotaoVenderItem>();
        if (vendedor == null)
            vendedor = gameObject.AddComponent<BotaoVenderItem>();

        // Configurar vendedor com as flores
        if (vendedor != null && floresParaVenda != null)
        {
            vendedor.ConfigurarItensVenda(floresParaVenda, precoPorFlor);
            vendedor.textoStatusTMP = textoStatus;
        }
    }

    void Update()
    {
        VerificarProximidadeDoJogador();

        if (jogadorPerto && Input.GetKeyDown(teclaInteracao))
        {
            if (painelVenda != null && !painelVenda.activeInHierarchy)
            {
                AbrirPainelVenda();
            }
            else
            {
                FecharPainelVenda();
            }
        }

        // Fechar painel com ESC
        if (painelVenda != null && painelVenda.activeInHierarchy && Input.GetKeyDown(KeyCode.Escape))
        {
            FecharPainelVenda();
        }
    }

    void VerificarProximidadeDoJogador()
    {
        if (jogador == null) return;

        float distancia = Vector3.Distance(transform.position, jogador.position);
        bool estavaPerto = jogadorPerto;
        jogadorPerto = distancia <= interacaoDistancia;

        // Atualizar texto de instrução
        if (textoInstrucao != null)
        {
            textoInstrucao.gameObject.SetActive(jogadorPerto);
            if (jogadorPerto)
            {
                textoInstrucao.text = $"Pressione <color=yellow>{teclaInteracao}</color> para falar com o vendedor";
            }
        }

        if (jogadorPerto != estavaPerto)
        {
            OnProximidadeAlterada(jogadorPerto);
        }
    }

    void OnProximidadeAlterada(bool estaPerto)
    {
        Debug.Log(estaPerto ? "Jogador está perto do NPC" : "Jogador saiu do alcance");
    }

    public void AbrirPainelVenda()
    {
        // Salvar estado atual do cursor antes de abrir o painel
        cursorEstadoAnterior = Cursor.lockState;
        cursorVisivelAnterior = Cursor.visible;

        painelVenda.SetActive(true);
        PausarJogo();
        AtualizarUI();
    }

    public void FecharPainelVenda()
    {
        painelVenda.SetActive(false);
        DespausarJogo();
    }

    void PausarJogo()
    {
        Time.timeScale = 0f;
        jogoPausado = true;

        // Liberar cursor para interagir com a UI
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("Jogo pausado - Painel de vendas aberto");
        Debug.Log("Cursor visível: " + Cursor.visible + ", LockState: " + Cursor.lockState);
    }

    void DespausarJogo()
    {
        Time.timeScale = 1f;
        jogoPausado = false;

        // Restaurar estado anterior do cursor
        Cursor.lockState = cursorEstadoAnterior;
        Cursor.visible = cursorVisivelAnterior;

        Debug.Log("Jogo despausado - Painel de vendas fechado");
        Debug.Log("Cursor visível: " + Cursor.visible + ", LockState: " + Cursor.lockState);
    }

    void VenderFlores()
    {
        if (vendedor != null)
        {
            vendedor.VenderTodos();
            StartCoroutine(AtualizarUIAposVenda());
        }
    }

    IEnumerator AtualizarUIAposVenda()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        AtualizarUI();
    }

    void AtualizarUI()
    {
        // Atualizar total de moedas
        if (textoTotalMoedas != null && GameData.Instance != null)
        {
            textoTotalMoedas.text = $"<color=yellow>Moedas:</color> {GameData.Instance.coins}";
        }

        // Atualizar status das flores
        if (textoStatus != null)
        {
            bool temFlores = VerificarSeTemFlores();

            if (!temFlores)
            {
                textoStatus.text = "<color=red>Você não tem flores para vender.</color>";
                textoStatus.alignment = TextAlignmentOptions.Center;
            }
            else
            {
                textoStatus.text = "Clique em <color=green>'VENDER FLORES'</color> para vender todas as suas flores!";
                textoStatus.alignment = TextAlignmentOptions.Center;
            }
        }
    }

    bool VerificarSeTemFlores()
    {
        if (InventorySystem.Instance == null || floresParaVenda == null)
            return false;

        foreach (ItemData flor in floresParaVenda)
        {
            if (flor != null && InventorySystem.Instance.GetItemCount(flor) > 0)
            {
                return true;
            }
        }
        return false;
    }

    // Método público para ser usado no OnClick do botão
    public void BotaoFecharPainel()
    {
        FecharPainelVenda();
    }

    // Visual helper no editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interacaoDistancia);
    }

    // Para interação por trigger
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jogadorPerto = true;
            OnProximidadeAlterada(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jogadorPerto = false;
            FecharPainelVenda();
            OnProximidadeAlterada(false);
        }
    }
}