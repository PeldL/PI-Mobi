using UnityEngine;
using UnityEngine.SceneManagement;

public class TrocarCenaAposDesativacao : MonoBehaviour
{
    [Header("Objeto a ser monitorado")]
    public GameObject objetoMonitorado;

    [Header("Cena para carregar após desativação")]
    public string nomeCenaDestino;

    private bool foiAtivado = false;

    void Update()
    {
        if (objetoMonitorado == null) return;

        // Detecta se foi ativado
        if (!foiAtivado && objetoMonitorado.activeSelf)
        {
            foiAtivado = true;
        }

        // Se já foi ativado e agora está desativado, troca de cena
        if (foiAtivado && !objetoMonitorado.activeSelf)
        {
            TrocarCena();
            enabled = false; // evita chamadas múltiplas
        }
    }

    void TrocarCena()
    {
        // Salva a cena de origem, se usar sistema de spawn
        if (ScenePositionManager.Instance != null)
        {
            ScenePositionManager.Instance.SaveOriginScene(SceneManager.GetActiveScene().name);
        }

        Debug.Log("Cena será trocada automaticamente.");
        SceneManager.LoadScene(nomeCenaDestino);
    }
}
