using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VendorRecipeSlotUI : MonoBehaviour
{
    [Header("UI References")]
    public Image itemIcon;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI priceText;
    public TextMeshProUGUI descriptionText;
    public Button buyButton;
    public GameObject ownedTag;

    private CraftingRecipe recipe;
    private RecipeVendorDialogue vendor;

    public void Initialize(CraftingRecipe recipeData, RecipeVendorDialogue vendorRef)
    {
        recipe = recipeData;
        vendor = vendorRef;

        // Debug visual
        Image bg = GetComponent<Image>();
        if (bg != null)
        {
            bg.color = Color.red;
            bg.rectTransform.sizeDelta = new Vector2(350, 80);
        }

        // Configura UI
        if (itemIcon != null && recipeData.resultItem != null)
            itemIcon.sprite = recipeData.resultItem.icon;

        if (itemNameText != null)
        {
            itemNameText.text = $"🎯 {recipeData.resultItem.itemName}";
            itemNameText.color = Color.white;
            itemNameText.fontSize = 20;
        }

        if (priceText != null)
        {
            priceText.text = $"{recipeData.unlockCost}G";
            priceText.color = Color.yellow;
            priceText.fontSize = 18;
        }

        if (descriptionText != null)
            descriptionText.text = recipeData.description;

        // Status
        if (ownedTag != null)
            ownedTag.SetActive(recipeData.isUnlocked);

        // Botão
        if (buyButton != null)
        {
            buyButton.interactable = !recipeData.isUnlocked;

            Image btnImage = buyButton.GetComponent<Image>();
            if (btnImage != null)
                btnImage.color = recipeData.isUnlocked ? Color.gray : Color.green;

            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(OnBuyClick); // 🔥 NOME CORRETO
        }

        Debug.Log($"✅ Slot {recipeData.recipeName} inicializado!");
    }

    // 🔥 MÉTODO CORRETO - OnBuyClick
    // NO VendorRecipeSlotUI.cs, ADICIONE:

    public void RefreshUI()
    {
        // Atualiza o status da receita
        if (ownedTag != null)
            ownedTag.SetActive(recipe.isUnlocked);

        // Atualiza botão
        if (buyButton != null)
        {
            buyButton.interactable = !recipe.isUnlocked && GameData.Instance.coins >= recipe.unlockCost;

            Image btnImage = buyButton.GetComponent<Image>();
            if (btnImage != null)
            {
                if (recipe.isUnlocked)
                    btnImage.color = Color.gray;
                else if (GameData.Instance.coins >= recipe.unlockCost)
                    btnImage.color = Color.green;
                else
                    btnImage.color = Color.red;
            }
        }

        // Atualiza texto do preço
        if (priceText != null)
        {
            priceText.text = $"{recipe.unlockCost}G";
            priceText.color = GameData.Instance.coins >= recipe.unlockCost ? Color.yellow : Color.red;
        }
    }

    // 🔥 MÉTODO CORRETO PARA COMPRAR
    public void OnBuyClick()
    {
        Debug.Log($"🛒 Clicou para comprar: {recipe.recipeName}");
        vendor.ComprarReceita(recipe);
    }

}