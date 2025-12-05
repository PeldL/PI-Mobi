using UnityEngine;

/// <summary>
/// Interface que define algo que pode mudar o tempo.
/// </summary>
public interface ITimeChanger
{
    void SetTime(float hora);
}

/// <summary>
/// Implementação do controlador de tempo (DayNightCycle).
/// </summary>
public class TimeController : MonoBehaviour, ITimeChanger
{
    public static TimeController Instance { get; private set; }
    public DayNightCycle cycle;

    private void Awake()
    {
        Instance = this;
    }

    public void SetTime(float hora)
    {
        if (cycle != null)
        {
            cycle.currentTime = hora;
            PlayerPrefs.SetFloat("HoraAtual", hora);
        }
    }
}
