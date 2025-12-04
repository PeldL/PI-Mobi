using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Configurações de vida")]
    public int maxHealth = 5;     // Vida máxima (5)
    public int currentHealth;     // Vida atual

    [Header("Referência UI")]
    public VidaUI vidaUI;         // Link para o script da UI

    void Start()
    {
        currentHealth = maxHealth;
        Debug.Log("Vida inicial: " + currentHealth);

        // Atualiza a UI no início
        if (vidaUI != null)
        {
            vidaUI.AtualizarVida(currentHealth);
        }
    }

    // Método para dar dano
    public void TakeDamage(int damage)
    {
        if (damage <= 0) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log("Dano recebido: " + damage + ", Vida atual: " + currentHealth);

        // Atualiza a UI
        if (vidaUI != null)
        {
            vidaUI.AtualizarVida(currentHealth);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Método para curar
    public void Heal(int amount)
    {
        if (amount <= 0) return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log("Cura: " + amount + ", Vida atual: " + currentHealth);

        // Atualiza a UI
        if (vidaUI != null)
        {
            vidaUI.AtualizarVida(currentHealth);
        }
    }

    // Método para morte
    private void Die()
    {
        Debug.Log("Player morreu!");
        SceneManager.LoadScene("Game Over");
    }

    // Método para obter vida atual (opcional)
    public int GetCurrentHealth()
    {
        return currentHealth;
    }
}