using UnityEngine.UI;

using TMPro;
using System.Collections.Generic;
using UnityEngine;

public class CardPanelUI : MonoBehaviour
{
    public static CardPanelUI Instance;

    [Header("UI")]
    public Image cardImage;         
    public TMP_Text cardNameText;    
    public Button leftButton;
    public Button rightButton;
    public Button closeButton;

    [Header("Cartas")]
    public List<Sprite> cardSprites;     
    public List<string> cardNames;       

    private int currentIndex = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        closeButton.onClick.AddListener(ClosePanel);
        leftButton.onClick.AddListener(ShowPreviousCard);
        rightButton.onClick.AddListener(ShowNextCard);
    }

    public void ShowFirstCard()
    {
        currentIndex = 0;
        UpdateUI();
    }

    public void ShowNextCard()
    {
        if (currentIndex < cardSprites.Count - 1)
        {
            currentIndex++;
            UpdateUI();
        }
    }

    public void ShowPreviousCard()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        if (cardSprites.Count == 0) return;

        cardImage.sprite = cardSprites[currentIndex];

        if (cardNames.Count == cardSprites.Count)
        {
            cardNameText.text = cardNames[currentIndex];
        }
        else
        {
            cardNameText.text = "Carta " + (currentIndex + 1);
        }
    }

    private void ClosePanel()
    {
        gameObject.SetActive(false);
    }
}
