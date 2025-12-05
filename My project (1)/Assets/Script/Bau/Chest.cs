using UnityEngine;

public class Chest : MonoBehaviour
{
    [Header("Referências")]
    public GameObject promptE;
    public GameObject painelUI;
    public Animator animator;
    public KeyCode teclaInteracao = KeyCode.E;

    private bool jogadorPerto = false;
    private bool isOpen = false;

    void Start()
    {
        if (promptE != null)
            promptE.SetActive(false);

        if (painelUI != null)
            painelUI.SetActive(false);

        if (animator != null)
            animator.SetBool("isOpen", false); 
    }

    void Update()
    {
        if (jogadorPerto && Input.GetKeyDown(teclaInteracao))
        {
            isOpen = !isOpen; 

            if (animator != null)
                animator.SetBool("isOpen", isOpen);

            if (painelUI != null)
                painelUI.SetActive(isOpen);

            if (promptE != null && !isOpen)
                promptE.SetActive(true);

            Debug.Log("Baú agora está " + (isOpen ? "aberto" : "fechado"));
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jogadorPerto = true;

            if (!isOpen && promptE != null)
                promptE.SetActive(true);
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
        }
    }
}
