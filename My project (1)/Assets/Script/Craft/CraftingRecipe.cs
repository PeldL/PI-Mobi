using UnityEngine;

[CreateAssetMenu(fileName = "New Recipe", menuName = "Inventory/Crafting Recipe")]
public class CraftingRecipe : ScriptableObject
{
    [Header("Informações da Receita")]
    public string recipeName;
    [TextArea] public string description;

    [Header("Desbloqueio")]
    public bool isUnlocked = false;
    public int unlockCost = 100;

    [Header("Item Resultado")]
    public ItemData resultItem;
    public int resultAmount = 1;

    [Header("Materiais Necessários")]
    public RecipeMaterial[] materials;

    // ----------------------------------------------------------
    // 🔑 Gera uma chave única baseada no GUID do ScriptableObject
    // Isso evita que renomear a receita QUEBRE os saves.
    // ----------------------------------------------------------
    private string SaveKey => $"Recipe_{this.GetInstanceID()}_Unlocked";


    // ==========================================================
    // 💾 SALVAR ESTADO
    // ==========================================================
    public void SaveUnlockedState()
    {
        PlayerPrefs.SetInt(SaveKey, isUnlocked ? 1 : 0);
        PlayerPrefs.Save();

        Debug.Log($"💾 [SAVE] Receita '{recipeName}' = {(isUnlocked ? "DESBLOQUEADA" : "BLOQUEADA")}");
    }

    // ==========================================================
    // 📂 CARREGAR ESTADO
    // ==========================================================
    public void LoadUnlockedState()
    {
        if (PlayerPrefs.HasKey(SaveKey))
        {
            isUnlocked = PlayerPrefs.GetInt(SaveKey) == 1;
            Debug.Log($"📂 [LOAD] Receita '{recipeName}' = {(isUnlocked ? "DESBLOQUEADA" : "BLOQUEADA")}");
        }
        else
        {
            // Padrão: vem bloqueado
            isUnlocked = false;
            Debug.Log($"🆕 [FIRST TIME] Receita '{recipeName}' iniciando BLOQUEADA.");
        }
    }

    // ==========================================================
    // 🔓 DESBLOQUEAR + SALVAR
    // ==========================================================
    public void UnlockAndSave()
    {
        isUnlocked = true;
        SaveUnlockedState();

        Debug.Log($"🔓 [UNLOCK] Receita '{recipeName}' foi desbloqueada!");
    }

    // ==========================================================
    // 🚮 RESET (Útil em testes)
    // ==========================================================
    public void ResetSave()
    {
        PlayerPrefs.DeleteKey(SaveKey);
        Debug.Log($"🗑️ [RESET] Save da receita '{recipeName}' removido.");
    }
}
