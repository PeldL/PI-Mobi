using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // 🔥 Mudei de 'instance' para 'Instance'

    public GameObject player;

    private void Awake()
    {
        // 🔥 ADICIONA ESTA VERIFICAÇÃO DE SINGLETON
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 🔥 FAZ PERSISTIR ENTRE CENAS
            Debug.Log("✅ GameManager persistindo entre cenas");
        }
        else
        {
            Destroy(gameObject); // ⚠️ Destrói apenas duplicatas
            Debug.Log("⚠️ GameManager duplicado destruído");
        }
    }
}