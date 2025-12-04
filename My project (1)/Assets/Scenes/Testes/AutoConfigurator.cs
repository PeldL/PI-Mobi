using UnityEngine;

public class AutoConfigurator : MonoBehaviour
{
    [Header("Itens (Arraste do Project)")]
    public ItemData madeira;
    public ItemData pedra;
    public ItemData corda;

    [Header("Receitas (Arraste do Project)")]
    public CraftingRecipe receitaMachado;
    public CraftingRecipe receitaPicareta;

    void Start()
    {
        Debug.Log("⚙️ CONFIGURANDO AUTOMATICAMENTE...");
        ConfigureSystems();
    }

    void ConfigureSystems()
    {
        // Configura CraftingSystem
        CraftingSystem craftingSystem = FindObjectOfType<CraftingSystem>();
        if (craftingSystem != null && receitaMachado != null)
        {
            craftingSystem.allRecipes.Clear();
            craftingSystem.allRecipes.Add(receitaMachado);
            if (receitaPicareta != null) craftingSystem.allRecipes.Add(receitaPicareta);
            Debug.Log($"✅ CraftingSystem configurado com {craftingSystem.allRecipes.Count} receitas");
        }

        // Configura testers
        ConfigureTesters();
    }

    void ConfigureTesters()
    {
        // GeneralTester
        GeneralTester generalTester = FindObjectOfType<GeneralTester>();
        if (generalTester != null)
        {
            generalTester.madeira = madeira;
            generalTester.pedra = pedra;
            generalTester.receitaTeste = receitaMachado;
            Debug.Log("✅ GeneralTester configurado");
        }

        // InventoryTester
        InventoryTester inventoryTester = FindObjectOfType<InventoryTester>();
        if (inventoryTester != null)
        {
            inventoryTester.madeira = madeira;
            inventoryTester.pedra = pedra;
            inventoryTester.corda = corda;
            Debug.Log("✅ InventoryTester configurado");
        }

        // CraftingTester
        CraftingTester craftingTester = FindObjectOfType<CraftingTester>();
        if (craftingTester != null)
        {
            craftingTester.receitaMachado = receitaMachado;
            craftingTester.receitaPicareta = receitaPicareta;
            Debug.Log("✅ CraftingTester configurado");
        }
    }
}