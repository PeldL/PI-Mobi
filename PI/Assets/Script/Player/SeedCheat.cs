using UnityEngine;

public class SeedCheat : MonoBehaviour
{
    public ItemData seedItem;
    public KeyCode cheatKey = KeyCode.F9;

    void Update()
    {
        if (Input.GetKeyDown(cheatKey))
        {
            InventorySystem.Instance.AddItem(seedItem, 10);
            Debug.Log($"+10 {seedItem.itemName} adicionadas (Cheat)");
        }
    }
}
