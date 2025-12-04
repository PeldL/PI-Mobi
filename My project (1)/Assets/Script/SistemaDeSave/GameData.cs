using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData Instance;
    public int coins = 100; // Moedas iniciais

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadCoins();
            Debug.Log("GameData está persistindo entre cenas.");
        }
        else
        {
            Destroy(gameObject);
            Debug.Log("Instância duplicada de GameData destruída.");
        }
    }

    public void SaveCoins()
    {
        PlayerPrefs.SetInt("Coins", coins);
        PlayerPrefs.Save();
        Debug.Log($"Moedas salvas: {coins}");
    }

    public void LoadCoins()
    {
        if (PlayerPrefs.HasKey("Coins"))
        {
            coins = PlayerPrefs.GetInt("Coins");
            Debug.Log($"Moedas carregadas: {coins}");
        }
        else
        {
            coins = 100;
            Debug.Log("Moedas iniciais definidas: 100");
        }
    }

    public void AddCoins(int amount)
    {
        coins += amount;
        Debug.Log($"Adicionando {amount} moedas. Total agora: {coins}");
        SaveCoins();
    }

    public bool SpendCoins(int amount)
    {
        if (coins >= amount)
        {
            coins -= amount;
            Debug.Log($"Gastou {amount} moedas. Restante: {coins}");
            SaveCoins();
            return true;
        }
        Debug.LogWarning($"Tentativa de gastar {amount} moedas, mas só tem {coins}.");
        return false;
    }

    private void OnApplicationQuit()
    {
        SaveCoins();
    }
}
