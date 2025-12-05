using System.Collections.Generic;
using UnityEngine;

public class ShopSystem : MonoBehaviour
{
    [Header("Itens disponíveis na loja")]
    public List<ShopItem> shopItems = new List<ShopItem>();
    
    public ItemData itemToBuy;

    public void Buy1x() => BuyItem(itemToBuy, 1);
    public void Buy2x() => BuyItem(itemToBuy, 2);
    public void Buy10x() => BuyItem(itemToBuy, 10);


    private void Start()
    {
        PrintShop(); 
    }

    public void BuyItem(ItemData itemData, int quantity = 1)
    {
        ShopItem item = shopItems.Find(i => i.itemData == itemData);

        if (item == null)
        {
            Debug.LogWarning("Item não encontrado na loja.");
            return;
        }

        int totalPrice = item.price * quantity;

        if (GameData.Instance.coins >= totalPrice)
        {
            GameData.Instance.coins -= totalPrice;
            InventorySystem.Instance.AddItem(item.itemData, quantity);
            Debug.Log($"✅ Comprou {quantity}x {item.itemName} por {totalPrice} moedas. Moedas restantes: {GameData.Instance.coins}");
        }
        else
        {
            Debug.Log("❌ Moedas insuficientes para comprar esse pacote.");
        }
    }

    public void PrintShop()
    {
        Debug.Log("🛒 Itens disponíveis na loja:");
        foreach (var item in shopItems)
        {
            Debug.Log($"- {item.itemName}: {item.price} moedas");
        }
    }
}