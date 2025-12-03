using UnityEngine;

public class ShopButton : MonoBehaviour
{
    public ShopSystem shopSystem;
    public ItemData itemData;
    public int quantity = 1;

    public void Buy()
    {
        shopSystem.BuyItem(itemData, quantity);
    }
}
