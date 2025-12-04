using UnityEngine;

[System.Serializable]
public class GameDataJSON
{
    // -----------------------------
    // 🌍 DADOS GERAIS
    // -----------------------------
    public string sceneName;
    public SerializableVector3 playerPosition;

    // -----------------------------
    // 🧍 PLAYER
    // -----------------------------
    public int coins;
    public int playerHealth;
    public int playerMoney;

    // -----------------------------
    // 🎒 INVENTÁRIO
    // -----------------------------
    public string[] inventoryItems;
    public int[] inventoryQuantities;

    // -----------------------------
    // 🌱 PLANTAÇÃO
    // -----------------------------
    public string[] plantedCropTypes;
    public float[] plantedGrowthStages;

    // -----------------------------
    // 🔥 PROGRESSO / FLAGS
    // -----------------------------
    public bool cutsceneVista;
    public bool regadorDesbloqueado;
    public int pocoesFeitas;

    // -----------------------------
    // 🔧 CONSTRUTOR
    // -----------------------------
    public GameDataJSON()
    {
        // gerais
        sceneName = "MainScene";
        playerPosition = new SerializableVector3(Vector3.zero);

        // player
        coins = 0;
        playerHealth = 100;
        playerMoney = 0;

        // inventário
        inventoryItems = new string[0];
        inventoryQuantities = new int[0];

        // plantação
        plantedCropTypes = new string[0];
        plantedGrowthStages = new float[0];

        // progresso
        cutsceneVista = false;
        regadorDesbloqueado = false;
        pocoesFeitas = 0;
    }
}
