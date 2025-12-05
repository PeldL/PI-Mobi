using UnityEngine;

public class FireOnGround : MonoBehaviour
{
    [Header("Dano ao jogador")]
    [SerializeField] private int damageAmount = 1;
    [SerializeField] private float damageInterval = 1f;

    [Header("Tempo de duração do fogo")]
    [SerializeField] private float duration = 5f;

    private float nextDamageTime = 0f;

    private void Start()
    {
        // Destroi automaticamente após 'duration' segundos
        Destroy(gameObject, duration);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (Time.time >= nextDamageTime)
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);
                nextDamageTime = Time.time + damageInterval;
                Debug.Log("Fogo no chão causou dano ao player.");
            }
        }
    }
}
