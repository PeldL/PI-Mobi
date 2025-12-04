using UnityEngine;
using UnityEngine.UI;

public class SleepUI : MonoBehaviour
{
    public GameObject painelDormir;
    public Button botaoAmanha;
    public Button botaoNoite;

    private void Start()
    {
        if (painelDormir != null)
            painelDormir.SetActive(false);

        if (botaoAmanha != null)
            botaoAmanha.onClick.AddListener(() => Dormir(6f));

        if (botaoNoite != null)
            botaoNoite.onClick.AddListener(() => Dormir(20f));
    }

    public void AbrirMenu()
    {
        if (painelDormir != null)
            painelDormir.SetActive(true);
    }

    public void FecharMenu()
    {
        if (painelDormir != null)
            painelDormir.SetActive(false);
    }

    private void Dormir(float hora)
    {
        // Muda direto o currentTime do DayNightCycle
        DayNightCycle.Instance.currentTime = hora;
        Debug.Log($"Você dormiu... Agora são {hora}:00!");

        FecharMenu();
    }
}