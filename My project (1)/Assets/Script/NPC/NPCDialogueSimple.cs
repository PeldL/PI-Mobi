using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class NPCDialogueSimple : MonoBehaviour
{
    [Header("Diálogos")]
    [TextArea] public string[] dialogoAntes;
    [TextArea] public string[] dialogoPosBoss;
    [TextArea] public string[] dialogoDepois;
    public bool missaoCompleta = false;

    [Header("UI")]
    public GameObject painelDialogo;
    public Text textoDialogo;
    public Text nomeNpc;
    public Image imagemNpc;
    public Sprite spriteNpc;
    public GameObject botaoE;
    public Vector3 offsetUI = new Vector3(0, 1.5f, 0);

    [Header("Animação da Boca")]
    public Sprite[] spritesBoca;           // Array com sprites da boca (0=fechada, 1=aberta, etc)
    public float velocidadeAnimacao = 0.1f; // Velocidade entre frames

    [Header("Configuração")]
    [Range(0.01f, 0.5f)] public float velocidadeLetra = 0.05f;
    public string nomeDoNpc = "Dona Zefa";

    [Header("Configuração de Áudio")]
    public AudioClip somLetra;
    public AudioSource audioSourceDialogo;
    [Range(0, 1)] public float volumeSomLetra = 0.3f;
    public float pitchMinimo = 0.8f;
    public float pitchMaximo = 1.2f;

    [Header("Recompensas")]
    public bool darMachado = false;

    private bool jogadorPerto = false;
    private bool emDialogo = false;
    private bool digitando = false;
    private int linhaAtual = 0;
    private string[] dialogoAtual;
    private int frameAtual = 0;
    private Coroutine corrotinaAnimacao;

    void Start()
    {
        painelDialogo.SetActive(false);
        nomeNpc.text = nomeDoNpc;
        imagemNpc.sprite = spriteNpc;
        if (botaoE != null) botaoE.SetActive(false);

        if (audioSourceDialogo == null)
            audioSourceDialogo = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (botaoE != null && botaoE.activeSelf)
            botaoE.transform.position = transform.position + offsetUI;

        if (jogadorPerto && Input.GetKeyDown(KeyCode.E))
        {
            if (!emDialogo)
                IniciarDialogo();
            else if (!digitando && textoDialogo.text == dialogoAtual[linhaAtual])
                ProximaLinha();
        }
    }

    void IniciarDialogo()
    {
        bool jaTemRegador = FindFirstObjectByType<PlayerRegador>()?.regadorDesbloqueado ?? false;

        if (jaTemRegador)
            dialogoAtual = dialogoDepois;
        else if (MissionManager.Instance != null && MissionManager.Instance.bossDerrotado)
            dialogoAtual = dialogoPosBoss;
        else
            dialogoAtual = dialogoAntes;

        linhaAtual = 0;
        emDialogo = true;

        painelDialogo.SetActive(true);
        if (botaoE != null) botaoE.SetActive(false);

        FindFirstObjectByType<CharacterController2D>()?.SetSpeed(0f);
        StartCoroutine(DigitarTexto());
    }

    void ProximaLinha()
    {
        linhaAtual++;
        if (linhaAtual < dialogoAtual.Length)
        {
            StartCoroutine(DigitarTexto());
        }
        else
        {
            FinalizarDialogo();
        }
    }

    IEnumerator DigitarTexto()
    {
        digitando = true;

        // Inicia animação da boca
        if (spritesBoca != null && spritesBoca.Length > 1)
        {
            if (corrotinaAnimacao != null) StopCoroutine(corrotinaAnimacao);
            corrotinaAnimacao = StartCoroutine(AnimacaoBoca());
        }

        textoDialogo.text = "";
        foreach (char letra in dialogoAtual[linhaAtual])
        {
            textoDialogo.text += letra;

            if (somLetra != null && audioSourceDialogo != null && !char.IsWhiteSpace(letra))
            {
                TocarSomLetra();
            }

            yield return new WaitForSeconds(velocidadeLetra);
        }

        digitando = false;

        // Para animação da boca quando termina de digitar
        if (corrotinaAnimacao != null)
        {
            StopCoroutine(corrotinaAnimacao);
            corrotinaAnimacao = null;
        }

        // Volta para sprite normal
        if (spriteNpc != null) imagemNpc.sprite = spriteNpc;
    }

    IEnumerator AnimacaoBoca()
    {
        while (true)
        {
            // Alterna entre os frames do array
            frameAtual = (frameAtual + 1) % spritesBoca.Length;
            imagemNpc.sprite = spritesBoca[frameAtual];
            yield return new WaitForSeconds(velocidadeAnimacao);
        }
    }

    void TocarSomLetra()
    {
        audioSourceDialogo.pitch = Random.Range(pitchMinimo, pitchMaximo);
        audioSourceDialogo.PlayOneShot(somLetra, volumeSomLetra);
    }

    void FinalizarDialogo()
    {
        // Para animação da boca
        if (corrotinaAnimacao != null)
        {
            StopCoroutine(corrotinaAnimacao);
            corrotinaAnimacao = null;
        }

        // Volta para sprite normal
        if (spriteNpc != null) imagemNpc.sprite = spriteNpc;

        painelDialogo.SetActive(false);
        emDialogo = false;
        digitando = false;
        linhaAtual = 0;

        FindFirstObjectByType<CharacterController2D>()?.SetSpeed(5f);
        if (botaoE != null && jogadorPerto) botaoE.SetActive(true);
    }

    void DarRecompensas()
    {
        var player = FindFirstObjectByType<PlayerRegador>();
        if (player == null)
        {
            Debug.LogWarning("PlayerRegador não encontrado!");
            return;
        }

        if (darMachado)
        {
            Debug.Log("Dando machado");
            player.DesbloquearMachado();
        }
    }

    public void SetRecompensas(bool machado, bool regador)
    {
        darMachado = machado;
    }

    // ⭐⭐ MÉTODO PARA BOTÃO MOBILE (OnClick direto no NPC) ⭐⭐
    public void BotaoMobileInteragir()
    {
        if (jogadorPerto)
        {
            if (!emDialogo)
            {
                IniciarDialogo();
            }
            else if (!digitando && textoDialogo.text == dialogoAtual[linhaAtual])
            {
                ProximaLinha();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            jogadorPerto = true;
            if (!emDialogo && botaoE != null)
                botaoE.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            jogadorPerto = false;
            if (botaoE != null)
                botaoE.SetActive(false);
            if (emDialogo)
                FinalizarDialogo();
        }
    }
}