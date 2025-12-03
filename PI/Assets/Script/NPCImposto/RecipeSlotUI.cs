using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class RecipeSlotUI : MonoBehaviour, IPointerClickHandler
{
    [Header("UI References")]
    public Image recipeIcon;
    public TextMeshProUGUI recipeName;
    public GameObject selectedHighlight;

    // ✅ CORREÇÃO: Adicionar propriedade pública
    public CraftingRecipe Recipe { get; private set; }
    private CraftingUI craftingUI;
    private bool isSelected = false;

    public void Initialize(CraftingRecipe recipeData, CraftingUI ui)
    {
        Recipe = recipeData;
        craftingUI = ui;

        // Configura informações básicas
        if (recipeIcon != null && Recipe.resultItem != null)
            recipeIcon.sprite = Recipe.resultItem.icon;

        if (recipeName != null)
            recipeName.text = Recipe.recipeName ?? "Sem Nome";

        // Inicia desselecionado
        if (selectedHighlight != null)
            selectedHighlight.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (craftingUI != null && Recipe != null)
        {
            if (isSelected)
            {
                craftingUI.DeselectCurrentRecipe();
            }
            else
            {
                craftingUI.SelectRecipe(Recipe);
            }
        }
    }

    public void Select()
    {
        isSelected = true;
        if (selectedHighlight != null)
            selectedHighlight.SetActive(true);
    }

    public void Deselect()
    {
        isSelected = false;
        if (selectedHighlight != null)
            selectedHighlight.SetActive(false);
    }

    // ✅ MÉTODO CORRETO para atualização
    public void RefreshAppearance()
    {
        // Atualiza cores baseadas se pode craftar
        if (Recipe != null)
        {
            bool canCraft = CraftingSystem.Instance.CanCraft(Recipe);
            Image bg = GetComponent<Image>();
            if (bg != null)
            {
                bg.color = canCraft ? new Color(0.6f, 1.0f, 0.6f) : Color.white;
            }
        }
    }
}