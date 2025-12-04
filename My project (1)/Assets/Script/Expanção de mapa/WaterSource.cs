using UnityEngine;

public class WaterSource : MonoBehaviour
{
    public GameObject pressEIndicator;
    private bool playerInRange = false;

    void Start()
    {
        if (pressEIndicator != null)
            pressEIndicator.SetActive(false);
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            PlayerRegador player = FindFirstObjectByType<PlayerRegador>();
            if (player != null && player.regadorDesbloqueado)
            {
                player.EncherRegador();
            }
        }

        if (pressEIndicator != null)
            pressEIndicator.SetActive(playerInRange);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
