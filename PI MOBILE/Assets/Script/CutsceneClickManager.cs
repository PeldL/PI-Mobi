using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class CutsceneClickManager : MonoBehaviour
{
    [Header("Imagem de Fundo Fixa")]
    public Sprite fixedBackground;
    public Image backgroundImage;

    [Header("Imagens da Cutscene")]
    public Sprite[] cutsceneImages;
    [Tooltip("Índices dos diálogos em que cada imagem de cutscene será trocada. Ex: [0, 3, 5] troca a imagem nos diálogos 0, 3 e 5.")]
    public int[] backgroundChangeIndices;
    public Image cutsceneImage;

    [Header("Diálogos da Cutscene")]
    [TextArea(2, 5)]
    public string[] dialogues;
    public TextMeshProUGUI dialogueText;

    [Header("Música da Cutscene")]
    public AudioClip cutsceneMusic;

    [Header("Configurações de Áudio")]
    public AudioClip typingSound; // Som de digitação
    public bool disableMenuMusic = true; // Opção para desativar música do menu
    public GameObject menuMusicObject; // Referência para o objeto da música do menu

    private int currentIndex = 0;
    private int currentCutsceneImage = -1;
    private AudioSource audioSource;
    private AudioSource typingAudioSource; // AudioSource separado para o som de digitação
    private GameDataJSON gameData;
    private bool animacaoFinalRodando = false;
    private bool isTyping = false;
    private Coroutine typingCoroutine;
    private AudioSource menuMusicSource;
    private float lastTypingSoundTime; // Controle de tempo entre sons

    void Start()
    {
        gameData = SaveSystem.LoadGame();
        if (gameData == null)
        {
            gameData = SaveSystem.CreateNewSave();
        }

        if (gameData.cutsceneVista)
        {
            Debug.Log("[Cutscene] Já foi vista, desativando.");
            gameObject.SetActive(false);
            return;
        }

        // Configurar AudioSources
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Criar AudioSource separado para digitação
        typingAudioSource = gameObject.AddComponent<AudioSource>();
        typingAudioSource.playOnAwake = false;
        typingAudioSource.loop = false;
        typingAudioSource.volume = 0.3f; // Volume mais baixo para não estourar

        // Desativar música do menu se configurado
        if (disableMenuMusic && menuMusicObject != null)
        {
            DisableMenuMusic();
        }

        if (backgroundImage != null && fixedBackground != null)
            backgroundImage.sprite = fixedBackground;

        if (cutsceneImage != null)
            cutsceneImage.gameObject.SetActive(false);

        if (cutsceneMusic != null)
        {
            audioSource.clip = cutsceneMusic;
            audioSource.loop = true;
            audioSource.Play();
        }
        Debug.Log("[Cutscene] Iniciando cutscene.");
        MostrarCutscene();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !animacaoFinalRodando)
        {
            if (isTyping)
            {
                // Se estiver digitando, pular para o texto completo
                SkipTyping();
            }
            else
            {
                Debug.Log($"[Cutscene] Mouse 1 pressionado. Avançando para diálogo {currentIndex + 1}.");
                AvancarCutscene();
            }
        }
    }

    void DisableMenuMusic()
    {
        if (menuMusicObject != null)
        {
            menuMusicSource = menuMusicObject.GetComponent<AudioSource>();
            if (menuMusicSource != null && menuMusicSource.isPlaying)
            {
                menuMusicSource.Stop();
                Debug.Log("[Cutscene] Música do menu desativada.");
            }
        }
        else
        {
            // Fallback: procurar por AudioSources que possam estar tocando música do menu
            AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();
            foreach (AudioSource source in allAudioSources)
            {
                if (source != audioSource && source != typingAudioSource && source.isPlaying)
                {
                    menuMusicSource = source;
                    menuMusicSource.Stop();
                    Debug.Log("[Cutscene] Música do menu desativada (fallback).");
                    break;
                }
            }
        }
    }

    void EnableMenuMusic()
    {
        // Reativar música do menu se necessário
        if (menuMusicSource != null)
        {
            menuMusicSource.Play();
            Debug.Log("[Cutscene] Música do menu reativada.");
        }
    }

    void MostrarCutscene()
    {
        int foundIndex = System.Array.IndexOf(backgroundChangeIndices, currentIndex);
        if (foundIndex != -1 && foundIndex < cutsceneImages.Length)
        {
            cutsceneImage.sprite = cutsceneImages[foundIndex];
            cutsceneImage.gameObject.SetActive(true);
            cutsceneImage.color = Color.white;
            currentCutsceneImage = foundIndex;
            Debug.Log($"[Cutscene] Trocando imagem da cutscene para índice {foundIndex} no diálogo {currentIndex}.");
        }
        else
        {
            cutsceneImage.gameObject.SetActive(false);
            Debug.Log($"[Cutscene] Nenhuma imagem para este diálogo ({currentIndex}), cutsceneImage desativada.");
        }

        if (currentIndex < dialogues.Length)
        {
            // Iniciar efeito de digitação
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
                StopTypingSound(); // Parar som de digitação anterior
            }

            typingCoroutine = StartCoroutine(TypeText(dialogues[currentIndex]));
            Debug.Log($"[Cutscene] Iniciando digitação do diálogo {currentIndex}");
        }

        // Se for o último diálogo (Element 4) E a imagem atual for a última (Element 3)
        if (currentIndex == dialogues.Length - 1 && currentCutsceneImage == 3)
        {
            StartCoroutine(AnimacaoFinalCutscene());
        }
    }

    IEnumerator TypeText(string text)
    {
        isTyping = true;
        dialogueText.text = "";
        lastTypingSoundTime = 0f;

        // Tocar som de digitação se configurado
        bool playSound = typingSound != null;
        int characterCount = 0;

        foreach (char letter in text.ToCharArray())
        {
            dialogueText.text += letter;
            characterCount++;

            // Tocar som de digitação apenas a cada 2-3 caracteres e com intervalo mínimo
            if (playSound && characterCount % 2 == 0 && Time.time - lastTypingSoundTime > 0.08f)
            {
                PlayTypingSound();
                lastTypingSoundTime = Time.time;
            }

            // Velocidade da digitação
            yield return new WaitForSeconds(0.02f);
        }

        // Parar som de digitação quando terminar
        StopTypingSound();
        isTyping = false;
    }

    void PlayTypingSound()
    {
        if (typingAudioSource != null && typingSound != null)
        {
            // Parar qualquer som anterior antes de tocar o próximo
            if (typingAudioSource.isPlaying)
            {
                typingAudioSource.Stop();
            }

            // Pequena variação de pitch para soar mais natural
            typingAudioSource.pitch = Random.Range(0.9f, 1.1f);
            typingAudioSource.PlayOneShot(typingSound);
        }
    }

    void StopTypingSound()
    {
        if (typingAudioSource != null && typingAudioSource.isPlaying)
        {
            typingAudioSource.Stop();
        }
    }

    void SkipTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            StopTypingSound(); // Parar som ao pular
        }

        dialogueText.text = dialogues[currentIndex];
        isTyping = false;
        Debug.Log($"[Cutscene] Digitação pulada - diálogo completo: {dialogues[currentIndex]}");
    }

    void AvancarCutscene()
    {
        // Parar som de digitação antes de avançar
        StopTypingSound();

        currentIndex++;
        if (currentIndex < dialogues.Length)
        {
            MostrarCutscene();
        }
        else
        {
            if (audioSource.isPlaying)
                audioSource.Stop();

            // Reativar música do menu se foi desativada
            if (disableMenuMusic)
            {
                EnableMenuMusic();
            }

            gameData.cutsceneVista = true;
            SaveSystem.SaveGame(gameData);

            Debug.Log("[Cutscene] Cutscene finalizada e progresso salvo.");
            gameObject.SetActive(false);
        }
    }

    IEnumerator AnimacaoFinalCutscene()
    {
        // Parar som de digitação durante animação final
        StopTypingSound();

        animacaoFinalRodando = true;
        Debug.Log("🎯 ANIMAÇÃO SMOOTH + PROPORCIONAL");

        float tempo = 0f;
        float duracao = 3f;

        Vector3 escalaInicial = cutsceneImage.rectTransform.localScale;

        // ⚠️ CORREÇÃO: Mantém a PROPORÇÃO da escala original, só aumenta
        float proporcaoX = escalaInicial.x / escalaInicial.y; // Calcula proporção atual
        Vector3 escalaFinal = new Vector3(25f, 25f / proporcaoX, escalaInicial.z); // Mantém proporção

        Debug.Log($"🔍 Escala inicial: {escalaInicial}");
        Debug.Log($"📐 Proporção X/Y: {proporcaoX}");
        Debug.Log($"🎯 Escala final: {escalaFinal}");

        // CORREÇÃO DO MATERIAL
        if (cutsceneImage.material != null)
        {
            cutsceneImage.material = null;
        }

        Sprite spriteOriginal = cutsceneImage.sprite;

        // ANIMAÇÃO SMOOTH com curva
        while (tempo < duracao)
        {
            tempo += Time.deltaTime;
            float t = tempo / duracao;

            // ⚠️ CURVA SMOOTH - lento no início, rápido no meio, lento no final
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            // ZOOM PROPORCIONAL
            cutsceneImage.rectTransform.localScale = Vector3.Lerp(escalaInicial, escalaFinal, smoothT);

            // ⚠️ TRANSição SUAVE para branco - só nos últimos 50%
            if (t > 0.5f)
            {
                float brancoT = (t - 0.5f) / 0.5f; // 0 a 1 nos últimos 50%
                cutsceneImage.color = Color.Lerp(Color.white, new Color(1.2f, 1.2f, 1.2f, 1f), brancoT);
            }

            yield return null;
        }

        // ⚠️ TROCA FINAL para sprite BRANCO (opcional)
        Texture2D textureBranca = new Texture2D(2, 2);
        for (int x = 0; x < 2; x++)
            for (int y = 0; y < 2; y++)
                textureBranca.SetPixel(x, y, Color.white);
        textureBranca.Apply();

        Sprite spriteBranco = Sprite.Create(textureBranca, new Rect(0, 0, 2, 2), new Vector2(0.5f, 0.5f));
        cutsceneImage.sprite = spriteBranco;
        cutsceneImage.color = Color.white;
        cutsceneImage.rectTransform.localScale = escalaFinal;

        Debug.Log("✅ ANIMAÇÃO SMOOTH CONCLUÍDA");

        yield return new WaitForSeconds(1.5f);

        animacaoFinalRodando = false;
        AvancarCutscene();
    }

    void OnDisable()
    {
        // Garantir que todos os sons parem quando o objeto for desativado
        StopTypingSound();
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
}