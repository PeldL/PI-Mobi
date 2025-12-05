using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlotUI : MonoBehaviour
{
    [Header("Referências do Slot")]
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemAmount;

    public void UpdateSlot(ItemData data, int quantity)
    {
        if (data == null)
        {
            ClearSlot();
            return;
        }

        if (icon != null)
        {
            icon.sprite = data.icon;
            icon.enabled = true;
        }

        if (itemName != null)
            itemName.text = data.itemName;

        if (itemAmount != null)
            itemAmount.text = quantity > 1 ? quantity.ToString() : "";
    }

    private void ClearSlot()
    {
        if (icon != null)
            icon.enabled = false;
        if (itemName != null)
            itemName.text = "";
        if (itemAmount != null)
            itemAmount.text = "";
    }
}