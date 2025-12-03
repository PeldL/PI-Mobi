using UnityEngine;

public class InventoryDebugger : MonoBehaviour
{
    public ItemData seedItem; // Arraste o ItemData da semente aqui
    public int amount = 1;

    [ContextMenu("Adicionar Sementes")]
    public void AddSeeds()
    {
        InventorySystem.Instance.AddItem(seedItem, amount);
        Debug.Log($"Adicionou {amount} {seedItem.itemName}");
    }

    [ContextMenu("Remover Sementes")]
    public void RemoveSeeds()
    {
        bool success = InventorySystem.Instance.RemoveItem(seedItem, amount);
        Debug.Log(success ? $"Removeu {amount} {seedItem.itemName}" : "Falha ao remover!");
    }
}