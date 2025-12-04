using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingSystem : MonoBehaviour
{
    public static CraftingSystem Instance;

    public List<CraftingRecipe> allRecipes = new List<CraftingRecipe>();

    public System.Action OnRecipesUpdated;
    public System.Action<CraftingRecipe> OnRecipeCrafted;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private IEnumerator Start()
    {
        yield return null;

        LoadUnlockedRecipes();
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void CreateInstance()
    {
        if (Instance == null)
        {
            GameObject obj = new GameObject("CraftingSystem_Auto");
            obj.AddComponent<CraftingSystem>();
        }
    }

    public bool BuyRecipe(CraftingRecipe recipe)
    {
        if (recipe == null || recipe.isUnlocked)
            return false;

        if (GameData.Instance.SpendCoins(recipe.unlockCost))
        {
            UnlockRecipe(recipe);
            return true;
        }

        return false;
    }

    public void UnlockRecipe(CraftingRecipe recipe)
    {
        recipe.isUnlocked = true;
        SaveUnlockedRecipes();
        OnRecipesUpdated?.Invoke();
    }

    public bool CanCraft(CraftingRecipe recipe)
    {
        if (!recipe.isUnlocked)
            return false;

        foreach (var m in recipe.materials)
        {
            if (!InventorySystem.Instance.HasItem(m.item, m.amount))
                return false;
        }

        return true;
    }

    public bool CraftItem(CraftingRecipe recipe)
    {
        if (!CanCraft(recipe))
            return false;

        foreach (var m in recipe.materials)
            InventorySystem.Instance.RemoveItem(m.item, m.amount);

        InventorySystem.Instance.AddItem(recipe.resultItem, recipe.resultAmount);

        QuestSystem.Instance?.OnPocaoCraftada(recipe);

        OnRecipeCrafted?.Invoke(recipe);
        OnRecipesUpdated?.Invoke();

        return true;
    }

    // ---------------------------
    //      SAVE / LOAD FIX
    // ---------------------------

    private void SaveUnlockedRecipes()
    {
        List<string> unlockedIDs = new List<string>();

        foreach (var recipe in allRecipes)
        {
            if (recipe.isUnlocked)
                unlockedIDs.Add(recipe.recipeName);
        }

        PlayerPrefsX.SetStringArray("UnlockedRecipes", unlockedIDs.ToArray());
        PlayerPrefs.Save();
    }

    private void LoadUnlockedRecipes()
    {
        string[] loadedIDs = PlayerPrefsX.GetStringArray("UnlockedRecipes");

        if (loadedIDs == null)
            return;

        foreach (var recipe in allRecipes)
            recipe.isUnlocked = false;

        foreach (string id in loadedIDs)
        {
            CraftingRecipe r = allRecipes.Find(x => x.recipeName == id);

            if (r != null)
                r.isUnlocked = true;
        }
    }

    public List<CraftingRecipe> GetUnlockedRecipes()
    {
        return allRecipes.FindAll(r => r.isUnlocked);
    }

    [ContextMenu("Resetar Todas Receitas")]
    public void ResetAllRecipes()
    {
        foreach (var recipe in allRecipes)
            recipe.isUnlocked = false;

        PlayerPrefs.DeleteKey("UnlockedRecipes");
        PlayerPrefs.Save();

        OnRecipesUpdated?.Invoke();
    }
}
