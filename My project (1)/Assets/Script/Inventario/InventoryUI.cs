using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [Header("Referências")]
    public GameObject slotPrefab;
    public Transform inventoryPanel;

    private List<GameObject> currentSlots = new List<GameObject>();

    private void Start()
    {
        if (InventorySystem.Instance != null)
        {
            InventorySystem.Instance.InventoryChanged += UpdateUI;
            UpdateUI();
        }
    }

    public void UpdateUI()
    {
        ClearSlots();

        // Usa seu método original GetAllItems() - COMPATÍVEL
        Dictionary<ItemData, int> allItems = InventorySystem.Instance.GetAllItems();

        foreach (var kvp in allItems)
        {
            ItemData item = kvp.Key;
            int amount = kvp.Value;

            GameObject slotGO = Instantiate(slotPrefab, inventoryPanel);
            slotGO.SetActive(true);
            currentSlots.Add(slotGO);

            InventorySlotUI slotUI = slotGO.GetComponent<InventorySlotUI>();

            if (slotUI != null)
                slotUI.UpdateSlot(item, amount);
            else
                Debug.LogError("Prefab do slot não tem InventorySlotUI!");
        }
    }

    private void ClearSlots()
    {
        foreach (GameObject slot in currentSlots)
            Destroy(slot);
        currentSlots.Clear();
    }

    private void OnDestroy()
    {
        if (InventorySystem.Instance != null)
            InventorySystem.Instance.InventoryChanged -= UpdateUI;
    }
}