using UnityEngine;

public class CanvasDebugger : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F6))
        {
            DebugCanvasStatus();
        }
    }

    void DebugCanvasStatus()
    {
        GameObject canvas = GameObject.Find("CraftingCanvas");
        GameObject panel = GameObject.Find("CraftingPanel");

        Debug.Log("=== 🎨 DEBUG CANVAS ===");
        Debug.Log($"Canvas: {canvas != null} | Ativo: {canvas?.activeInHierarchy}");
        Debug.Log($"Panel: {panel != null} | Ativo: {panel?.activeInHierarchy}");

        if (panel != null)
        {
            Debug.Log($"Posição: {panel.transform.position}");
            Debug.Log($"Escala: {panel.transform.localScale}");
        }
    }
}