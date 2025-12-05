using UnityEngine;

public class PlayerSpirit : MonoBehaviour
{
    public float luzEspiritual = 100f;
    public float luzMaxima = 100f;

    public void ReduzirLuzEspiritual(float valor)
    {
        luzEspiritual -= valor;
        luzEspiritual = Mathf.Clamp(luzEspiritual, 0, luzMaxima);
        // Aqui você pode atualizar a UI ou mudar o ambiente
    }

    public void RestaurarLuzEspiritual(float valor)
    {
        luzEspiritual += valor;
        luzEspiritual = Mathf.Clamp(luzEspiritual, 0, luzMaxima);
    }
}
