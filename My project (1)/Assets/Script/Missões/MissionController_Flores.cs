using UnityEngine;

public class MissionController_Flores : MonoBehaviour
{
    [Header("Identificação da Missão")]
    [SerializeField] private string idMissao = "missao_flores";
    [SerializeField] private string nomeMissao = "Entregue 100 flores";
    [TextArea][SerializeField] private string descricao = "Ajude Dona Zefa entregando 100 flores.";

    [Header("Condições da Missão")]
    [SerializeField] private int totalNecessario = 100;
    [SerializeField] private bool ativarSomenteAposMissaoAnterior = true;

    [Header("Recompensas")]
    [SerializeField] private bool darMachado = false;
    [SerializeField] private bool darRegador = true;

    [Header("Referência ao NPC")]
    [SerializeField] private NPCDialogueSimple npcDialogue;

    private bool concluida = false;

    void Start()
    {
        if (ativarSomenteAposMissaoAnterior &&
            (MissionManager.Instance == null || !MissionManager.Instance.bossDerrotado))
        {
            Debug.Log($"🟡 Missão '{nomeMissao}' desativada (aguardando missão anterior).");
            enabled = false;
            return;
        }
    }

    void Update()
    {
        if (!concluida && VerificarSeCompleta())
        {
            ConcluirMissao();
        }
    }

    bool VerificarSeCompleta()
    {
        int total = 0;

        // Usa GetAllItems() ao invés de acessar items diretamente
        var allItems = InventorySystem.Instance.GetAllItems();
        foreach (var kvp in allItems)
        {
            total += kvp.Value;
        }

        return total >= totalNecessario;
    }

    void ConcluirMissao()
    {
        concluida = true;
        npcDialogue.missaoCompleta = true;
        npcDialogue.SetRecompensas(darMachado, darRegador);

        // 🚨 Aqui já damos a recompensa sem depender do diálogo
        var player = FindFirstObjectByType<PlayerRegador>();
        if (player != null)
        {
            if (darMachado)
            {
                Debug.Log("🔓 Machado desbloqueado diretamente pela missão!");
                player.DesbloquearMachado();
            }

            if (darRegador)
            {
                Debug.Log("🔓 Regador desbloqueado diretamente pela missão!");
                player.DesbloquearRegador();
            }
        }
        else
        {
            Debug.LogWarning("PlayerRegador não encontrado na cena!");
        }

        Debug.Log($"✅ Missão '{nomeMissao}' concluída!");
    }
}
