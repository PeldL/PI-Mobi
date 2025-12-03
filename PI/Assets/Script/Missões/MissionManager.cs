using UnityEngine;

public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance;

    public bool bossDerrotado = false;
    public bool segundaMissaoAtivada = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
