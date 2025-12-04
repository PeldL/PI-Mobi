using UnityEngine;

public class GameDataTester : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F9))
        {
            TestAddCoins();
        }

        if (Input.GetKeyDown(KeyCode.F10))
        {
            TestSpendCoins();
        }

        if (Input.GetKeyDown(KeyCode.F11))
        {
            TestCoinStatus();
        }
    }

    void TestAddCoins()
    {
        Debug.Log("🟢 INICIANDO TESTE: Adicionar Moedas");

        if (GameData.Instance == null)
        {
            Debug.LogError("❌ GameData.Instance é NULL!");
            return;
        }

        int coinsBefore = GameData.Instance.coins;
        Debug.Log($"💰 Moedas antes: {coinsBefore}");

        GameData.Instance.AddCoins(100);

        int coinsAfter = GameData.Instance.coins;
        Debug.Log($"💰 Moedas depois: {coinsAfter}");
        Debug.Log($"📈 Diferença: {coinsAfter - coinsBefore} (esperado: +100)");

        Debug.Log(coinsAfter == coinsBefore + 100 ? "✅ Teste passou!" : "❌ Teste falhou!");
    }

    void TestSpendCoins()
    {
        Debug.Log("🟢 INICIANDO TESTE: Gastar Moedas");

        int coinsBefore = GameData.Instance.coins;
        Debug.Log($"💰 Moedas antes: {coinsBefore}");

        bool success = GameData.Instance.SpendCoins(50);
        Debug.Log(success ? "✅ Moedas gastas com sucesso!" : "❌ Falha ao gastar moedas");

        int coinsAfter = GameData.Instance.coins;
        Debug.Log($"💰 Moedas depois: {coinsAfter}");

        if (success)
        {
            Debug.Log(coinsAfter == coinsBefore - 50 ? "✅ Teste passou!" : "❌ Teste falhou!");
        }
    }

    void TestCoinStatus()
    {
        Debug.Log($"💰 STATUS MOEDAS: {GameData.Instance.coins}");
        Debug.Log($"💳 Pode gastar 100? {GameData.Instance.coins >= 100}");
        Debug.Log($"💳 Pode gastar 500? {GameData.Instance.coins >= 500}");
    }
}