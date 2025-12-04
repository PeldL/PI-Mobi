using UnityEngine;

public enum ItemType
{
    Material,   // Ingredientes para crafting
    Pocao,      // Poções craftadas
    Ferramenta, // Machados, picaretas, regador
    Semente,    // Sementes para plantar
    Quest       // Itens de missão
}

[CreateAssetMenu(fileName = "NewItem", menuName = "Farming/Item Data")]
public class ItemData : ScriptableObject
{
    [Header("Básico")]
    public string itemName;
    public Sprite icon;

    [Header("Configuração")]
    [Range(1, 999)] public int maxStack = 99;
    public ItemType itemType = ItemType.Material; // 🔥 NOVO CAMPO

    [Header("Plantio (apenas para sementes)")]
    public bool isSeed;
    public GameObject plantPrefab; // Prefab da planta que cresce

    [Header("Crafting/Vendas (opcional)")]
    public int buyPrice = 10;
    public int sellPrice = 5;

    void OnValidate()
    {
        if (icon == null)
            Debug.LogError($"[ERRO] {name} não tem sprite definido!");

        // 🔥 AUTO-CONFIGURAÇÃO: Se isSeed = true, muda automaticamente para tipo Semente
        if (isSeed)
        {
            itemType = ItemType.Semente;
        }
    }
}