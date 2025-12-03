using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;

public class Menu : MonoBehaviour
{
    [Header("Botões Principais")]
    public Button newGameButton;
    public Button continueButton;
    public Button controlsButton;
    public Button quitButton;
    public Button deleteSaveButton; // Botão extra

    [Header("Painéis")]
    public GameObject controlsPanel;
    public Button controlsBackButton;

    [Header("Navegação")]
    public bool enableWASD = true;
    public float navigationCooldown = 0.2f;

    [Header("Cores de Seleção")]
    public Color normalColor = Color.white;
    public Color selectedColor = Color.yellow;
    public Color disabledColor = Color.gray;

    private List<Button> currentButtons = new List<Button>();
    private List<Button> mainMenuButtons = new List<Button>();
    private List<Button> controlsMenuButtons = new List<Button>();

    private int currentButtonIndex = 0;
    private float lastNavigationTime = 0f;
    private bool inControlsMenu = false;

    // Para guardar as cores originais dos textos
    private Dictionary<Button, Color> originalTextColors = new Dictionary<Button, Color>();

    private void Start()
    {
        InitializeMenus();
        UpdateContinueButton();
        ShowMainMenu();
    }

    private void InitializeMenus()
    {
        // Configura botões do menu principal
        if (newGameButton != null)
        {
            mainMenuButtons.Add(newGameButton);
            StoreOriginalColor(newGameButton);
            newGameButton.onClick.AddListener(NewGame);
        }
        if (continueButton != null)
        {
            mainMenuButtons.Add(continueButton);
            StoreOriginalColor(continueButton);
            continueButton.onClick.AddListener(ContinueGame);
        }
        if (controlsButton != null)
        {
            mainMenuButtons.Add(controlsButton);
            StoreOriginalColor(controlsButton);
            controlsButton.onClick.AddListener(OpenControls);
        }
        if (quitButton != null)
        {
            mainMenuButtons.Add(quitButton);
            StoreOriginalColor(quitButton);
            quitButton.onClick.AddListener(QuitGame);
        }
        if (deleteSaveButton != null)
        {
            mainMenuButtons.Add(deleteSaveButton);
            StoreOriginalColor(deleteSaveButton);
            deleteSaveButton.onClick.AddListener(DeleteAllSave);
        }

        // Configura botões do menu de controles
        if (controlsBackButton != null)
        {
            controlsMenuButtons.Add(controlsBackButton);
            StoreOriginalColor(controlsBackButton);
            controlsBackButton.onClick.AddListener(CloseControls);
        }

        // Configura eventos de mouse
        SetupMouseEvents();
    }

    private void StoreOriginalColor(Button button)
    {
        // Guarda a cor original do texto do botão
        TextMeshProUGUI text = button.GetComponentInChildren<TextMeshProUGUI>();
        if (text != null)
        {
            originalTextColors[button] = text.color;
        }
    }

    private void SetupMouseEvents()
    {
        // Adiciona eventos de mouse para todos os botões
        foreach (Button button in mainMenuButtons)
        {
            if (button != null)
            {
                ButtonMouseHandler mouseHandler = button.gameObject.AddComponent<ButtonMouseHandler>();
                mouseHandler.Initialize(this, button);
            }
        }

        foreach (Button button in controlsMenuButtons)
        {
            if (button != null)
            {
                ButtonMouseHandler mouseHandler = button.gameObject.AddComponent<ButtonMouseHandler>();
                mouseHandler.Initialize(this, button);
            }
        }
    }

