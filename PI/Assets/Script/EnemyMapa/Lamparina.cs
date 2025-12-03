using UnityEngine;

public class Lamparina : MonoBehaviour
{
    public float dano = 10f;
    public float intervaloDano = 0.5f;
    public Transform luzFrontal;
    public float alcance = 5f;
    public LayerMask enemyLayer;
    private bool luzForte = false;

    void Update()
    {
        if (luzForte)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(luzFrontal.position, alcance, enemyLayer);
            foreach (Collider2D hit in hits)
            {
             
            }
        }
    }

    public void AtivarLuzForte(float tempo)
    {
        luzForte = true;
        Invoke("DesativarLuzForte", tempo);
    }

    void DesativarLuzForte()
    {
        luzForte = false;
    }
}