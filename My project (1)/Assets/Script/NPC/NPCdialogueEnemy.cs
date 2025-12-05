using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NPCDialogueEnemy : MonoBehaviour
{
    [Header("Identificador único do NPC")]
    public string idUnicoNPC = "NPC_Default";

    [Header("Destruir permanentemente após diálogo?")]
    public bool destruirDepoisDoDialogo = false;

    [Header("Diálogo")]
    public string[] dialogueNpc;
    public int dialogueIndex;
    public GameObject dialoguePanel;
    public Text dialogueText;
    public Text nameNpc;
    public Image imageNpc;
    public Sprite spriteNpc;

    public GameObject botaoUI;
    public Vector3 offsetUI = new Vector3(0, 1.5f, 0);

    [Header("Velocidade do diálogo (segundos por letra)")]
    [Range(0.01f, 0.5f)] public float dialogueSpeed = 0.1f;

    [Header("Animação da Boca")]
    public Sprite[] spritesBoca;           // Array com sprites da boca (0=fechada, 1=aberta, etc)
    public float velocidadeAnimacao = 0.1f; // Velocidade entre frames

    [Header("Configuração de Áudio")]
    public AudioClip somLetra;
    public AudioSource audioSourceDialogo;
    [Range(0, 1)] public float volumeSomLetra = 0.3f;
    public float pitchMinimo = 0.8f;
    public float pitchMaximo = 1.2f;

    [Header("Dar Machado ao Jogador")]
    public bool darMachado = false;
    public int indiceDarMachado = -1;

    [Header("Cena para carregar após o diálogo")]
    public string nomeCenaParaTrocar;

    private bool playerInRange = false;
    private bool startDialogue = false;
    private int frameAtual = 0;
    private Coroutine corrotinaAnimacao;

    void Start()
    {
        if (destruirDepoisDoDialogo && PlayerPrefs.GetInt(idUnicoNPC, 0) == 1)
        {
            Destroy(gameObject); // Já foi falado → destruir
            return;
        }

        if (string.IsNullOrEmpty(idUnicoNPC) && destruirDepoisDoDialogo)
        {
            Debug.LogWarning("idUnicoNPC está vazio em " + gameObject.name);
        }

        dialoguePanel.SetActive(false);
        if (botaoUI != null)
            botaoUI.SetActive(false);

        // Configurar AudioSource se não foi atribuído
        if (audioSourceDialogo == null)
            audioSourceDialogo = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (botaoUI != null && botaoUI.activeSelf)
        {
            botaoUI.transform.position = transform.position + offsetUI;
        }

        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (!startDialogue)
            {
                FindFirstObjectByType<CharacterController2D>().SetSpeed(0f);
                StartDialogue();
            }
            else if (dialogueText.text == dialogueNpc[dialogueIndex])
            {
                nextDialogue();
            }
        }
    }

    void nextDialogue()
    {
        dialogueIndex++;

        if (darMachado && dialogueIndex == indiceDarMachado)
        {
            FindFirstObjectByType<PlayerMachado>()?.DesbloquearMachado();
        }

        if (dialogueIndex < dialogueNpc.Length)
        {
            StartCoroutine(ShowDialogue());
        }
        else
        {
            dialoguePanel.SetActive(false);
            startDialogue = false;
            dialogueIndex = 0;

            // Para animação da boca
            if (corrotinaAnimacao != null)
            {
                StopCoroutine(corrotinaAnimacao);
                corrotinaAnimacao = null;
            }

            // Volta para sprite normal
            if (spriteNpc != null) imageNpc.sprite = spriteNpc;

            // Salva destruição permanente se ativado
            if (destruirDepoisDoDialogo)
            {
                PlayerPrefs.SetInt(idUnicoNPC, 1);
                PlayerPrefs.Save();
            }

            if (!string.IsNullOrEmpty(nomeCenaParaTrocar))
            {
                Destroy(gameObject); // Remove o NPC
                SceneManager.LoadScene(nomeCenaParaTrocar);
            }
        }
    }

    private void StartDialogue()
    {
        nameNpc.text = "";
        imageNpc.sprite = spriteNpc;
        startDialogue = true;
        dialogueIndex = 0;
        dialoguePanel.SetActive(true);
        StartCoroutine(ShowDialogue());

        if (botaoUI != null)
            botaoUI.SetActive(false);

        if (darMachado && dialogueIndex == indiceDarMachado)
        {
            FindFirstObjectByType<PlayerMachado>()?.DesbloquearMachado();
        }
    }

    IEnumerator ShowDialogue()
    {
        // Inicia animação da boca
        if (spritesBoca != null && spritesBoca.Length > 1)
        {
            if (corrotinaAnimacao != null) StopCoroutine(corrotinaAnimacao);
            corrotinaAnimacao = StartCoroutine(AnimacaoBoca());
        }

        dialogueText.text = "";
        foreach (char letter in dialogueNpc[dialogueIndex])
        {
            dialogueText.text += letter;

            // Tocar som para cada letra (apenas para caracteres visíveis)
            if (somLetra != null && audioSourceDialogo != null && !char.IsWhiteSpace(letter))
            {
                TocarSomLetra();
            }

            yield return new WaitForSeconds(dialogueSpeed);
        }

        // Para animação da boca quando termina de digitar
        if (corrotinaAnimacao != null)
        {
            StopCoroutine(corrotinaAnimacao);
            corrotinaAnimacao = null;
        }

        // Volta para sprite normal
        if (spriteNpc != null) imageNpc.sprite = spriteNpc;
    }

    IEnumerator AnimacaoBoca()
    {
        while (true)
        {
            // Alterna entre os frames do array
            frameAtual = (frameAtual + 1) % spritesBoca.Length;
            imageNpc.sprite = spritesBoca[frameAtual];
            yield return new WaitForSeconds(velocidadeAnimacao);
        }
    }

    void TocarSomLetra()
    {
        // Variação de pitch para deixar mais natural
        audioSourceDialogo.pitch = Random.Range(pitchMinimo, pitchMaximo);
        audioSourceDialogo.PlayOneShot(somLetra, volumeSomLetra);
    }

    // ⭐⭐ MÉTODO PARA BOTÃO MOBILE (OnClick direto no NPC) ⭐⭐
    public void BotaoMobileInteragir()
    {
        if (playerInRange)
        {
            if (!startDialogue)
            {
                FindFirstObjectByType<CharacterController2D>().SetSpeed(0f);
                StartDialogue();
            }
            else if (dialogueText.text == dialogueNpc[dialogueIndex])
            {
                nextDialogue();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
            if (!startDialogue && botaoUI != null)
                botaoUI.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
            if (botaoUI != null)
                botaoUI.SetActive(false);

            if (startDialogue)
            {
                dialoguePanel.SetActive(false);
                startDialogue = false;
                dialogueIndex = 0;

                // Para animação da boca
                if (corrotinaAnimacao != null)
                {
                    StopCoroutine(corrotinaAnimacao);
                    corrotinaAnimacao = null;
                }

                // Volta para sprite normal
                if (spriteNpc != null) imageNpc.sprite = spriteNpc;

                FindFirstObjectByType<CharacterController2D>().SetSpeed(5f);
            }
        }
    }
}