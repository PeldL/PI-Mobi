using UnityEngine;
using TMPro;

public class TestManager : MonoBehaviour
{
    [Header("Itens para Teste")]
    public ItemData madeira;
    public ItemData pedra;
    public ItemData cogumelo;
    public ItemData raiz;
    public ItemData flor;
    public ItemData pocaoVida;
    public ItemData pocaoMana;

    [Header("Receitas para Teste")]
    public CraftingRecipe receitaMachado;
    public CraftingRecipe receitaPocaoVida;
    public CraftingRecipe receitaPocaoMana;

    [Header("UI Debug")]
    public TextMeshProUGUI debugText;
    public GameObject debugPanel;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1)) TestAddItens();
        if (Input.GetKeyDown(KeyCode.F2)) TestAddMoedas();
        if (Input.GetKeyDown(KeyCode.F3)) TestComprarReceitas();
        if (Input.GetKeyDown(KeyCode.F4)) TestCraftItens();
        if (Input.GetKeyDown(KeyCode.F5)) TestMissaoPocoes();
        if (Input.GetKeyDown(KeyCode.F6)) TestResetTudo();
        if (Input.GetKeyDown(KeyCode.F7)) TestStatusSistemas();
        if (Input.GetKeyDown(KeyCode.F8)) ToggleDebugPanel();

        if (Input.GetKeyDown(KeyCode.C) && CraftingUI.Instance != null)
            CraftingUI.Instance.ToggleCrafting();
    }

    void TestAddItens()
    {
        Debug.Log("🟢 [F1] ADICIONANDO ITENS DE TESTE");

        InventorySystem.Instance.AddItem(madeira, 10);
        InventorySystem.Instance.AddItem(pedra, 8);
        InventorySystem.Instance.AddItem(cogumelo, 5);
        InventorySystem.Instance.AddItem(raiz, 5);
        InventorySystem.Instance.AddItem(flor, 5);

        UpdateDebugText("📦 Itens adicionados: Madeira(10), Pedra(8), Ingredientes(5)");
    }

    void TestAddMoedas()
    {
        Debug.Log("🟢 [F2] ADICIONANDO MOEDAS");

        GameData.Instance.AddCoins(1000);
        UpdateDebugText($"💰 +1000 Moedas! Total: {GameData.Instance.coins}");
    }

    void TestComprarReceitas()
    {
        Debug.Log("🟢 [F3] COMPRANDO RECEITAS");

        if (receitaPocaoVida != null)
            CraftingSystem.Instance.BuyRecipe(receitaPocaoVida);

        if (receitaPocaoMana != null)
            CraftingSystem.Instance.BuyRecipe(receitaPocaoMana);

        bool missaoIniciada = QuestSystem.Instance != null && QuestSystem.Instance.missaoAtiva;
        UpdateDebugText("🛒 Receitas compradas! Missão iniciada? " + missaoIniciada);
    }

    void TestCraftItens()
    {
        Debug.Log("🟢 [F4] CRAFTANDO ITENS");

        if (receitaPocaoVida != null && CraftingSystem.Instance.CanCraft(receitaPocaoVida))
            CraftingSystem.Instance.CraftItem(receitaPocaoVida);

        if (receitaPocaoMana != null && CraftingSystem.Instance.CanCraft(receitaPocaoMana))
            CraftingSystem.Instance.CraftItem(receitaPocaoMana);

        UpdateDebugText("🛠 Tentando craftar poções...");
    }

    void TestMissaoPocoes()
    {
        Debug.Log("🟢 [F5] STATUS MISSÃO POÇÕES");

        if (QuestSystem.Instance != null && QuestSystem.Instance.missaoAtiva)
        {
            string status = $"🧪 Missão Ativa: {QuestSystem.Instance.missaoPocoes.pocoesEntregues}/3 poções";
            UpdateDebugText(status);
        }
        else
        {
            UpdateDebugText("💤 Missão Inativa - Compre receitas de poção para iniciar");
        }
    }

    void TestResetTudo()
    {
        Debug.Log("🟢 [F6] RESETANDO TUDO");

        // Reset Crafting
        if (CraftingSystem.Instance != null)
            CraftingSystem.Instance.ResetAllRecipes();

        // Reset Missão
        if (QuestSystem.Instance != null)
            QuestSystem.Instance.ResetarMissao(); // 🔥 AGORA ESTÁ CORRETO!

        // Reset Inventário
        PlayerPrefs.DeleteKey("InventoryKeys");
        PlayerPrefs.DeleteKey("InventoryValues");

        // Reset Moedas
        PlayerPrefs.DeleteKey("Coins");
        if (GameData.Instance != null)
            GameData.Instance.coins = 100;

        PlayerPrefs.Save();
        UpdateDebugText("🔄 TODOS OS SISTEMAS RESETADOS!");
    }

    void TestStatusSistemas()
    {
        Debug.Log("🟢 [F7] STATUS DOS SISTEMAS");

        string status = "";
        status += $"🎮 Inventory: {InventorySystem.Instance != null}\n";
        status += $"🛠 Crafting: {CraftingSystem.Instance != null}\n";
        status += $"💰 GameData: {GameData.Instance != null}\n";
        status += $"🧪 Quest: {QuestSystem.Instance != null}\n";
        status += $"📱 CraftingUI: {CraftingUI.Instance != null}";

        UpdateDebugText(status);
    }

    void ToggleDebugPanel()
    {
        if (debugPanel != null)
        {
            debugPanel.SetActive(!debugPanel.activeSelf);
            if (debugPanel.activeSelf)
                TestStatusSistemas();
        }
    }

    void UpdateDebugText(string message)
    {
        if (debugText != null)
            debugText.text = message;
    }

    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 400));
        GUILayout.Label("🎮 TESTES RÁPIDOS - FARMING GAME");
        GUILayout.Space(10);

        if (GUILayout.Button("[F1] Add Itens")) TestAddItens();
        if (GUILayout.Button("[F2] Add Moedas")) TestAddMoedas();
        if (GUILayout.Button("[F3] Comprar Receitas")) TestComprarReceitas();
        if (GUILayout.Button("[F4] Craft Itens")) TestCraftItens();
        if (GUILayout.Button("[F5] Status Missão")) TestMissaoPocoes();
        if (GUILayout.Button("[F6] Reset Tudo")) TestResetTudo();
        if (GUILayout.Button("[F7] Status Sistemas")) TestStatusSistemas();
        if (GUILayout.Button("[F8] Toggle Debug Panel")) ToggleDebugPanel();

        GUILayout.Space(10);
        GUILayout.Label("Teclas:");
        GUILayout.Label("C = Crafting UI");
        GUILayout.Label("I = Inventário");

        // Info em tempo real
        if (GameData.Instance != null)
            GUILayout.Label($"💰 Moedas: {GameData.Instance.coins}");

        if (QuestSystem.Instance != null && QuestSystem.Instance.missaoAtiva)
            GUILayout.Label($"🧪 Poções: {QuestSystem.Instance.missaoPocoes.pocoesEntregues}/3");

        GUILayout.EndArea();
    }
}