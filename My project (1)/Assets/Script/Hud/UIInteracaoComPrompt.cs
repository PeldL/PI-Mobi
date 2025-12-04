using UnityEngine;

public class UIInteracaoComPrompt : MonoBehaviour
{
    [Header("Referências")]
    public GameObject promptE;    
    public GameObject painelUI;    
    public KeyCode teclaInteracao = KeyCode.E;

    private bool jogadorPerto = false;

    void Start()
    {
        if (promptE != null)
            promptE.SetActive(false);

        if (painelUI != null)
            painelUI.SetActive(false);
    }

    void Update()
    {
        if (jogadorPerto && Input.GetKeyDown(teclaInteracao))
        {
            Debug.Log("E pressionado. Abrindo painel e escondendo prompt.");

            if (painelUI != null)
                painelUI.SetActive(true);

            if (promptE != null)
                promptE.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jogadorPerto = true;

            if (promptE != null)
                promptE.SetActive(true);

            Debug.Log("Jogador entrou na área.");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jogadorPerto = false;

            if (promptE != null)
                promptE.SetActive(false);

            if (painelUI != null)
                painelUI.SetActive(false);

            Debug.Log("Jogador saiu da área.");
        }
    }
}
