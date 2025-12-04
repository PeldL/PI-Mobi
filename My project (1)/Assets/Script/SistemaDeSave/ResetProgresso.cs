using UnityEngine;

public class ResetProgresso : MonoBehaviour
{
    [Header("Aperte V para resetar PlayerPrefs")]
    public bool ativarResetComTecla = true;

    void Update()
    {
        if (ativarResetComTecla && Input.GetKeyDown(KeyCode.V))
        {
            ResetarTudo();
        }
    }

    // Método principal de reset
    public void ResetarTudo()
    {
        // Apaga tudo do PlayerPrefs
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        // Limpa o inventário na memória usando métodos públicos
        if (InventorySystem.Instance != null)
        {
            var allItems = InventorySystem.Instance.GetAllItems();
            foreach (var kvp in allItems)
            {
                InventorySystem.Instance.RemoveItem(kvp.Key, kvp.Value);
            }
            Debug.Log("🧹 Inventário limpo da memória.");
        }

        // Reseta moedas
        if (GameData.Instance != null)
        {
            GameData.Instance.coins = 50;
            GameData.Instance.SaveCoins();
            Debug.Log("💰 Moedas resetadas.");
        }

        // 🔥 NOVO: Reseta receitas compradas
        ResetarReceitasCompradas();

        Debug.Log("🚨 Todo o progresso foi resetado!");
    }

    // 🔥 NOVO MÉTODO: Reseta todas as receitas compradas
    void ResetarReceitasCompradas()
    {
        if (CraftingSystem.Instance != null)
        {
            int receitasResetadas = 0;

            // Percorre todas as receitas do jogo
            foreach (CraftingRecipe recipe in CraftingSystem.Instance.allRecipes)
            {
                if (recipe != null && recipe.isUnlocked)
                {
                    recipe.isUnlocked = false; // Bloqueia a receita
                    receitasResetadas++;
                    Debug.Log($"🔒 Receita resetada: {recipe.recipeName}");
                }
            }

            // 🔥 IMPORTANTE: Limpa também o PlayerPrefs específico do crafting
            PlayerPrefs.DeleteKey("UnlockedRecipes");
            PlayerPrefs.Save();

            Debug.Log($"📜 {receitasResetadas} receitas foram bloqueadas.");
        }
        else
        {
            Debug.LogWarning("⚠️ CraftingSystem não encontrado para resetar receitas.");
        }
    }

    // Para chamar pelo botão da UI
    public void ResetarViaBotao()
    {
        ResetarTudo();
    }
}