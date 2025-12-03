using UnityEngine;

public class DanoMachado : MonoBehaviour
{
    public int danoMachado = 2; // Dano do machado (maior que ataque normal)
    public float forcaEmpurrao = 3f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Inimigo"))
        {
            InimigoAvançado inimigo = other.GetComponent<InimigoAvançado>();
            if (inimigo != null)
            {
                // Aplica dano
                inimigo.TomarDano(danoMachado, true);

                // Empurra o inimigo (opcional)
                Vector2 direcaoEmpurrao = (other.transform.position - transform.position).normalized;
                Rigidbody2D rbInimigo = other.GetComponent<Rigidbody2D>();
                if (rbInimigo != null)
                {
                    rbInimigo.AddForce(direcaoEmpurrao * forcaEmpurrao, ForceMode2D.Impulse);
                }

                Debug.Log("Machado acertou o inimigo! Dano: " + danoMachado);
            }
        }
    }
}