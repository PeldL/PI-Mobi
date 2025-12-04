using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawnPoint : MonoBehaviour
{
    private void Start()
    {
        if (ScenePositionManager.Instance != null)
        {
            string cenaDeOrigem = ScenePositionManager.Instance.originScene;

            if (!string.IsNullOrEmpty(cenaDeOrigem))
            {
                string nomeDoPonto = "Spawn_" + cenaDeOrigem;
                GameObject ponto = GameObject.Find(nomeDoPonto);

                if (ponto != null)
                {
                    transform.position = ponto.transform.position;
                    Debug.Log("✅ Jogador spawnado em: " + nomeDoPonto);
                    return;
                }
                else
                {
                    Debug.LogWarning("⚠ Ponto de entrada '" + nomeDoPonto + "' não encontrado!");
                }
            }
        }

        Debug.Log("ℹ️ Nenhuma origem definida. Mantendo posição atual.");
    }
}
