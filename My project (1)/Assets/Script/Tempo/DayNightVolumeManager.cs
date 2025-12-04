using UnityEngine;

using UnityEngine.Rendering;

public class DayNightVolumeManager : MonoBehaviour
{
    [Header("Referências")]
    public DayNightCycle dayNightCycle;
    public Volume volume;
    public VolumeProfile dayProfile;
    public VolumeProfile nightProfile;

    private void OnEnable()
    {
        dayNightCycle.OnDayStart.AddListener(SetDayProfile);
        dayNightCycle.OnNightStart.AddListener(SetNightProfile);
    }

    private void OnDisable()
    {
        dayNightCycle.OnDayStart.RemoveListener(SetDayProfile);
        dayNightCycle.OnNightStart.RemoveListener(SetNightProfile);
    }

    private void SetDayProfile()
    {
        if (volume != null && dayProfile != null)
            volume.profile = dayProfile;
    }

    private void SetNightProfile()
    {
        if (volume != null && nightProfile != null)
            volume.profile = nightProfile;
    }
}