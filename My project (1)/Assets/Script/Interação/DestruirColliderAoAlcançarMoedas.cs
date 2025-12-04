using UnityEngine;

public class DestruirColliderAoAlcançarMoedas : MonoBehaviour
{
    [Header("Requisitos")]
    public int moedasNecessarias = 200;

    [Header("O que será destruído")]
    public GameObject objetoParaDestruir; // Pode ser o próprio GameObject, uma barreira, etc.

    private bool jaDestruiu = false;

    void Update()
    {
        if (!jaDestruiu && GameData.Instance != null && GameData.Instance.coins >= moedasNecessarias)
        {
            if (objetoParaDestruir != null)
            {
                Destroy(objetoParaDestruir);
                Debug.Log("✅ Requisito de moedas alcançado. Objeto destruído.");
            }
            else
            {
                // Se não tiver nada atribuído, destrói o próprio objeto onde o script está
                Destroy(gameObject);
                Debug.Log("✅ Objeto com o script destruído por padrão.");
            }

            jaDestruiu = true; // Garante que só execute uma vez
        }
    }
}
