using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PuzzleEEncherAgua : MonoBehaviour
{
    [Header("UI - Puzzle de Senha")]
    public GameObject passwordPanel;
    public TMP_InputField passwordInput;
    public Button confirmButton;
    public TextMeshProUGUI messageText;
    public string correctPassword = "1234";

    [Header("UI - Pressione E")]
    public GameObject pressEIndicator;

    [Header("Objetos")]
    public GameObject objectToChangeColor;
    public GameObject playerObject;

    [Header("Modo Fonte de Água")]
    public bool regadorDesbloqueado = false;

    private bool playerInRange = false;
    private bool resolvido = false;
    private bool interagindo = false;

    void Start()
    {
        confirmButton.onClick.AddListener(VerificarSenha);
        if (pressEIndicator != null) pressEIndicator.SetActive(false);
        if (passwordPanel != null) passwordPanel.SetActive(false);
        messageText.text = "";
    }

    void Update()
    {
        if (!playerInRange) return;

        if (resolvido)
        {
            // Modo Fonte
            if (Input.GetKeyDown(KeyCode.E))
            {
                var player = FindFirstObjectByType<PlayerRegador>();
                if (player != null && player.regadorDesbloqueado)
                {
                    player.EncherRegador();
                }
            }
        }
        else
        {
            // Modo Puzzle
            if (!interagindo && Input.GetKeyDown(KeyCode.E))
            {
                AbrirPainel();
            }
            else if (interagindo && Input.GetKeyDown(KeyCode.Escape))
            {
                FecharPainel();
            }
        }
    }

    void AbrirPainel()
    {
        interagindo = true;
        passwordPanel.SetActive(true);
        if (pressEIndicator != null) pressEIndicator.SetActive(false);

        // Pausar o jogo
        Time.timeScale = 0f;
    }

    void FecharPainel()
    {
        interagindo = false;
        passwordPanel.SetActive(false);
        messageText.text = "";

        // Despausar o jogo
        Time.timeScale = 1f;
    }

    void VerificarSenha()
    {
        string entered = passwordInput.text;
        if (entered == correctPassword)
        {
            messageText.text = "Senha Correta!";
            messageText.color = Color.green;

            if (objectToChangeColor != null)
            {
                Renderer renderer = objectToChangeColor.GetComponent<Renderer>();
                if (renderer != null)
                    renderer.material.color = new Color(0.5f, 0f, 0.5f);
            }

            resolvido = true;
            FecharPainel();

            Debug.Log("🔓 Puzzle resolvido. Agora é uma fonte de água!");
        }
        else
        {
            messageText.text = "Senha Incorreta!";
            messageText.color = Color.red;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (!interagindo && pressEIndicator != null)
                pressEIndicator.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (pressEIndicator != null)
                pressEIndicator.SetActive(false);

            if (interagindo)
                FecharPainel();
        }
    }
}
