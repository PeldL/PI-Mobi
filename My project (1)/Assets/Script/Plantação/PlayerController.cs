using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Controles")]
    public KeyCode tillKey = KeyCode.T;
    public KeyCode waterKey = KeyCode.W;
    public KeyCode plantKey = KeyCode.P;
    public KeyCode harvestKey = KeyCode.H;
    public float interactRange = 1.5f;

    [Header("Referências")]
    [SerializeField] private CropData selectedCrop;

    void Start()
    {
        if (selectedCrop != null)
        {
            Debug.Log($"[INVENTÁRIO] Adicionadas 5 sementes de {selectedCrop.cropName} para teste");
            InventorySystem.Instance.AddItem(selectedCrop.seedItem, 0);
        }
    }

    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(tillKey)) Interact("Till");
        if (Input.GetKeyDown(waterKey)) Interact("Water");
        if (Input.GetKeyDown(plantKey)) Interact("Plant");
        if (Input.GetKeyDown(harvestKey)) Interact("Harvest");
    }

    // ⭐⭐ MÉTODOS PARA BOTÕES MOBILE ⭐⭐
    public void BotaoMobileArar()
    {
        Interact("Till");
    }

    public void BotaoMobileRegar()
    {
        Interact("Water");
    }

    public void BotaoMobilePlantar()
    {
        Interact("Plant");
    }

    public void BotaoMobileColher()
    {
        Interact("Harvest");
    }

    void Interact(string action)
    {
        Collider2D[] spots = Physics2D.OverlapCircleAll(transform.position, interactRange);
        if (spots.Length == 0)
        {
            Debug.LogWarning("[AVISO] Nenhum local de plantio próximo!");
            return;
        }

        foreach (Collider2D spot in spots)
        {
            FarmingSpot farmingSpot = spot.GetComponent<FarmingSpot>();
            if (farmingSpot == null) continue;

            switch (action)
            {
                case "Till":
                    farmingSpot.Till();
                    Debug.Log("🪴 Arando a terra (mobile)");
                    break;
                case "Water":
                    farmingSpot.Water();
                    Debug.Log("💧 Regando (mobile)");
                    break;
                case "Plant":
                    if (selectedCrop == null)
                    {
                        Debug.LogError("[ERRO] Selecione uma semente primeiro!");
                        return;
                    }
                    farmingSpot.Plant(selectedCrop);
                    Debug.Log($"🌱 Plantando {selectedCrop.cropName} (mobile)");
                    break;
                case "Harvest":
                    farmingSpot.Harvest();
                    Debug.Log("✂️ Colhendo (mobile)");
                    break;
            }
        }
    }

    // Método para selecionar uma cultura (se você tiver UI para isso)
    public void SelecionarCultura(CropData novaCultura)
    {
        selectedCrop = novaCultura;
        Debug.Log($"🌿 Cultura selecionada: {selectedCrop.cropName}");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}