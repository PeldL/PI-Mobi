using UnityEngine;

public class FireDamage : MonoBehaviour
{
    [Header("Dano ao jogador")]
    [SerializeField] private int damageAmount = 1;

    [Header("Fogo no chão")]
    [SerializeField] private GameObject fireOnGroundPrefab;
    [SerializeField, Range(0f, 1f)] private float chanceToStay = 0.5f;
    [SerializeField] private float destroyDelayAfterSpawn = 0.05f;

    private Rigidbody2D rb;
    private Collider2D col;

    private bool triggered = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (triggered) return;

        if (collision.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null)
                playerHealth.TakeDamage(damageAmount);
        }

        if (collision.CompareTag("Ground"))
        {
            // Só com chance ele irá parar e instanciar fogo no chão
            if (Random.value <= chanceToStay && fireOnGroundPrefab != null)
            {
                triggered = true;

                if (rb != null)
                {
                    rb.linearVelocity = Vector2.zero;
                    rb.gravityScale = 0;
                    rb.simulated = false;
                }

                if (col != null)
                    col.enabled = false;

                Vector3 offset = new Vector3(
                    Random.Range(-0.2f, 0.2f),
                    Random.Range(-0.1f, 0.1f),
                    0f
                );

                Instantiate(fireOnGroundPrefab, transform.position + offset, Quaternion.identity);
                Destroy(gameObject, destroyDelayAfterSpawn);
            }
        }
    }
}
