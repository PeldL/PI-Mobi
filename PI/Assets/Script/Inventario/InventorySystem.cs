using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance;

    // Dicionário de itens (continua privado, mas com acesso público via propriedade)
    private Dictionary<ItemData, int> items = new Dictionary<ItemData, int>();

    // 🔹 Propriedade pública somente leitura
    public Dictionary<ItemData, int> Items => new Dictionary<ItemData, int>(items);

    // Evento chamado quando o inventário muda
    public delegate void OnInventoryChanged();
    public event OnInventoryChanged InventoryChanged;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadInventory();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // =======================
    // MÉTODOS PÚBLICOS
    // =======================

    public void AddItem(ItemData item, int amount = 1)
    {
        if (items.ContainsKey(item)) items[item] += amount;
        else items.Add(item, amount);

        SaveInventory();
        InventoryChanged?.Invoke();
    }

    public bool RemoveItem(ItemData item, int amount = 1)
    {
        if (!items.ContainsKey(item) || items[item] < amount) return false;

        items[item] -= amount;
        if (items[item] <= 0) items.Remove(item);

        SaveInventory();
        InventoryChanged?.Invoke();
        return true;
    }

    public bool HasItem(ItemData item, int amount = 1)
    {
        return items.ContainsKey(item) && items[item] >= amount;
    }

    public int GetItemCount(ItemData item)
    {
        return items.ContainsKey(item) ? items[item] : 0;
    }

    // 🔹 Agora o GetAllItems continua existindo também
    public Dictionary<ItemData, int> GetAllItems()
    {
        return new Dictionary<ItemData, int>(items);
    }

    // =======================
    // SALVAR / CARREGAR
    // =======================

    public void SaveInventory()
    {
        List<string> keys = new List<string>();
        List<int> values = new List<int>();

        foreach (var item in items)
        {
            keys.Add(item.Key.name);
            values.Add(item.Value);
        }

        PlayerPrefsX.SetStringArray("InventoryKeys", keys.ToArray());
        PlayerPrefsX.SetIntArray("InventoryValues", values.ToArray());
        PlayerPrefs.Save();
        Debug.Log("Inventário salvo!");
    }

    void LoadInventory()
    {
        if (PlayerPrefs.HasKey("InventoryKeys"))
        {
            string[] keys = PlayerPrefsX.GetStringArray("InventoryKeys");
            int[] values = PlayerPrefsX.GetIntArray("InventoryValues");

            for (int i = 0; i < keys.Length; i++)
            {
                ItemData item = Resources.Load<ItemData>("Items/" + keys[i]);
                if (item != null) items.Add(item, values[i]);
            }
            Debug.Log("Inventário carregado!");
        }
    }

    void OnApplicationQuit()
    {
        SaveInventory();
    }
}
