using UnityEngine;

public class Clock : MonoBehaviour
{
    public Transform ponteiroRelogio;
    public DayNightCycle dayNightCycle;

    private void Update()
    {
        if (dayNightCycle == null || ponteiroRelogio == null)
            return;

        float horaAtual = dayNightCycle.currentTime;
        float angulo = (horaAtual / 24f) * 360f;
        ponteiroRelogio.rotation = Quaternion.Euler(0f, 0f, -angulo + 180f);
    }

}