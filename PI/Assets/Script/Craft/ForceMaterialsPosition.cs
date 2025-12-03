using UnityEngine;

public class ForceMaterialsPosition : MonoBehaviour
{
    void Start()
    {
        ForceChildrenPosition();
    }

    [ContextMenu("FORÇAR POSIÇÃO DOS FILHOS")]
    public void ForceChildrenPosition()
    {
        Debug.Log("🔄 FORÇANDO POSIÇÃO DOS MATERIAIS...");

        int childIndex = 0;
        foreach (Transform child in transform)
        {
            RectTransform rt = child.GetComponent<RectTransform>();
            if (rt != null)
            {
                // ✅ POSIÇÃO ABSOLUTA
                rt.anchorMin = new Vector2(0f, 1f);
                rt.anchorMax = new Vector2(0f, 1f);
                rt.pivot = new Vector2(0f, 1f);
                rt.anchoredPosition = new Vector2(childIndex * 200f, 0f);
                rt.sizeDelta = new Vector2(120, 40);

                Debug.Log($"✅ Material {childIndex}: Posição = {rt.anchoredPosition}");
            }
            childIndex++;
        }
    }
}