using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CasaInteracao : MonoBehaviour
{
    public GameObject botaoUI;
    public string nomeCena;

    private bool jogadorPerto = false;
    private GameObject jogador;

    void Start()
    {
        if (botaoUI != null)
            botaoUI.SetActive(false);
    }

    void Update()
    {
        if (jogadorPerto && Input.GetKeyDown(KeyCode.E))
        {
            EntrarNaCasa();
        }
    }

    // ⭐⭐ MÉTODO PARA BOTÃO MOBILE (OnClick direto na casa) ⭐⭐
    public void BotaoMobileInteragir()
    {
        if (jogadorPerto)
        {
            EntrarNaCasa();
        }
    }

    void EntrarNaCasa()
    {
        // Salvar a cena atual como origem antes de trocar
        if (ScenePositionManager.Instance != null)
        {
            ScenePositionManager.Instance.SaveOriginScene(SceneManager.GetActiveScene().name);
        }

        SceneManager.LoadScene(nomeCena);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jogadorPerto = true;
            jogador = other.gameObject;

            if (botaoUI != null)
                botaoUI.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jogadorPerto = false;
            jogador = null;

            if (botaoUI != null)
                botaoUI.SetActive(false);
        }
    }
}