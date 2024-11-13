using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Audio;

public class MenuMusicManager : MonoBehaviour
{
    public AudioSource menuAudioSource;
    public float volume = 1.0f;
    private bool isMuted = false;

    void Start()
    {
        menuAudioSource.loop = true;
        menuAudioSource.Play();
        menuAudioSource.volume = volume;
    }

    public void MuteToggle()
    {
        isMuted = !isMuted;
        menuAudioSource.volume = isMuted ? 0 : volume;
    }

    public void SetVolume(float newVolume)
    {
        volume = newVolume;
        if (!isMuted)
        {
            menuAudioSource.volume = volume;
        }
    }
}
