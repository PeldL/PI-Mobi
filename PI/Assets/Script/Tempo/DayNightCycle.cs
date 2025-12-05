using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;

public class DayNightCycle : MonoBehaviour
{
    public static DayNightCycle Instance { get; private set; }

    public Light2D globalLight;
    public float currentTime;
    public float dayDurationInMinutes = 1f;

    public UnityEvent OnDayStart;
    public UnityEvent OnNightStart;

    private bool isDay;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        currentTime = PlayerPrefs.GetFloat("HoraAtual", 0f);
    }

    private void Update()
    {
        currentTime += (24f / (dayDurationInMinutes * 24f)) * Time.deltaTime;

        if (currentTime >= 24f)
            currentTime = 0f;

        PlayerPrefs.SetFloat("HoraAtual", currentTime);

        UpdateLighting();
    }

    void UpdateLighting()
    {
        if (currentTime >= 6f && currentTime < 18f)
        {
            if (!isDay)
            {
                isDay = true;
                OnDayStart.Invoke();
            }
            globalLight.intensity = Mathf.Lerp(globalLight.intensity, 1f, Time.deltaTime * 2f);
        }
        else
        {
            if (isDay)
            {
                isDay = false;
                OnNightStart.Invoke();
            }
            globalLight.intensity = Mathf.Lerp(globalLight.intensity, 0.3f, Time.deltaTime * 2f);
        }
    }
}