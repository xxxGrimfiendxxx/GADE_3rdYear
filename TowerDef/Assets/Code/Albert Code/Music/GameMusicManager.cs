using UnityEngine;
using UnityEngine.Audio;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class GameMusicManager : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource[] slowTracks;         // Slow songs array
    public AudioSource[] normalTracks;       // Normal songs array
    public AudioSource[] fastTracks;         // Fast songs array
    public AudioSource[] segments;           // Song segments array

    [Header("Audio Settings")]
    public AudioMixerGroup mixerGroup;       // Audio mixer for volume control
    public float minTempo = 0.8f;            // Minimum tempo speed
    public float maxTempo = 1.5f;            // Maximum tempo speed
    public float fadeDuration = 2.0f;        // Duration for fade out/in between songs
    public float volume = 1.0f;              // Volume control
    private bool isMuted = false;

    [Header("Gameplay and Score")]
    public TMP_Text scoreText;               // TextMeshPro object for score display
    public int towerCost = 300;              // Base tower cost
    private float gameProgressMultiplier = 0.1f;  // Multiplier for adjusting tempo based on progress

    private AudioSource currentTrack;
    private int currentRound = 1;
    private float currentTempo = 1.0f;

    void Start()
    {
        InitializeAudioSources();
        PlayInitialSong();
        SkipSong();
        
    }

    void Update()
    {
        AdjustTempoBasedOnGameplay();
    }

    public void MuteToggle()
    {
        isMuted = !isMuted;
        AudioListener.volume = isMuted ? 0 : 1;
    }

    private void InitializeAudioSources()
    {
        foreach (var source in slowTracks) source.outputAudioMixerGroup = mixerGroup;
        foreach (var source in normalTracks) source.outputAudioMixerGroup = mixerGroup;
        foreach (var source in fastTracks) source.outputAudioMixerGroup = mixerGroup;
        foreach (var source in segments) source.outputAudioMixerGroup = mixerGroup;
    }

    private void PlayInitialSong()
    {
        currentTrack = SelectSongBasedOnScore(0);
        currentTrack.Play();
    }

    private AudioSource SelectSongBasedOnScore(float score)
    {
        // Choose track based on score level
        if (score < 500) return GetRandomTrack(slowTracks);
        if (score < 1500) return GetRandomTrack(normalTracks);
        return GetRandomTrack(fastTracks);
    }

    private AudioSource GetRandomTrack(AudioSource[] tracks)
    {
        return tracks[Random.Range(0, tracks.Length)];
    }

    private void AdjustTempoBasedOnGameplay()
    {
        // Calculate the current score
        float score = GetScoreFromText();
        currentTempo = Mathf.Lerp(minTempo, maxTempo, (score / (towerCost * 10)) * gameProgressMultiplier);

        // Ensure tempo is within limits and apply
        currentTempo = Mathf.Clamp(currentTempo, minTempo, maxTempo);
        currentTrack.pitch = currentTempo;
        currentTrack.volume = isMuted ? 0 : volume;

        // If game progresses significantly, transition to next track
        if (score > 500 * currentRound)
        {
            currentRound++;
            SwitchToNextSong(score);
        }
    }

    private float GetScoreFromText()
    {
        if (scoreText && int.TryParse(scoreText.text, out int score)) return score;
        return 0f;
    }

    private void SwitchToNextSong(float score)
    {
        StartCoroutine(FadeOutAndSwitchSongs(score));
    }

    public void SetVolume(float sliderValue)
    {
        volume = sliderValue;         // Update the volume based on slider value
        if (currentTrack != null)
        {
            currentTrack.volume = volume;  // Apply volume to the currently playing track
        }
    }
    public void SkipSong()
    {
        if (currentTrack != null)
        {
            currentTrack.Stop();  // Stop the current song
            float score = GetScoreFromText();  // Get current score to select the next song
            SwitchToNextSong(score);  // Switch to the next song based on the score
        }
    }


    private IEnumerator FadeOutAndSwitchSongs(float score)
    {
        // Fade out current track
        float startVolume = currentTrack.volume;
        float timeElapsed = 0f;

        while (timeElapsed < fadeDuration)
        {
            currentTrack.volume = Mathf.Lerp(startVolume, 0, timeElapsed / fadeDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        currentTrack.Stop();

        // Choose next track and fade in
        currentTrack = SelectSongBasedOnScore(score);
        currentTrack.pitch = currentTempo;
        currentTrack.Play();

        timeElapsed = 0f;
        while (timeElapsed < fadeDuration)
        {
            currentTrack.volume = Mathf.Lerp(0, volume, timeElapsed / fadeDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }
}
