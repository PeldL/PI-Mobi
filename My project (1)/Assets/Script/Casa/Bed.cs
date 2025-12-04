using UnityEngine;

public class Bed : MonoBehaviour
{
    public float raioDeteccao = 1.5f;
    public GameObject botaoUI;
    public SleepUI sleepUI;

    private bool jogadorPerto = false;

    void Start()
    {
        if (botaoUI != null)
            botaoUI.SetActive(false);
    }

    void Update()
    {
        // Detecta se jogador está perto
        Collider2D jogador = Physics2D.OverlapCircle(transform.position, raioDeteccao, LayerMask.GetMask("Player"));

        if (jogador != null)
        {
            if (!jogadorPerto)
            {
                jogadorPerto = true;
                botaoUI.SetActive(true);
            }

            // Pressionou E para dormir
            if (Input.GetKeyDown(KeyCode.E))
            {
                sleepUI.AbrirMenu();
                botaoUI.SetActive(false);
            }
        }
        else
        {
            if (jogadorPerto)
            {
                jogadorPerto = false;
                botaoUI.SetActive(false);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, raioDeteccao);
    }
}