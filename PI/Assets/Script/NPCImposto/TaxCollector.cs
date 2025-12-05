using UnityEngine;

public class TaxCollectorSimple : MonoBehaviour
{
    [Range(0.05f, 0.2f)] public float percentualTaxa = 0.1f; // 10%
    private bool jaCobrouHoje = false;

    void Start()
    {
        // Quando o NPC é ativado (começo do dia), reseta a cobrança
        ResetarCobrancaDiaria();
    }

    void OnEnable()
    {
        // Sempre que o NPC é reativado (novo dia), reseta
        ResetarCobrancaDiaria();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !jaCobrouHoje)
        {
            CobrarTaxa();
        }
    }

    void CobrarTaxa()
    {
        GameData gameData = GameData.Instance;
        if (gameData == null) return;

        int valorCobrado = Mathf.Max(1, Mathf.RoundToInt(gameData.coins * percentualTaxa));

        if (gameData.coins >= valorCobrado)
        {
            gameData.coins -= valorCobrado;
            jaCobrouHoje = true; // Marca que já cobrou hoje
            Debug.Log($"💰 Taxa diária coletada: {valorCobrado}G");
        }
    }

    void ResetarCobrancaDiaria()
    {
        jaCobrouHoje = false;
        Debug.Log("🔄 Cobrança diária resetada - Novo dia começou");
    }

   
}