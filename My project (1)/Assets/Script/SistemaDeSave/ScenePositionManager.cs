using UnityEngine;

public class ScenePositionManager : MonoBehaviour
{
    public static ScenePositionManager Instance;

    public string originScene;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persiste entre cenas
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveOriginScene(string sceneName)
    {
        originScene = sceneName;
    }
}
