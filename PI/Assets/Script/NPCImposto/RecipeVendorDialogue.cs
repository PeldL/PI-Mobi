using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RecipeVendorDialogue : MonoBehaviour
{
    [Header("Diálogos")]
    [TextArea] public string[] dialogoInicial;
    [TextArea] public string[] dialogoSemMoedas;
    [TextArea] public string[] dialogoCompraSucesso;
    [TextArea] public string[] dialogoJaTemReceita;

    [Header("UI")]
    public GameObject painelDialogo;
    public TextMeshProUGUI textoDialogo;
    public TextMeshProUGUI nomeNpc;
    public Image imagemNpc;
    public Sprite spriteNpc;
    public GameObject botaoE;
    public Vector3 offsetUI = new Vector3(0, 1.5f, 0);

    [Header("Animação da Boca")]
    public Sprite[] spritesBoca;           // Array com sprites da boca (0=fechada, 1=aberta, etc)
    public float velocidadeAnimacao = 0.1f; // Velocidade entre frames
    public Image imagemBoca;               // Imagem separada para a boca (OPCIONAL)

    [Header("Receitas à Venda")]
    public CraftingRecipe[] receitasParaVender;

    [Header("Configuração")]
    [Range(0.01f, 0.5f)] public float velocidadeLetra = 0.05f;
    public string nomeDoNpc = "Vendedor";

    [Header("Som por Letra")]
    public AudioClip somLetra;
    public AudioSource audioSourceDialogo;
    [Range(0, 1)] public float volumeSomLetra = 0.3f;

    [Header("Animação")]
    public Animator vendorAnimator;
    public string animationTrigger = "VendorGreet";

    [Header("UI de Vendas")]
    public GameObject vendorPanel;
    public Transform recipesVendorContainer;
    public GameObject vendorRecipeSlotPrefab;

    private bool jogadorPerto = false;
    private bool emDialogo = false;
    private bool digitando = false;
    private int linhaAtual = 0;
    private string[] dialogoAtual;
    private int frameAtual = 0;
    private Coroutine corrotinaAnimacao;

    void Start()
    {
        GameDebugger.LogVendor("Vendor inicializado", this);

        if (painelDialogo != null) painelDialogo.SetActive(false);
        if (vendorPanel != null)
        {
            vendorPanel.SetActive(false);

            Canvas canvas = vendorPanel.GetComponent<Canvas>();
            if (canvas == null)
            {
                canvas = vendorPanel.AddComponent<Canvas>();
                canvas.overrideSorting = true;
                canvas.sortingOrder = 100;

                if (vendorPanel.GetComponent<GraphicRaycaster>() == null)
                    vendorPanel.AddComponent<GraphicRaycaster>();
            }
        }
        if (nomeNpc != null) nomeNpc.text = nomeDoNpc;
        if (botaoE != null) botaoE.SetActive(false);

        // Configurar imagem do NPC
        if (imagemNpc != null && spriteNpc != null)
            imagemNpc.sprite = spriteNpc;
    }

    void Update()
    {
        if (botaoE != null && botaoE.activeSelf)
            botaoE.transform.position = transform.position + offsetUI;

        if (vendorPanel != null && vendorPanel.activeInHierarchy)
            return;

        if (jogadorPerto && Input.GetKeyDown(KeyCode.E))
        {
            if (!emDialogo)
                IniciarDialogo();
            else if (!digitando)
                ProximaLinha();
        }
    }

    void IniciarDialogo()
    {
        GameDebugger.LogVendor("Diálogo iniciado com jogador", this);

        dialogoAtual = dialogoInicial;
        linhaAtual = 0;
        emDialogo = true;

        if (painelDialogo != null) painelDialogo.SetActive(true);
        if (botaoE != null) botaoE.SetActive(false);

        var player = FindFirstObjectByType<CharacterController2D>();
        if (player != null) player.SetSpeed(0f);

        StartCoroutine(DigitarTexto());
    }

    void ProximaLinha()
    {
        linhaAtual++;
        if (linhaAtual < dialogoAtual.Length)
        {
            StartCoroutine(DigitarTexto());
        }
        else
        {
            FinalizarDialogo();
            AbrirLoja();
        }
    }

    IEnumerator DigitarTexto()
    {
        digitando = true;

        // Inicia animação da boca apenas se tiver sprites configurados
        if (spritesBoca != null && spritesBoca.Length > 1 && imagemNpc != null)
        {
            if (corrotinaAnimacao != null) StopCoroutine(corrotinaAnimacao);
            corrotinaAnimacao = StartCoroutine(AnimacaoBoca());
        }

        if (textoDialogo != null)
        {
            textoDialogo.text = "";

            foreach (char letra in dialogoAtual[linhaAtual])
            {
                textoDialogo.text += letra;

                if (somLetra != null && audioSourceDialogo != null && !char.IsWhiteSpace(letra))
                {
                    audioSourceDialogo.PlayOneShot(somLetra, volumeSomLetra);
                }

                yield return new WaitForSecondsRealtime(velocidadeLetra);
            }
        }

        digitando = false;

        // Para animação da boca quando termina de digitar
        if (corrotinaAnimacao != null)
        {
            StopCoroutine(corrotinaAnimacao);
            corrotinaAnimacao = null;
        }

        // Volta para sprite normal do NPC
        if (spriteNpc != null && imagemNpc != null)
            imagemNpc.sprite = spriteNpc;
    }

    IEnumerator AnimacaoBoca()
    {
        while (true)
        {
            // Alterna entre os frames do array de boca
            if (spritesBoca.Length > 0)
            {
                frameAtual = (frameAtual + 1) % spritesBoca.Length;

                // SE tiver imagem separada para boca, use ela
                if (imagemBoca != null)
                {
                    imagemBoca.sprite = spritesBoca[frameAtual];
                }
                // SENÃO, substitui a imagem principal (apenas se for sprite de boca)
                else if (imagemNpc != null)
                {
                    // Cria um sprite combinado (NPC + boca animada)
                    // Para simplificar, vamos usar apenas o sprite da boca como temporário
                    // Em um sistema mais avançado, você criaria sprites combinados
                    imagemNpc.sprite = spritesBoca[frameAtual];
                }
            }
            yield return new WaitForSecondsRealtime(velocidadeAnimacao);
        }
    }

    void FinalizarDialogo()
    {
        // Para animação da boca
        if (corrotinaAnimacao != null)
        {
            StopCoroutine(corrotinaAnimacao);
            corrotinaAnimacao = null;
        }

        // Garante que volta para o sprite normal do NPC
        if (spriteNpc != null && imagemNpc != null)
            imagemNpc.sprite = spriteNpc;

        if (painelDialogo != null) painelDialogo.SetActive(false);
        emDialogo = false;
        digitando = false;
        linhaAtual = 0;

        var player = FindFirstObjectByType<CharacterController2D>();
        if (player != null) player.SetSpeed(5f);

        if (botaoE != null && jogadorPerto) botaoE.SetActive(true);
    }

    void AbrirLoja()
    {
        GameDebugger.LogVendor("Abrindo loja de receitas", this);

        if (vendorPanel == null)
        {
            GameDebugger.LogVendor("❌ VendorPanel não atribuído!", this);
            return;
        }

        vendorPanel.SetActive(true);

        foreach (Transform child in vendorPanel.transform)
        {
            child.gameObject.SetActive(true);
        }

        AtualizarUIReceitas();
        Canvas.ForceUpdateCanvases();
        vendorPanel.transform.SetAsLastSibling();

        Time.timeScale = 0f;
        GameDebugger.LogVendor("=== LOJA ABERTA ===", this);
    }

    public void FecharLoja()
    {
        if (vendorPanel != null)
        {
            vendorPanel.SetActive(false);
            Time.timeScale = 1f;
            GameDebugger.LogVendor("Loja fechada", this);
        }
    }

    void AtualizarUIReceitas()
    {
        GameDebugger.LogVendor("Atualizando UI de receitas", this);

        if (vendorRecipeSlotPrefab == null)
        {
            GameDebugger.LogVendor("❌ VendorRecipeSlotPrefab não atribuído!", this);
            return;
        }

        if (recipesVendorContainer == null)
        {
            GameDebugger.LogVendor("❌ RecipesVendorContainer não atribuído!", this);
            return;
        }

        GameDebugger.LogVendor($"Quantidade de receitas: {receitasParaVender?.Length ?? 0}", this);

        if (receitasParaVender == null || receitasParaVender.Length == 0)
        {
            GameDebugger.LogVendor("⚠ Nenhuma receita configurada no NPC!", this);
            return;
        }

        foreach (Transform child in recipesVendorContainer)
        {
            Destroy(child.gameObject);
        }

        int slotsCriados = 0;
        foreach (CraftingRecipe recipe in receitasParaVender)
        {
            if (recipe == null) continue;

            GameDebugger.LogRecipe($"Criando slot para: {recipe.recipeName}", recipe);

            GameObject recipeSlot = Instantiate(vendorRecipeSlotPrefab, recipesVendorContainer);
            recipeSlot.SetActive(true);
            SetActiveRecursive(recipeSlot, true);

            VendorRecipeSlotUI slotUI = recipeSlot.GetComponent<VendorRecipeSlotUI>();
            if (slotUI != null)
            {
                slotUI.Initialize(recipe, this);
                slotsCriados++;
            }
        }

        GameDebugger.LogVendor($"Total de slots criados: {slotsCriados}", this);
    }

    void SetActiveRecursive(GameObject obj, bool state)
    {
        obj.SetActive(state);
        foreach (Transform child in obj.transform)
        {
            SetActiveRecursive(child.gameObject, state);
        }
    }

    public void ComprarReceita(CraftingRecipe recipe)
    {
        if (recipe == null)
        {
            GameDebugger.LogRecipe("❌ Tentativa de comprar receita NULL!");
            return;
        }

        GameDebugger.LogRecipe($"💰 Tentando comprar: {recipe.recipeName}", recipe);
        GameDebugger.LogPlayerPrefs($"Recipe_{recipe.recipeName}_Unlocked");

        // ✅ VERIFICAÇÃO ATUALIZADA - Sempre carregar estado atual
        recipe.LoadUnlockedState();

        if (recipe.isUnlocked)
        {
            GameDebugger.LogRecipe("⚠ Receita já comprada anteriormente!", recipe);
            MostrarDialogo(dialogoJaTemReceita);
            return;
        }

        if (GameData.Instance.coins >= recipe.unlockCost)
        {
            if (GameData.Instance.SpendCoins(recipe.unlockCost))
            {
                // ✅ CORREÇÃO CRÍTICA: Usar UnlockAndSave() em vez de apenas isUnlocked = true
                recipe.UnlockAndSave();

                GameDebugger.LogRecipe($"✅ COMPRADA COM SUCESSO: {recipe.recipeName}", recipe);
                GameDebugger.LogPlayerPrefs($"Recipe_{recipe.recipeName}_Unlocked");

                // ✅ ATUALIZAR SISTEMA DE CRAFTING
                if (CraftingSystem.Instance != null)
                {
                    CraftingSystem.Instance.UnlockRecipe(recipe);
                }

                // ✅ ATUALIZAR UI
                AtualizarUIReceitas();

                MostrarDialogo(dialogoCompraSucesso);

                Debug.Log($"✅ Receita comprada: {recipe.recipeName}");
            }
            else
            {
                GameDebugger.LogSystem("❌ Erro inesperado ao gastar moedas!");
                MostrarDialogo(dialogoSemMoedas);
            }
        }
        else
        {
            GameDebugger.LogSystem($"❌ Moedas insuficientes! Precisa: {recipe.unlockCost}, Tem: {GameData.Instance.coins}");
            MostrarDialogo(dialogoSemMoedas);
        }
    }

    void MostrarDialogo(string[] dialogo)
    {
        if (vendorPanel != null)
            vendorPanel.SetActive(false);

        dialogoAtual = dialogo;
        linhaAtual = 0;
        emDialogo = true;

        if (painelDialogo != null) painelDialogo.SetActive(true);
        StartCoroutine(DigitarTexto());
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            jogadorPerto = true;
            if (!emDialogo && botaoE != null)
                botaoE.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            jogadorPerto = false;
            if (botaoE != null)
                botaoE.SetActive(false);
            if (emDialogo)
                FinalizarDialogo();
        }
    }
}