using UnityEngine;

public class CraftingTester : MonoBehaviour
{
    [Header("Receitas para Testar")]
    public CraftingRecipe receitaMachado;
    public CraftingRecipe receitaPicareta;
    public CraftingRecipe receitaPocao;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            TestUnlockRecipe();
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            TestCraftRecipe();
        }

        if (Input.GetKeyDown(KeyCode.F3))
        {
            TestCanCraft();
        }

        if (Input.GetKeyDown(KeyCode.F4))
        {
            TestAllRecipesStatus();
        }
    }

    void TestUnlockRecipe()
    {
        Debug.Log("🟢 INICIANDO TESTE: Desbloquear Receita");

        if (CraftingSystem.Instance == null)
        {
            Debug.LogError("❌ CraftingSystem.Instance é NULL!");
            return;
        }

        if (receitaMachado == null)
        {
            Debug.LogError("❌ ReceitaMachado não atribuída!");
            return;
        }

        Debug.Log($"🔓 Tentando desbloquear: {receitaMachado.recipeName}");
        Debug.Log($"💰 Custo: {receitaMachado.unlockCost} | Já desbloqueada? {receitaMachado.isUnlocked}");

        // Garante que tem moedas
        GameData.Instance.AddCoins(200);
        Debug.Log($"💰 Moedas disponíveis: {GameData.Instance.coins}");

        // CORREÇÃO: BuyRecipe retorna bool, então está correto
        bool success = CraftingSystem.Instance.BuyRecipe(receitaMachado);
        Debug.Log(success ? "✅ Receita desbloqueada com sucesso!" : "❌ Falha ao desbloquear receita");

        if (success)
        {
            Debug.Log($"🔓 Status após desbloqueio: {receitaMachado.isUnlocked}");
        }
    }

    void TestCraftRecipe()
    {
        Debug.Log("🟢 INICIANDO TESTE: Craftar Receita");

        if (receitaMachado == null)
        {
            Debug.LogError("❌ ReceitaMachado não atribuída!");
            return;
        }

        Debug.Log($"🛠️ Tentando craftar: {receitaMachado.recipeName}");
        Debug.Log($"📦 Resultado: {receitaMachado.resultAmount}x {receitaMachado.resultItem.itemName}");
        Debug.Log($"🔓 Receita desbloqueada? {receitaMachado.isUnlocked}");

        // Verifica materiais antes
        Debug.Log("📋 Verificando materiais necessários:");
        foreach (var material in receitaMachado.materials)
        {
            if (material.item != null)
            {
                int playerAmount = InventorySystem.Instance.GetItemCount(material.item);
                Debug.Log($"   - {material.item.itemName}: {playerAmount}/{material.amount} {(playerAmount >= material.amount ? "✅" : "❌")}");
            }
        }

        // CORREÇÃO: CraftItem retorna bool, então está correto
        bool success = CraftingSystem.Instance.CraftItem(receitaMachado);
        Debug.Log(success ? "✅ Item craftado com sucesso!" : "❌ Falha ao craftar item");

        if (success)
        {
            // Verifica se o item foi adicionado
            int resultCount = InventorySystem.Instance.GetItemCount(receitaMachado.resultItem);
            Debug.Log($"📦 {receitaMachado.resultItem.itemName} no inventário: {resultCount}");
        }
    }

    void TestCanCraft()
    {
        Debug.Log("🟢 INICIANDO TESTE: Pode Craftar?");

        if (receitaMachado != null)
        {
            // CORREÇÃO: CanCraft retorna bool, então está correto
            bool canCraft = CraftingSystem.Instance.CanCraft(receitaMachado);
            Debug.Log($"🛠️ {receitaMachado.recipeName}: {(canCraft ? "✅ PODE CRAFTAR" : "❌ NÃO PODE CRAFTAR")}");

            if (!canCraft)
            {
                Debug.Log("📋 Motivo (materiais faltando):");
                foreach (var material in receitaMachado.materials)
                {
                    if (material.item != null)
                    {
                        int playerAmount = InventorySystem.Instance.GetItemCount(material.item);
                        if (playerAmount < material.amount)
                        {
                            Debug.Log($"   - ❌ {material.item.itemName}: {playerAmount}/{material.amount}");
                        }
                    }
                }
            }
        }
    }

    void TestAllRecipesStatus()
    {
        Debug.Log("🟢 INICIANDO TESTE: Status de Todas as Receitas");

        if (CraftingSystem.Instance == null)
        {
            Debug.LogError("❌ CraftingSystem.Instance é NULL!");
            return;
        }

        Debug.Log($"📋 Total de receitas: {CraftingSystem.Instance.allRecipes.Count}");

        foreach (var recipe in CraftingSystem.Instance.allRecipes)
        {
            if (recipe != null)
            {
                // CORREÇÃO: CanCraft retorna bool, então está correto
                bool canCraft = CraftingSystem.Instance.CanCraft(recipe);
                Debug.Log($"📦 {recipe.recipeName}: " +
                         $"Desbloqueada={recipe.isUnlocked} | " +
                         $"PodeCraftar={canCraft} | " +
                         $"Resultado={recipe.resultAmount}x {recipe.resultItem.itemName}");
            }
        }
    }
}