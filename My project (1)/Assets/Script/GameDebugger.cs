using System;
using System.IO;
using System.Text;
using UnityEngine;

public static class GameDebugger
{
    private static string logFilePath;
    private static StringBuilder logBuilder = new StringBuilder();
    private static bool isInitialized = false;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        // Criar pasta de logs se não existir
        string logFolder = Path.Combine(Application.persistentDataPath, "GameLogs");
        if (!Directory.Exists(logFolder))
            Directory.CreateDirectory(logFolder);

        // Nome do arquivo com data/hora
        string fileName = $"DebugLog_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.txt";
        logFilePath = Path.Combine(logFolder, fileName);

        // Registrar para capturar todos os logs
        Application.logMessageReceived += HandleLog;
        isInitialized = true;

        // Log inicial
        LogSystem("=== 🎮 GAME DEBUGGER INICIADO ===");
        LogSystem($"Data: {DateTime.Now}");
        LogSystem($"Platform: {Application.platform}");
        LogSystem($"Unity Version: {Application.unityVersion}");
        LogSystem($"Data Path: {Application.persistentDataPath}");
        LogSystem($"Log File: {logFilePath}");
    }

    private static void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (!isInitialized) return;

        string logEntry = $"[{DateTime.Now:HH:mm:ss.fff}] [{type}] {logString}";

        if (type == LogType.Error || type == LogType.Exception)
        {
            logEntry += $"\nStack Trace: {stackTrace}";
        }

        logBuilder.AppendLine(logEntry);

        // Salvar no arquivo a cada 10 logs (para performance)
        if (logBuilder.Length > 1000)
        {
            SaveToFile();
        }
    }

    private static void SaveToFile()
    {
        try
        {
            File.AppendAllText(logFilePath, logBuilder.ToString());
            logBuilder.Clear();
        }
        catch (Exception e)
        {
            Debug.LogError($"❌ Erro ao salvar log: {e.Message}");
        }
    }

    // ✅ MÉTODOS ESPECÍFICOS PARA SEU PROBLEMA

    [System.Diagnostics.Conditional("UNITY_EDITOR"), System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
    public static void LogRecipe(string message, CraftingRecipe recipe = null)
    {
        string logMsg = $"🍳 RECEITA: {message}";
        if (recipe != null)
        {
            logMsg += $" | Nome: {recipe.recipeName}";
            logMsg += $" | Unlocked: {recipe.isUnlocked}";
            logMsg += $" | Custo: {recipe.unlockCost}";
        }
        Debug.Log(logMsg);
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR"), System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
    public static void LogVendor(string message, RecipeVendorDialogue vendor = null)
    {
        string logMsg = $"🏪 VENDEDOR: {message}";
        if (vendor != null)
        {
            logMsg += $" | Receitas: {vendor.receitasParaVender?.Length ?? 0}";
        }
        Debug.Log(logMsg);
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR"), System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
    public static void LogCrafting(string message, CraftingSystem system = null)
    {
        string logMsg = $"🔨 CRAFTING: {message}";
        if (system != null)
        {
            logMsg += $" | Receitas: {system.allRecipes?.Count ?? 0}";
            logMsg += $" | Unlocked: {system.GetUnlockedRecipes()?.Count ?? 0}";
        }
        Debug.Log(logMsg);
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR"), System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
    public static void LogSystem(string message)
    {
        Debug.Log($"⚙️ SISTEMA: {message}");
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR"), System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
    public static void LogPlayerPrefs(string key)
    {
        if (PlayerPrefs.HasKey(key))
        {
            int value = PlayerPrefs.GetInt(key);
            Debug.Log($"💾 PLAYERPREFS: {key} = {value} ({(value == 1 ? "TRUE" : "FALSE")})");
        }
        else
        {
            Debug.Log($"💾 PLAYERPREFS: {key} = NÃO EXISTE");
        }
    }

    // ✅ MÉTODO PARA FORÇAR SALVAR LOG
    public static void ForceSave()
    {
        SaveToFile();
        Debug.Log("💾 Log salvo forçadamente!");
    }

    // ✅ MÉTODO PARA VER PASTA DE LOGS
    [ContextMenu("Abrir Pasta de Logs")]
    public static void OpenLogFolder()
    {
        string logFolder = Path.Combine(Application.persistentDataPath, "GameLogs");
        if (Directory.Exists(logFolder))
        {
            Application.OpenURL($"file://{logFolder}");
            Debug.Log($"📁 Pasta de logs aberta: {logFolder}");
        }
        else
        {
            Debug.LogWarning("📁 Pasta de logs não encontrada!");
        }
    }
}