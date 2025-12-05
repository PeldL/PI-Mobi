using UnityEngine;
using UnityEngine.UI;

public class ShopUIItem : MonoBehaviour
{
    public Text nameText;
    public Text priceText;
    public Button buyButton;

    private ShopSystem shop;
    private ItemData itemData;

    public void Setup(ShopSystem shopSystem, ShopItem shopItem)
    {
        shop = shopSystem;
        itemData = shopItem.itemData;

        nameText.text = shopItem.itemName;
        priceText.text = $"{shopItem.price} moedas";

        buyButton.onClick.AddListener(() => shop.BuyItem(itemData));
    }
}
