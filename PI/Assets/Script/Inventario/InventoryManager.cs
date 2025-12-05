using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [Header("UI do Inventário")]
    [SerializeField] private GameObject inventoryUI;
    [SerializeField] private KeyCode toggleKey = KeyCode.I;

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleInventory();
        }
    }

    public void ToggleInventory()
    {
        bool isOpening = !inventoryUI.activeInHierarchy;

        inventoryUI.SetActive(isOpening);
        Time.timeScale = isOpening ? 0f : 1f;
    }

    public void OpenInventory()
    {
        inventoryUI.SetActive(true);
        Time.timeScale = 0f;
    }

    public void CloseInventory()
    {
        inventoryUI.SetActive(false);
        Time.timeScale = 1f;
    }
}