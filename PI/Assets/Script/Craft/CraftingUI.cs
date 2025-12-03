using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class CraftingUI : MonoBehaviour
{
    public static CraftingUI Instance { get; private set; }

    [Header("Referências UI")]
    public GameObject craftingPanel;
    public Transform recipesContainer;
    public Transform materialsContainer;
    public GameObject recipeSlotPrefab;
    public GameObject materialSlotPrefab;

    [Header("🎯 POSIÇÕES POR GAMEOBJECT")]
    public List<GameObject> materialPositions;
    public List<GameObject> recipePositions;

    [Header("🎯 BOTÕES MANUAIS (NOVO)")]
    public List<Button> botoesManuais;
    public List<CraftingRecipe> receitasParaBotoes;

    [Header("Detalhes da Receita")]
    public TextMeshProUGUI recipeNameText;
    public TextMeshProUGUI recipeDescriptionText;
    public Image recipeIconImage;

    [Header("Botão de Craft")]
    public Button craftButton;
    public TextMeshProUGUI craftButtonText;

    [Header("Configurações")]
    public bool pausarJogo = true;

    private CraftingRecipe selectedRecipe;
    private List<GameObject> currentSlots = new List<GameObject>();
    private List<GameObject> currentMaterialSlots = new List<GameObject>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        InitializeUI();

        if (CraftingSystem.Instance != null)
        {
            CraftingSystem.Instance.OnRecipesUpdated += UpdateUI;
            Debug.Log("✅ CraftingUI conectado ao CraftingSystem");
        }
        else
        {
            Debug.LogError("❌ CraftingSystem não encontrado no Start!");
        }
    }

    void InitializeUI()
    {
        if (craftButton != null)
        {
            craftButton.onClick.AddListener(OnCraftButtonClicked);
        }

        ConfigurarBotoesManuais();
        UpdateUI();
    }

    // ✅✅✅ MÉTODO ATUALIZADO: BOTÕES COM IMAGEM DO ITEM ✅✅✅
    private void ConfigurarBotoesManuais()
    {
        if (botoesManuais == null || receitasParaBotoes == null) return;

        Debug.Log($"🔧 Configurando {botoesManuais.Count} botões manuais");

        for (int i = 0; i < botoesManuais.Count; i++)
        {
            Button botao = botoesManuais[i];
            int index = i;

            if (botao != null && i < receitasParaBotoes.Count && receitasParaBotoes[i] != null)
            {
                // Remove listeners antigos
                botao.onClick.RemoveAllListeners();

                // Adiciona novo listener
                botao.onClick.AddListener(() => {
                    CraftingRecipe receita = receitasParaBotoes[index];
                    if (receita != null)
                    {
                        receita.LoadUnlockedState();

                        if (receita.isUnlocked)
                        {
                            Debug.Log($"🎯 Botão manual clicado: {receita.recipeName}");
                            SelectRecipe(receita);
                            AtualizarCoresBotoes(index);
                        }
                        else
                        {
                            Debug.Log($"❌ Receita não desbloqueada: {receita.recipeName}");
                        }
                    }
                });

                // ✅✅✅ CONFIGURA IMAGEM DO ITEM NO BOTÃO ✅✅✅
                ConfigurarImagemBotao(botao, receitasParaBotoes[i]);

                // Configura estado inicial do botão
                AtualizarEstadoBotao(botao, receitasParaBotoes[i]);

                Debug.Log($"✅ Botão manual {i} configurado: {receitasParaBotoes[i].recipeName}");
            }
        }
    }

    // ✅✅✅ NOVO MÉTODO: CONFIGURAR IMAGEM DO ITEM NO BOTÃO ✅✅✅
    private void ConfigurarImagemBotao(Button botao, CraftingRecipe receita)
    {
        if (botao == null || receita == null || receita.resultItem == null) return;

        // Procura ou cria um Image component no botão
        Image imagemBotao = botao.GetComponent<Image>();
        if (imagemBotao == null)
        {
            imagemBotao = botao.gameObject.AddComponent<Image>();
        }

        // Configura a imagem do item resultante
        imagemBotao.sprite = receita.resultItem.icon;
        imagemBotao.preserveAspect = true; // Mantém proporção

        // Remove o texto se existir
        TextMeshProUGUI texto = botao.GetComponentInChildren<TextMeshProUGUI>();
        if (texto != null)
        {
            texto.gameObject.SetActive(false); // Esconde o texto
        }

        Debug.Log($"🖼️ Imagem configurada no botão: {receita.resultItem.itemName}");
    }

    // ✅✅✅ MÉTODO ATUALIZADO: BOTÕES SÓ MOSTRAM NOME QUANDO BLOQUEADOS ✅✅✅
    private void AtualizarEstadoBotao(Button botao, CraftingRecipe receita)
    {
        if (botao == null || receita == null) return;

        receita.LoadUnlockedState();

        botao.interactable = receita.isUnlocked;

        var cores = botao.colors;
        if (receita.isUnlocked)
        {
            cores.normalColor = Color.white;
            // ✅ IMAGEM VISÍVEL - TEXTO ESCONDIDO
            Image imagem = botao.GetComponent<Image>();
            if (imagem != null) imagem.color = Color.white;

            TextMeshProUGUI texto = botao.GetComponentInChildren<TextMeshProUGUI>();
            if (texto != null) texto.gameObject.SetActive(false);

            Debug.Log($"🟢 Botão {receita.recipeName} ATIVO (imagem visível)");
        }
        else
        {
            cores.normalColor = Color.gray;
            // ✅ IMAGEM ESCONDIDA - TEXTO VISÍVEL
            Image imagem = botao.GetComponent<Image>();
            if (imagem != null) imagem.color = new Color(1, 1, 1, 0.3f); // Transparente

            TextMeshProUGUI texto = botao.GetComponentInChildren<TextMeshProUGUI>();
            if (texto != null)
            {
                texto.gameObject.SetActive(true);
                texto.text = receita.recipeName;
                texto.color = Color.gray;
            }

            Debug.Log($"🔴 Botão {receita.recipeName} INATIVO (só texto)");
        }
        botao.colors = cores;
    }

    // ✅✅✅ MÉTODO ATUALIZADO: CORES COM IMAGEM ✅✅✅
    private void AtualizarCoresBotoes(int indexSelecionado)
    {
        for (int i = 0; i < botoesManuais.Count; i++)
        {
            Button botao = botoesManuais[i];
            if (botao != null && i < receitasParaBotoes.Count && receitasParaBotoes[i] != null)
            {
                var cores = botao.colors;
                Image imagem = botao.GetComponent<Image>();

                if (i == indexSelecionado)
                {
                    // ✅ BOTÃO SELECIONADO - BRANCO BRILLHANTE
                    cores.normalColor = Color.white;
                    if (imagem != null) imagem.color = Color.white;
                }
                else if (receitasParaBotoes[i].isUnlocked)
                {
                    // ✅ BOTÃO DESBLOQUEADO - BRANCO NORMAL
                    cores.normalColor = Color.white;
                    if (imagem != null) imagem.color = new Color(1, 1, 1, 0.8f); // Levemente transparente
                }
                else
                {
                    // ✅ BOTÃO BLOQUEADO - CINZA
                    cores.normalColor = Color.gray;
                    if (imagem != null) imagem.color = new Color(1, 1, 1, 0.3f); // Quase invisível
                }

                botao.colors = cores;
            }
        }
    }

    // 🔥🔥🔥 ABRIR CRAFTING
    public void OpenCrafting()
    {
        if (craftingPanel != null && !craftingPanel.activeSelf)
        {
            craftingPanel.SetActive(true);
            ConfigurarBotoesManuais();
            UpdateUI();

            if (pausarJogo)
            {
                Time.timeScale = 0f;
                Debug.Log("⏸ Crafting aberto - Jogo pausado");
            }

            Debug.Log("🛠 Crafting aberto via OpenCrafting()");
        }
    }

    // 🔥🔥🔥 FECHAR CRAFTING
    public void CloseCrafting()
    {
        if (craftingPanel != null)
        {
            craftingPanel.SetActive(false);

            if (pausarJogo && Time.timeScale == 0f)
            {
                Time.timeScale = 1f;
                Debug.Log("⏩ Crafting fechado - Jogo despausado");
            }

            Debug.Log("🛠 Crafting fechado via CloseCrafting()");
        }
    }

    // 🔥🔥🔥 TOGGLE NORMAL
    public void ToggleCrafting()
    {
        if (craftingPanel != null)
        {
            bool isActive = !craftingPanel.activeSelf;
            craftingPanel.SetActive(isActive);

            if (isActive)
            {
                ConfigurarBotoesManuais();
                UpdateUI();

                if (pausarJogo)
                {
                    Time.timeScale = 0f;
                    Debug.Log("⏸ Crafting aberto - Jogo pausado");
                }
            }
            else
            {
                if (pausarJogo && Time.timeScale == 0f)
                {
                    Time.timeScale = 1f;
                    Debug.Log("⏩ Crafting fechado - Jogo despausado");
                }
            }

            Debug.Log(isActive ? "🛠 Crafting aberto via Toggle" : "🛠 Crafting fechado via Toggle");
        }
    }

    void Update()
    {
        if (craftingPanel != null && craftingPanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseCrafting();
        }
    }

    public void UpdateUI()
    {
        Debug.Log("🔄 UPDATE UI CHAMADO");

        if (CraftingSystem.Instance == null)
        {
            Debug.LogError("❌ CraftingSystem não disponível no UpdateUI!");
            ClearSlots();
            ClearRecipeDetails();
            return;
        }

        ClearSlots();
        ConfigurarBotoesManuais();

        var unlockedRecipes = CraftingSystem.Instance.GetUnlockedRecipes();

        // ✅ CRIA SLOTS NAS POSIÇÕES DEFINIDAS
        for (int i = 0; i < unlockedRecipes.Count; i++)
        {
            CreateRecipeSlot(unlockedRecipes[i], i);
        }

        if (selectedRecipe != null)
        {
            UpdateRecipeDetails(selectedRecipe);
        }
        else
        {
            ClearRecipeDetails();
        }
    }

    void CreateRecipeSlot(CraftingRecipe recipe, int index)
    {
        if (recipeSlotPrefab == null) return;

        Transform parent = recipesContainer;
        Vector3 position = Vector3.zero;

        if (index < recipePositions.Count && recipePositions[index] != null)
        {
            parent = recipePositions[index].transform;
            position = recipePositions[index].transform.position;
            Debug.Log($"📍 Receita {index} na posição: {position}");
        }

        GameObject slot = Instantiate(recipeSlotPrefab, parent);
        slot.transform.position = position;

        RecipeSlotUI slotUI = slot.GetComponent<RecipeSlotUI>();

        if (slotUI != null)
        {
            slotUI.Initialize(recipe, this);
        }

        currentSlots.Add(slot);
    }

    public void SelectRecipe(CraftingRecipe recipe)
    {
        selectedRecipe = recipe;
        UpdateRecipeDetails(recipe);
    }

    public void DeselectCurrentRecipe()
    {
        selectedRecipe = null;
        ClearRecipeDetails();
        Debug.Log("🎯 Receita desselecionada");
    }

    public void UpdateRecipeDetails(CraftingRecipe recipe)
    {
        if (recipe == null) return;

        Debug.Log("🟥🟥🟥 INICIANDO UPDATE RECIPE DETAILS 🟥🟥🟥");

        // ✅✅✅ AGORA SÓ MOSTRA O NOME - SEM DESCRIÇÃO ✅✅✅
        if (recipeNameText != null)
            recipeNameText.text = recipe.recipeName;

        // ❌ REMOVEMOS A DESCRIÇÃO
        if (recipeDescriptionText != null)
            recipeDescriptionText.text = "";

        // ✅ IMAGEM DO ITEM (opcional - se quiser mostrar em algum lugar)
        if (recipeIconImage != null && recipe.resultItem != null)
            recipeIconImage.sprite = recipe.resultItem.icon;

        ClearMaterialSlots();

        if (materialsContainer != null && materialSlotPrefab != null)
        {
            Debug.Log($"🔵 Total Materials: {recipe.materials.Length}");

            for (int i = 0; i < recipe.materials.Length; i++)
            {
                RecipeMaterial material = recipe.materials[i];

                if (material.item != null)
                {
                    Transform parent = materialsContainer;
                    Vector3 position = Vector3.zero;

                    if (i < materialPositions.Count && materialPositions[i] != null)
                    {
                        parent = materialPositions[i].transform;
                        position = materialPositions[i].transform.position;
                        Debug.Log($"📍 Material {i} na posição: {position}");
                    }

                    GameObject materialSlot = Instantiate(materialSlotPrefab, parent);
                    materialSlot.transform.position = position;

                    MaterialSlotUI materialUI = materialSlot.GetComponent<MaterialSlotUI>();

                    if (materialUI != null)
                    {
                        materialUI.Initialize(material, i, recipe.materials.Length);
                    }

                    currentMaterialSlots.Add(materialSlot);
                }
            }

            Debug.Log($"🎯 Child Count Final: {materialsContainer.childCount}");
        }

        UpdateCraftButtonState(recipe);
        Debug.Log("🟥🟥🟥 FIM UPDATE RECIPE DETAILS 🟥🟥🟥");
    }

    // 🔥🔥🔥 CORREÇÃO: MÉTODO DE LIMPEZA CORRETO
    void ClearMaterialSlots()
    {
        if (materialsContainer == null) return;

        Debug.Log($"🟡 Child Count Before Clear: {materialsContainer.childCount}");

        // ✅ LIMPA PELA LISTA DE MATERIAIS
        foreach (GameObject slot in currentMaterialSlots)
        {
            if (slot != null)
            {
                Destroy(slot);
                Debug.Log($"🧹 Destruído material slot: {slot.name}");
            }
        }
        currentMaterialSlots.Clear();

        // ✅ LIMPEZA EXTRA - DESTRÓI QUALQUER OBJETO FILHO RESTANTE
        foreach (Transform child in materialsContainer)
        {
            if (child != null)
            {
                Destroy(child.gameObject);
                Debug.Log($"🧹 Destruído child: {child.name}");
            }
        }

        Debug.Log($"🟢 Child Count After Clear: {materialsContainer.childCount}");
        Debug.Log($"🧹 Limpos {currentMaterialSlots.Count} slots de material da lista");
    }

    void ClearSlots()
    {
        foreach (GameObject slot in currentSlots)
        {
            if (slot != null) Destroy(slot);
        }
        currentSlots.Clear();
    }

    void ClearRecipeDetails()
    {
        ClearMaterialSlots();

        if (recipeNameText != null) recipeNameText.text = "Selecione uma Receita";
        if (recipeDescriptionText != null) recipeDescriptionText.text = "";
        if (recipeIconImage != null) recipeIconImage.sprite = null;

        if (craftButton != null) craftButton.interactable = false;
        if (craftButtonText != null) craftButtonText.text = "Selecione uma Receita";
    }

    void UpdateCraftButtonState(CraftingRecipe recipe)
    {
        if (craftButton == null || craftButtonText == null) return;

        bool canCraft = CraftingSystem.Instance != null && CraftingSystem.Instance.CanCraft(recipe);

        craftButton.interactable = canCraft;

        if (canCraft)
        {
            craftButtonText.text = "Craftar";
        }
        else
        {
            craftButtonText.text = "Materiais Insuficientes";
        }

        Debug.Log($"🎯 Botão Craft - Interativo: {craftButton.interactable}");
    }

    void OnCraftButtonClicked()
    {
        if (selectedRecipe != null && CraftingSystem.Instance != null)
        {
            bool success = CraftingSystem.Instance.CraftItem(selectedRecipe);
            if (success)
            {
                Debug.Log($"✅ Item craftado: {selectedRecipe.recipeName}");
                UpdateRecipeDetails(selectedRecipe);
                UpdateUI();
            }
        }
    }

    void OnDestroy()
    {
        if (CraftingSystem.Instance != null)
        {
            CraftingSystem.Instance.OnRecipesUpdated -= UpdateUI;
        }
    }
}