using UnityEngine;
using UnityEngine.SceneManagement;


public class BotaoInteracao : MonoBehaviour
{
    [Header("Nome da cena para carregar")]
    public string nomeCenaDestino;

    public void TrocarCena()
    {
        if (ScenePositionManager.Instance != null)
        {
            
            ScenePositionManager.Instance.SaveOriginScene(SceneManager.GetActiveScene().name);
        }

        SceneManager.LoadScene(nomeCenaDestino);
    }
}
