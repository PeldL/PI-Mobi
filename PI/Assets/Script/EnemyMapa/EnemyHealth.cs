using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyHealth : MonoBehaviour
{
    [Header("Configurações")]
    public int vidaMaxima = 7;
    public int vidaAtual;

    [Header("UI")]
    public VidaUIEnemy vidaUI;

    [Header("Cena ao morrer")]
    public string nomeCenaParaTrocar;

    void Start()
    {
        vidaAtual = vidaMaxima;
        if (vidaUI != null)
        {
            vidaUI.Inicializar(vidaMaxima);
            vidaUI.AtualizarVida(vidaAtual);
        }
        else
        {
            Debug.LogError("VidaUIEnemy não atribuído no Inspector!");
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Light"))
        {
            TomarDano(1);
        }
    }

    public void TomarDano(int dano)
    {
        vidaAtual -= dano;
        vidaAtual = Mathf.Clamp(vidaAtual, 0, vidaMaxima);

        if (vidaUI != null)
            vidaUI.AtualizarVida(vidaAtual);

        if (vidaAtual <= 0)
            Morrer();
    }

    void Morrer()
    {
        if (MissionManager.Instance != null)
        {
            MissionManager.Instance.bossDerrotado = true;
            MissionManager.Instance.segundaMissaoAtivada = true;
            Debug.Log("☠️ Boss derrotado! Missão 1 completa.");
        }

        Invoke("TrocarCena", 0.5f);
        Destroy(gameObject, 0.5f);
    }

    void TrocarCena()
    {
        if (!string.IsNullOrEmpty(nomeCenaParaTrocar))
        {
            SceneManager.LoadScene(nomeCenaParaTrocar);
        }
    }
}
