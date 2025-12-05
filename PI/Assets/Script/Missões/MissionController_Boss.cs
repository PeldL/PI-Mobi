using UnityEngine;

public class MissionController_Boss : MonoBehaviour
{
    [Header("Identificação da Missão")]
    [SerializeField] private string idMissao = "missao_boss";
    [SerializeField] private string nomeMissao = "Derrote o Boss";

    [Header("Condições")]
    [SerializeField] private bool bossDerrotadoExternamente = false;

    [Header("Recompensas")]
    [SerializeField] private bool darMachado = true;

    private bool concluida = false;

    void Update()
    {
        if (!concluida && VerificarSeBossDerrotado())
        {
            ConcluirMissao();
        }
    }

    bool VerificarSeBossDerrotado()
    {
        return MissionManager.Instance != null && MissionManager.Instance.bossDerrotado;
    }

    void ConcluirMissao()
    {
        concluida = true;
        Debug.Log($"✅ Missão '{nomeMissao}' concluída!");

        var player = FindFirstObjectByType<PlayerRegador>();
        if (player != null)
        {
            if (darMachado)
            {
                Debug.Log("🪓 Machado desbloqueado diretamente pela missão do boss!");
                player.DesbloquearMachado();
            }
        }
        else
        {
            Debug.LogWarning("⚠️ PlayerRegador não encontrado na cena para dar o machado!");
        }
    }
}
