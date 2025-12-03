using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSaveLoad : MonoBehaviour
{
    private void Start()
    {
        // Se existir save, carregar posição
        if (SaveSystem.HasSave())
        {
            GameDataJSON data = SaveSystem.LoadGame();

            // Confere se está na cena correta
            if (data.sceneName == SceneManager.GetActiveScene().name)
            {
                transform.position = data.playerPosition;
                Debug.Log("[PlayerSaveLoad] Posição do player carregada: " + data.playerPosition);
            }
        }
    }

    private void OnApplicationQuit()
    {
        SavePlayerData();
    }

    private void OnDestroy()
    {
        // Garante que salva quando trocar de cena
        SavePlayerData();
    }

    public void SavePlayerData()
    {
        GameDataJSON data = new GameDataJSON();

        // Salva dados principais
        data.sceneName = SceneManager.GetActiveScene().name;
        data.playerPosition = transform.position;

        // Aqui você pode salvar mais coisas se quiser
        data.coins = 0;
        data.playerHealth = 100;
        data.playerMoney = 50;

        SaveSystem.SaveGame(data);
    }
}
