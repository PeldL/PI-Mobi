using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MaterialSlotUI : MonoBehaviour
{
    [Header("UI References")]
    public Image materialIcon;
    public TextMeshProUGUI materialName;
    public TextMeshProUGUI materialAmount;

    public void Initialize(RecipeMaterial material, int positionIndex, int totalMaterials)
    {
        // ✅ CONFIGURAÇÃO RÁPIDA
        SetupPosition(positionIndex);

        // ✅ CONTEÚDO
        if (materialIcon != null)
            materialIcon.sprite = material.item.icon;

        if (materialName != null)
            materialName.text = material.item.itemName;

        if (materialAmount != null)
        {
            int playerAmount = InventorySystem.Instance.GetItemCount(material.item);
            materialAmount.text = $"{playerAmount}/{material.amount}";
            materialAmount.color = playerAmount >= material.amount ? Color.green : Color.red;
        }

        // ✅ DEBUG FINAL
        Debug.Log($"🎯 MATERIAL {material.item.itemName} - Posição Final: {GetComponent<RectTransform>().anchoredPosition}");
    }

    void SetupPosition(int index)
    {
        RectTransform rt = GetComponent<RectTransform>();
        if (rt != null)
        {
            // ✅ TAMANHO
            rt.sizeDelta = new Vector2(120, 40);

            // ✅ ANCHORS - TOP LEFT
            rt.anchorMin = new Vector2(0f, 1f);
            rt.anchorMax = new Vector2(0f, 1f);
            rt.pivot = new Vector2(0f, 1f);

            // ✅ POSIÇÃO - 200 PIXELS DE ESPAÇO (MAIOR)
            float xPosition = index * 200f;
            float yPosition = 0f;

            rt.anchoredPosition = new Vector2(xPosition, yPosition);

            Debug.Log($"📍 SetupPosition: Index={index}, X={xPosition}, Anchors={rt.anchorMin}");
        }
    }

    void Start()
    {
        // ✅ VERIFICA SE A POSIÇÃO FOI MANTIDA
        Debug.Log($"🔍 Start - Posição: {GetComponent<RectTransform>().anchoredPosition}");
    }
}