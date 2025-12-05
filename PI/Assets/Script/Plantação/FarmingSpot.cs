using UnityEngine;
using System.Collections;

public class FarmingSpot : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite defaultSprite;
    public Sprite tilledSprite;
    public Sprite wateredSprite;
    public Sprite[] plantSprites;

    [Header("Configuração")]
    public float[] growthStageTimes = { 5f, 10f, 15f };
    public ItemData harvestItem;
    public int minHarvest = 1;
    public int maxHarvest = 3;

    // Estado
    private bool isTilled = false;
    private bool isWatered = false;
    private int currentStage = -1;
    private SpriteRenderer sr;
    private CropData plantedCrop; 

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr == null) sr = gameObject.AddComponent<SpriteRenderer>();
        ResetPlot();
    }

    public void Till()
    {
        if (!isTilled)
        {
            sr.sprite = tilledSprite;
            isTilled = true;
            Debug.Log("[SISTEMA] Solo arado com sucesso!");
        }
        else
        {
            Debug.LogWarning("[AVISO] Solo já está arado!");
        }
    }

    public void Water()
    {
        if (!isTilled)
        {
            Debug.LogError("[ERRO] Arade o solo antes de regar!");
            return;
        }

        if (!isWatered)
        {
            sr.sprite = wateredSprite;
            isWatered = true;
            Debug.Log("[SISTEMA] Solo regado com sucesso!");
        }
    }

    public void Plant(CropData crop)
    {
        if (crop == null)
        {
            Debug.LogWarning("[ERRO] Nenhuma planta selecionada!");
            return;
        }

        if (!isTilled || !isWatered)
        {
            Debug.LogWarning("[ERRO] Solo precisa ser arado e regado antes!");
            return;
        }

        if (currentStage != -1)
        {
            Debug.LogWarning("[AVISO] Já existe uma planta aqui!");
            return;
        }

        if (!InventorySystem.Instance.HasItem(crop.seedItem))
        {
            Debug.LogWarning($"[AVISO] Sem sementes de {crop.cropName} no inventário!");
            return;
        }

        plantSprites = crop.growthSprites;
        harvestItem = crop.harvestItem;
        minHarvest = crop.minHarvest;
        maxHarvest = crop.maxHarvest;
        plantedCrop = crop;

        currentStage = 0;
        sr.sprite = plantSprites[currentStage];
        InventorySystem.Instance.RemoveItem(crop.seedItem, 1);

        Debug.Log($"[SISTEMA] {crop.cropName} plantada com sucesso!");
        StartCoroutine(Grow());
    }

    IEnumerator Grow()
    {
        while (currentStage < plantSprites.Length - 1)
        {
            yield return new WaitForSeconds(growthStageTimes[currentStage]);
            currentStage++;
            sr.sprite = plantSprites[currentStage];
            Debug.Log($"[CRESCIMENTO] Planta avançou para estágio {currentStage + 1}/{plantSprites.Length}");
        }
    }

    public void Harvest()
    {
        if (currentStage != plantSprites.Length - 1)
        {
            Debug.LogWarning("[AVISO] Planta não está pronta para colheita!");
            return;
        }

        int yield = Random.Range(minHarvest, maxHarvest + 1);
        InventorySystem.Instance.AddItem(harvestItem, yield);
        Debug.Log($"[COLHEITA] Colhido {yield}x {harvestItem.itemName}!");

        if (Random.value <= 0.3f && plantedCrop != null)
        {
            InventorySystem.Instance.AddItem(plantedCrop.seedItem, 1);
            Debug.Log("[BÔNUS] Ganhou 1 semente de volta!");
        }

        ResetPlot();
    }

    void ResetPlot()
    {
        sr.sprite = defaultSprite;
        isTilled = false;
        isWatered = false;
        currentStage = -1;
        plantedCrop = null; 
    }
}