    private void Update()
    {
        if (enableWASD && currentButtons.Count > 0)
        {
            HandleNavigation();
            HandleSelection();
        }

        // Voltar com ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (inControlsMenu)
            {
                CloseControls();
            }
        }
    }

    private void HandleNavigation()
    {
        // Verifica cooldown para evitar navegação muito rápida
        if (Time.time - lastNavigationTime < navigationCooldown)
            return;

        int previousIndex = currentButtonIndex;

        // Navegação para baixo (S, Down Arrow)
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentButtonIndex = GetNextValidButtonIndex(currentButtonIndex + 1);
            lastNavigationTime = Time.time;
        }
        // Navegação para cima (W, Up Arrow)
        else if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentButtonIndex = GetNextValidButtonIndex(currentButtonIndex - 1);
            lastNavigationTime = Time.time;
        }

        // Se o índice mudou, seleciona o novo botão
        if (currentButtonIndex != previousIndex)
        {
            SelectCurrentButton();
        }
    }

    private int GetNextValidButtonIndex(int desiredIndex)
    {
        if (currentButtons.Count == 0) return 0;

        int newIndex = desiredIndex;

        // Faz loop
        if (newIndex >= currentButtons.Count)
            newIndex = 0;
        else if (newIndex < 0)
            newIndex = currentButtons.Count - 1;

        // Se o botão não for interativo, procura o próximo
        if (currentButtons[newIndex] != null && !currentButtons[newIndex].interactable)
        {
            return GetNextValidButtonIndex(newIndex + (desiredIndex > currentButtonIndex ? 1 : -1));
        }

        return newIndex;
    }

    private void HandleSelection()
    {
        // Seleciona com Enter, Space, ou E
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.E))
        {
            if (currentButtonIndex >= 0 && currentButtonIndex < currentButtons.Count)
            {
                Button currentButton = currentButtons[currentButtonIndex];
                if (currentButton != null && currentButton.interactable)
                {
                    currentButton.onClick.Invoke();
                }
            }
        }
    }

    private void ShowMainMenu()
    {
        inControlsMenu = false;
        currentButtons = new List<Button>(mainMenuButtons);

        // Esconde painel de controles
        if (controlsPanel != null)
            controlsPanel.SetActive(false);

        SelectFirstValidButton();
    }

    private void ShowControlsMenu()
    {
        inControlsMenu = true;
        currentButtons = new List<Button>(controlsMenuButtons);

        // Mostra painel de controles
        if (controlsPanel != null)
            controlsPanel.SetActive(true);

        SelectFirstValidButton();
    }

    private void SelectFirstValidButton()
    {
        for (int i = 0; i < currentButtons.Count; i++)
        {
            if (currentButtons[i] != null && currentButtons[i].interactable)
            {
                currentButtonIndex = i;
                SelectCurrentButton();
                return;
            }
        }

        // Se nenhum for interativo, seleciona o primeiro
        if (currentButtons.Count > 0)
        {
            currentButtonIndex = 0;
            SelectCurrentButton();
        }
    }

    private void SelectCurrentButton()
    {
        // Desseleciona todos os botões
        foreach (Button button in mainMenuButtons)
        {
            if (button != null) SetButtonSelected(button, false);
        }
        foreach (Button button in controlsMenuButtons)
        {
            if (button != null) SetButtonSelected(button, false);
        }

        // Seleciona o botão atual
        if (currentButtonIndex >= 0 && currentButtonIndex < currentButtons.Count)
        {
            Button currentButton = currentButtons[currentButtonIndex];
            if (currentButton != null)
            {
                currentButton.Select(); // Seleção padrão do Unity
                SetButtonSelected(currentButton, true);
            }
        }
    }

    private void SetButtonSelected(Button button, bool selected)
    {
        if (button == null) return;

        // Muda apenas a cor do texto - SEM MUDAR TAMANHO
        TextMeshProUGUI text = button.GetComponentInChildren<TextMeshProUGUI>();
        if (text != null)
        {
            if (!button.interactable)
            {
                text.color = disabledColor;
            }
            else if (selected)
            {
                text.color = selectedColor;
            }
            else
            {
                // Restaura a cor original
                if (originalTextColors.ContainsKey(button))
                {
                    text.color = originalTextColors[button];
                }
                else
                {
                    text.color = normalColor;
                }
            }
        }

        // REMOVIDO: efeito de escala para não mudar tamanho
        // O tamanho do botão permanece sempre normal
    }

    private void UpdateContinueButton()
    {
        if (continueButton != null)
        {
            continueButton.interactable = SaveSystem.HasSave();
        }
    }

    // Método para quando o mouse seleciona um botão
    public void OnButtonHover(Button hoveredButton)
    {
        for (int i = 0; i < currentButtons.Count; i++)
        {
            if (currentButtons[i] == hoveredButton)
            {
                currentButtonIndex = i;
                SelectCurrentButton();
                break;
            }
        }
    }

    // ========== MÉTODOS DOS BOTÕES ==========

    public void NewGame()
    {
        Debug.Log("🆕 Novo Jogo Iniciado");
        SaveSystem.DeleteSave();
        SceneManager.LoadScene("Game novo principal");
    }

    public void ContinueGame()
    {
        GameDataJSON data = SaveSystem.LoadGame();

        if (data != null)
        {
            if (!string.IsNullOrEmpty(data.sceneName))
                SceneManager.LoadScene(data.sceneName);
            else
                SceneManager.LoadScene("Game novo principal");

            if (GameData.Instance != null)
                GameData.Instance.coins = data.coins;

            Debug.Log("▶️ Jogo Continuado");
        }
        else
        {
            Debug.LogWarning("[Menu] Nenhum save encontrado para continuar.");
        }
    }

    public void OpenControls()
    {
        Debug.Log("🎮 Abrindo controles");
        ShowControlsMenu();
    }

    public void CloseControls()
    {
        Debug.Log("🔙 Fechando controles");
        ShowMainMenu();
    }

    public void DeleteAllSave()
    {
        SaveSystem.DeleteSave();
        Debug.Log("🗑️ Todos os saves foram apagados.");
        UpdateContinueButton();

        // Reseleciona o primeiro botão após deletar
        SelectFirstValidButton();
    }

    public void QuitGame()
    {
        Debug.Log("👋 Saindo do jogo...");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}

// Classe auxiliar para lidar com eventos de mouse
public class ButtonMouseHandler : MonoBehaviour
{
    private Menu menu;
    private Button button;

    public void Initialize(Menu menuRef, Button buttonRef)
    {
        menu = menuRef;
        button = buttonRef;
    }

    public void OnPointerEnter()
    {
        if (menu != null && button != null && button.interactable)
        {
            menu.OnButtonHover(button);
        }
    }
}