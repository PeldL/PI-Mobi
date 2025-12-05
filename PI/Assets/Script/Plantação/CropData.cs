using UnityEngine;

[CreateAssetMenu(fileName = "NewCrop", menuName = "Farming/Crop Data")]
public class CropData : ScriptableObject
{
    [Header("Identificação")]
    public string cropName;

    [Header("Sprites")]
    [SerializeField] private Sprite[] _growthSprites;
    public Sprite[] growthSprites => _growthSprites?.Length == 3 ? _growthSprites : null;

    [Header("Tempos")]
    public float[] growthTimes = { 5f, 10f, 15f };

    [Header("Itens")]
    public ItemData seedItem;
    public ItemData harvestItem;

    [Header("Colheita")]
    [Range(1, 10)] public int minHarvest = 1;
    [Range(1, 10)] public int maxHarvest = 3;

    void OnValidate()
    {
        if (_growthSprites?.Length != 3)
            Debug.LogError($"[ERRO] {name} precisa de 3 sprites de crescimento!");

        if (seedItem == null || harvestItem == null)
            Debug.LogError($"[ERRO] {name} tem itens não configurados!");
    }
}
