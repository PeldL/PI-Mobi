using UnityEngine;
using System.Collections.Generic;

public static class PlayerPrefsX
{
    // =========================
    // INT
    // =========================
    public static void SetInt(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
        UpdateGameData(key, value);
        PlayerPrefs.Save();
    }

    public static int GetInt(string key, int defaultValue = 0)
    {
        return PlayerPrefs.GetInt(key, defaultValue);
    }

    // =========================
    // FLOAT
    // =========================
    public static void SetFloat(string key, float value)
    {
        PlayerPrefs.SetFloat(key, value);
        UpdateGameData(key, value);
        PlayerPrefs.Save();
    }

    public static float GetFloat(string key, float defaultValue = 0f)
    {
        return PlayerPrefs.GetFloat(key, defaultValue);
    }

    // =========================
    // STRING
    // =========================
    public static void SetString(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
        UpdateGameData(key, value);
        PlayerPrefs.Save();
    }

    public static string GetString(string key, string defaultValue = "")
    {
        return PlayerPrefs.GetString(key, defaultValue);
    }

    // =========================
    // BOOL
    // =========================
    public static void SetBool(string key, bool value)
    {
        PlayerPrefs.SetInt(key, value ? 1 : 0);
        UpdateGameData(key, value);
        PlayerPrefs.Save();
    }

    public static bool GetBool(string key, bool defaultValue = false)
    {
        return PlayerPrefs.GetInt(key, defaultValue ? 1 : 0) == 1;
    }

    // =========================
    // VECTOR3
    // =========================
    public static void SetVector3(string key, Vector3 value)
    {
        PlayerPrefs.SetFloat(key + "_x", value.x);
        PlayerPrefs.SetFloat(key + "_y", value.y);
        PlayerPrefs.SetFloat(key + "_z", value.z);
        UpdateGameData(key, value);
        PlayerPrefs.Save();
    }

    public static Vector3 GetVector3(string key, Vector3 defaultValue = default(Vector3))
    {
        if (PlayerPrefs.HasKey(key + "_x"))
        {
            return new Vector3(
                PlayerPrefs.GetFloat(key + "_x"),
                PlayerPrefs.GetFloat(key + "_y"),
                PlayerPrefs.GetFloat(key + "_z")
            );
        }
        return defaultValue;
    }

    // =========================
    // ARRAYS - INT
    // =========================
    public static void SetIntArray(string key, int[] array)
    {
        for (int i = 0; i < array.Length; i++)
            PlayerPrefs.SetInt($"{key}_{i}", array[i]);

        PlayerPrefs.SetInt(key + "_Length", array.Length);
        UpdateGameData(key, array);
        PlayerPrefs.Save();
    }

    public static int[] GetIntArray(string key)
    {
        if (!PlayerPrefs.HasKey(key + "_Length"))
            return new int[0];

        int length = PlayerPrefs.GetInt(key + "_Length");
        int[] array = new int[length];

        for (int i = 0; i < length; i++)
            array[i] = PlayerPrefs.GetInt($"{key}_{i}");

        return array;
    }

    // =========================
    // ARRAYS - STRING
    // =========================
    public static void SetStringArray(string key, string[] array)
    {
        for (int i = 0; i < array.Length; i++)
            PlayerPrefs.SetString($"{key}_{i}", array[i]);

        PlayerPrefs.SetInt(key + "_Length", array.Length);
        UpdateGameData(key, array);
        PlayerPrefs.Save();
    }

    public static string[] GetStringArray(string key)
    {
        if (!PlayerPrefs.HasKey(key + "_Length"))
            return new string[0];

        int length = PlayerPrefs.GetInt(key + "_Length");
        string[] array = new string[length];

        for (int i = 0; i < length; i++)
            array[i] = PlayerPrefs.GetString($"{key}_{i}");

        return array;
    }

    // =========================
    // SINCRONIZAÇÃO COM JSON
    // =========================
    private static void UpdateGameData(string key, object value)
    {
        GameDataJSON data = SaveSystem.HasSave() ? SaveSystem.LoadGame() : new GameDataJSON();

        switch (key)
        {
            case "Coins":
                if (value is int) data.coins = (int)value;
                break;
            case "SceneName":
                if (value is string) data.sceneName = (string)value;
                break;
            case "PlayerPosition":
                if (value is Vector3) data.playerPosition = (Vector3)value;
                break;
            case "PlayerMoney":
                if (value is int) data.playerMoney = (int)value;
                break;
            default:
               
                break;
        }

        SaveSystem.SaveGame(data);
    }
}
