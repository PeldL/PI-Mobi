using UnityEngine;

public static class SaveSystem
{
    private const string SAVE_KEY = "SaveDataJSON";

    public static void SaveGame(GameDataJSON data)
    {
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save();
        
    }

    public static GameDataJSON LoadGame()
    {
        if (!HasSave())
        {
            Debug.Log("📭 Nenhum save encontrado");
            return null;
        }

        string json = PlayerPrefs.GetString(SAVE_KEY);
        GameDataJSON data = JsonUtility.FromJson<GameDataJSON>(json);
        
        return data;
    }

    public static void DeleteSave()
    {
        if (HasSave())
        {
            PlayerPrefs.DeleteKey(SAVE_KEY);
            PlayerPrefs.Save();
            Debug.Log("🗑️ Save deletado");
        }
    }

    public static bool HasSave()
    {
        return PlayerPrefs.HasKey(SAVE_KEY);
    }

    public static GameDataJSON CreateNewSave()
    {
        GameDataJSON newSave = new GameDataJSON();
        SaveGame(newSave);
        return newSave;
    }
}