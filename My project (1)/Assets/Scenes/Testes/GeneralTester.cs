using UnityEngine;

public class GeneralTester : MonoBehaviour
{
    [Header("Referências")]
    public ItemData madeira;
    public ItemData pedra;
    public CraftingRecipe receitaTeste;

    private void Start()
    {
        Debug.Log("🔍 INICIANDO TESTES GERAIS");
        TestAllSystems();
    }

    void TestAllSystems()
    {
        Debug.Log("=== SISTEMAS ===");
        Debug.Log($"InventorySystem: {InventorySystem.Instance != null}");
        Debug.Log($"CraftingSystem: {CraftingSystem.Instance != null}");
        Debug.Log($"CraftingUI: {CraftingUI.Instance != null}");
        Debug.Log($"GameData: {GameData.Instance != null}");

        Debug.Log("=== ITENS ===");
        Debug.Log($"Madeira: {madeira != null}");
        Debug.Log($"Pedra: {pedra != null}");

        Debug.Log("=== RECEITAS ===");
        Debug.Log($"ReceitaTeste: {receitaTeste != null}");

        if (CraftingSystem.Instance != null)
        {
            Debug.Log($"Total de receitas: {CraftingSystem.Instance.allRecipes.Count}");
        }

        Debug.Log("🟢 TESTES INICIAIS COMPLETOS");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F12))
        {
            QuickTest();
        }
    }

    void QuickTest()
    {
        Debug.Log("⚡ TESTE RÁPIDO");

        // Adiciona itens
        InventorySystem.Instance.AddItem(madeira, 5);
        InventorySystem.Instance.AddItem(pedra, 3);

        // Adiciona moedas
        GameData.Instance.AddCoins(200);

        // Tenta comprar receita
        if (receitaTeste != null)
        {
            CraftingSystem.Instance.BuyRecipe(receitaTeste);

            // Tenta craftar
            if (receitaTeste.isUnlocked)
            {
                CraftingSystem.Instance.CraftItem(receitaTeste);
            }
        }

        Debug.Log("⚡ TESTE RÁPIDO COMPLETO");
    }
}