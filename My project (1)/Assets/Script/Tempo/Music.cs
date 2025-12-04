using UnityEngine;

public class Music: MonoBehaviour
{
    public AudioSource musicSource;
    public AudioClip dayMusic;
    public AudioClip nightMusic;

    public void PlayDayMusic()
    {
        if (musicSource != null && dayMusic != null)
        {
            musicSource.clip = dayMusic;
            musicSource.Play();
        }
    }

    public void PlayNightMusic()
    {
        if (musicSource != null && nightMusic != null)
        {
            musicSource.clip = nightMusic;
            musicSource.Play();
        }
    }
}