using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class CutsceneInMenuManager : MonoBehaviour
{
    [Header("Referências da Cutscene")]
    public VideoPlayer videoPlayer;
    public GameObject cutsceneContainer;
    public GameObject menuContainer;

    [Header("Sistema de Diálogos/Imagens")]
    public List<Dialogo> dialogos;
    public Image imagemDialogo;
    public Text textoDialogo;
    public GameObject indicadorClique;

    [Header("UI Elements")]
    public Button skipButton;
    public Button startButton;
    public Button settingsButton;
    public Button quitButton;

    [Header("Áudio")]
    public AudioSource menuMusic;
    public AudioSource cutsceneAudio;
    public AudioSource efeitosSonoros;

    [Header("Configurações")]
    public string gameSceneName = "Game";
    public bool debugMode = true;
    public float tempoEntreDialogos = 0.1f;

    private bool cutscenePlaying = false;
    private bool cutsceneFinished = false;
    private int dialogoAtualIndex = -1;
    private bool aguardandoClique = false;
    private HashSet<int> audiosTocados = new HashSet<int>();

    [System.Serializable]
    public class Dialogo
    {
        public Sprite imagem;
        public string texto;
        public AudioClip audio;
        public bool tocarUmaVez = true;
    }

    void Start()
    {
        DebugSaveStatus();
        InitializeSystem();
        SetupButtons();
        SetupAudio();
    }

    void Update()
    {
        // Detecta clique do mouse para avançar diálogos
        if (cutscenePlaying && aguardandoClique && Input.GetMouseButtonDown(0))
        {
            ProximoDialogo();
        }
    }

    void InitializeSystem()
    {
        GameDataJSON saveData = SaveSystem.LoadGame();

#if UNITY_EDITOR
        if (debugMode)
        {
            Debug.Log("🔧 Modo Debug Ativo - Mostrando cutscene");
            ShowCutscene();
            return;
        }
#endif

        if (saveData == null)
        {
            Debug.Log("📝 Primeira execução - Mostrando cutscene");
            ShowCutscene();
        }
        else if (!saveData.cutsceneVista)
        {
            Debug.Log("🎬 Cutscene não vista - Mostrando cutscene");
            ShowCutscene();
        }
        else
        {
            Debug.Log("🏠 Cutscene já vista - Indo direto para menu");
            ShowMenu();
        }
    }

    void DebugSaveStatus()
    {
        GameDataJSON saveData = SaveSystem.LoadGame();
        if (saveData != null)
        {
            Debug.Log($"💾 Status do Save: cutsceneVista = {saveData.cutsceneVista}");
            Debug.Log($"📁 Caminho do save: {Application.persistentDataPath}");
        }
        else
        {
            Debug.Log("❌ Nenhum save encontrado");
        }
    }

    void SetupButtons()
    {
        if (skipButton != null)
            skipButton.onClick.AddListener(SkipCutscene);

        if (startButton != null)
            startButton.onClick.AddListener(StartGame);

        if (settingsButton != null)
            settingsButton.onClick.AddListener(OpenSettings);

        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);
    }

    void SetupAudio()
    {
        if (menuMusic != null)
            menuMusic.Stop();

        if (videoPlayer != null)
        {
            videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
            if (cutsceneAudio != null)
            {
                videoPlayer.SetTargetAudioSource(0, cutsceneAudio);
            }
        }
    }

    void ShowCutscene()
    {
        cutscenePlaying = true;
        cutsceneFinished = false;
        dialogoAtualIndex = -1;
        audiosTocados.Clear();

        if (cutsceneContainer != null)
        {
            cutsceneContainer.SetActive(true);
            Debug.Log("🎬 Container da cutscene ativado");
        }

        if (menuContainer != null)
        {
            menuContainer.SetActive(false);
            Debug.Log("🏠 Container do menu desativado");
        }

        // Decide se usa sistema de vídeo ou diálogos
        if (videoPlayer != null && dialogos.Count == 0)
        {
            // Modo vídeo tradicional
            videoPlayer.loopPointReached += OnCutsceneEnd;
            videoPlayer.Prepare();
            StartCoroutine(PlayVideoWhenReady());
        }
        else if (dialogos.Count > 0)
        {
            // Modo diálogos com imagens
            StartCoroutine(IniciarDialogos());
        }
        else
        {
            Debug.LogError("❌ Nenhum conteúdo de cutscene configurado!");
            OnCutsceneEnd(null);
        }
    }

    private IEnumerator IniciarDialogos()
    {
        Debug.Log($"🎭 Iniciando sistema de diálogos com {dialogos.Count} diálogos");

        // Pequeno delay antes do primeiro diálogo
        yield return new WaitForSeconds(0.5f);

        ProximoDialogo();
    }

    void ProximoDialogo()
    {
        dialogoAtualIndex++;

        // Verifica se acabaram os diálogos
        if (dialogoAtualIndex >= dialogos.Count)
        {
            FinishCutscene();
            return;
        }

        Dialogo dialogoAtual = dialogos[dialogoAtualIndex];

        // Atualiza UI
        if (imagemDialogo != null)
        {
            imagemDialogo.sprite = dialogoAtual.imagem;
            imagemDialogo.gameObject.SetActive(dialogoAtual.imagem != null);
        }

        if (textoDialogo != null)
        {
            textoDialogo.text = dialogoAtual.texto;
            textoDialogo.gameObject.SetActive(!string.IsNullOrEmpty(dialogoAtual.texto));
        }

        // Toca áudio (se configurado para tocar uma vez)
        if (dialogoAtual.audio != null && efeitosSonoros != null)
        {
            bool deveTocar = true;

            if (dialogoAtual.tocarUmaVez)
            {
                int audioID = dialogoAtual.audio.GetInstanceID();
                if (audiosTocados.Contains(audioID))
                {
                    deveTocar = false;
                }
                else
                {
                    audiosTocados.Add(audioID);
                }
            }

            if (deveTocar)
            {
                efeitosSonoros.PlayOneShot(dialogoAtual.audio);
                Debug.Log($"🔊 Tocando áudio: {dialogoAtual.audio.name} (Uma vez: {dialogoAtual.tocarUmaVez})");
            }
        }

        // Mostra indicador de clique
        if (indicadorClique != null)
        {
            indicadorClique.SetActive(true);
            StartCoroutine(PiscarIndicador());
        }

        aguardandoClique = true;
        Debug.Log($"💬 Diálogo {dialogoAtualIndex + 1}/{dialogos.Count} - Aguardando clique");
    }

    private IEnumerator PiscarIndicador()
    {
        while (aguardandoClique && indicadorClique != null)
        {
            indicadorClique.SetActive(!indicadorClique.activeSelf);
            yield return new WaitForSeconds(0.5f);
        }

        if (indicadorClique != null)
            indicadorClique.SetActive(false);
    }

    private IEnumerator PlayVideoWhenReady()
    {
        Debug.Log("🔄 Preparando vídeo...");

        while (!videoPlayer.isPrepared)
        {
            yield return null;
        }

        Debug.Log("✅ Vídeo preparado, iniciando...");
        videoPlayer.Play();

        if (cutsceneAudio != null)
            cutsceneAudio.Play();
    }

    void ShowMenu()
    {
        cutscenePlaying = false;
        cutsceneFinished = true;
        aguardandoClique = false;

        if (menuContainer != null)
        {
            menuContainer.SetActive(true);
            Debug.Log("🏠 Menu ativado");
        }

        if (cutsceneContainer != null)
        {
            cutsceneContainer.SetActive(false);
            Debug.Log("🎬 Cutscene desativada");
        }

        if (menuMusic != null && !menuMusic.isPlaying)
        {
            menuMusic.Play();
            Debug.Log("🎵 Música do menu iniciada");
        }

        if (cutsceneAudio != null)
            cutsceneAudio.Stop();

        if (efeitosSonoros != null)
            efeitosSonoros.Stop();
    }

    void OnCutsceneEnd(VideoPlayer vp)
    {
        Debug.Log("✅ Cutscene finalizada naturalmente");
        FinishCutscene();
    }

    void SkipCutscene()
    {
        Debug.Log("⏭ Cutscene pulada pelo usuário");
        FinishCutscene();
    }

    void FinishCutscene()
    {
        if (cutsceneFinished) return;

        cutsceneFinished = true;
        aguardandoClique = false;

        if (videoPlayer != null && videoPlayer.isPlaying)
        {
            videoPlayer.Stop();
            Debug.Log("⏹ Vídeo parado");
        }

        MarkCutsceneAsWatched();
        ShowMenu();
    }

    void MarkCutsceneAsWatched()
    {
        try
        {
            GameDataJSON saveData = SaveSystem.LoadGame();

            if (saveData == null)
            {
                saveData = new GameDataJSON();
                Debug.Log("📝 Criando novo save data");
            }

            saveData.cutsceneVista = true;
            SaveSystem.SaveGame(saveData);
            Debug.Log("✅ Cutscene marcada como assistida no save");

            GameDataJSON verifiedData = SaveSystem.LoadGame();
            if (verifiedData != null && verifiedData.cutsceneVista)
            {
                Debug.Log("✅ Save verificado com sucesso!");
            }
            else
            {
                Debug.LogError("❌ Falha na verificação do save!");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"💥 Erro ao marcar cutscene como vista: {e.Message}");
        }
    }

    void StartGame()
    {
        Debug.Log("🎮 Iniciando jogo...");
        SceneManager.LoadScene(gameSceneName);
    }

    void OpenSettings()
    {
        Debug.Log("⚙️ Abrindo configurações");
        // Implementar abertura de menu de configurações
    }

    void QuitGame()
    {
        Debug.Log("🚪 Saindo do jogo");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}