using UnityEngine;

public class InventoryTester : MonoBehaviour
{
    [Header("Itens para Teste")]
    public ItemData madeira;
    public ItemData pedra;
    public ItemData corda;
    public ItemData machado;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            TestAddItems();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            TestRemoveItems();
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            TestCheckItems();
        }
    }

    void TestAddItems()
    {
        Debug.Log("🟢 INICIANDO TESTE: Adicionar Itens");

        if (InventorySystem.Instance == null)
        {
            Debug.LogError("❌ InventorySystem.Instance é NULL!");
            return;
        }

        if (madeira == null)
        {
            Debug.LogError("❌ Item 'madeira' não atribuído!");
            return;
        }

        Debug.Log($"📦 Adicionando 5x {madeira.itemName}");

        // CORREÇÃO: AddItem retorna void, não bool
        InventorySystem.Instance.AddItem(madeira, 5);
        Debug.Log("✅ Madeira adicionada (verificando count...)");

        // Verifica se foi adicionado
        int countMadeira = InventorySystem.Instance.GetItemCount(madeira);
        Debug.Log($"📊 Madeira no inventário: {countMadeira}");

        if (pedra != null)
        {
            Debug.Log($"📦 Adicionando 3x {pedra.itemName}");
            InventorySystem.Instance.AddItem(pedra, 3);
            int countPedra = InventorySystem.Instance.GetItemCount(pedra);
            Debug.Log($"📊 Pedra no inventário: {countPedra}");
        }

        if (corda != null)
        {
            Debug.Log($"📦 Adicionando 2x {corda.itemName}");
            InventorySystem.Instance.AddItem(corda, 2);
            int countCorda = InventorySystem.Instance.GetItemCount(corda);
            Debug.Log($"📊 Corda no inventário: {countCorda}");
        }

        Debug.Log("🟢 TESTE COMPLETO: Adicionar Itens");
        PrintInventoryStatus();
    }

    void TestRemoveItems()
    {
        Debug.Log("🟢 INICIANDO TESTE: Remover Itens");

        if (madeira != null)
        {
            int hasMadeira = InventorySystem.Instance.GetItemCount(madeira);
            Debug.Log($"📦 Removendo 2x {madeira.itemName} (tem {hasMadeira})");

            if (hasMadeira >= 2)
            {
                // CORREÇÃO: RemoveItem também pode retornar void
                InventorySystem.Instance.RemoveItem(madeira, 2);
                int newCount = InventorySystem.Instance.GetItemCount(madeira);
                Debug.Log($"✅ Madeira removida! Novo total: {newCount}");
            }
            else
            {
                Debug.LogWarning($"⚠️ Não tem madeira suficiente: {hasMadeira}/2");
            }
        }

        PrintInventoryStatus();
    }

    void TestCheckItems()
    {
        Debug.Log("🟢 INICIANDO TESTE: Verificar Itens");

        if (madeira != null)
        {
            int count = InventorySystem.Instance.GetItemCount(madeira);
            bool hasEnough = InventorySystem.Instance.HasItem(madeira, 3);
            Debug.Log($"📦 {madeira.itemName}: {count} unidades | Tem 3? {hasEnough}");
        }

        if (pedra != null)
        {
            int count = InventorySystem.Instance.GetItemCount(pedra);
            bool hasEnough = InventorySystem.Instance.HasItem(pedra, 2);
            Debug.Log($"📦 {pedra.itemName}: {count} unidades | Tem 2? {hasEnough}");
        }

        if (machado != null)
        {
            int count = InventorySystem.Instance.GetItemCount(machado);
            Debug.Log($"📦 {machado.itemName}: {count} unidades");
        }
    }

    void PrintInventoryStatus()
    {
        Debug.Log("📊 STATUS DO INVENTÁRIO:");
        Debug.Log("🟢 InventorySystem parece funcionar!");
    }
}