using UnityEngine;
using UnityEngine.Audio;

public class SettingsMenu : MonoBehaviour
{
    public AudioMixer audioMixer;

    void Start()
    {
        if (audioMixer == null)
            Debug.LogError("AudioMixer não está atribuído no Inspector!");
        else
            Debug.Log("AudioMixer atribuído com sucesso.");
    }

    public void SetVolume(float volume)
    {
        // Evita volume zero que quebra o log10
        if (volume <= 0.001f)
        {
            audioMixer.SetFloat("Volume", -80f); // silêncio total
            Debug.Log("[Volume] Muted (<= 0.001)");
        }
        else
        {
            float dB = Mathf.Log10(volume) * 20f;
            audioMixer.SetFloat("Volume", dB);
            Debug.Log($"[Volume] Slider: {volume} → dB: {dB}");
        }
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }
}
