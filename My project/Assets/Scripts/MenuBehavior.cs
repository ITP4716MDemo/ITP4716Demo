using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuBehavior : MonoBehaviour
{
    [Header("Audio")]
    public AudioClip buttonClickSound;   // assign in Inspector
    public bool soundEnabled = true;     // option to enable/disable sound
    [Range(0f, 1f)]
    public float soundVolume = 1f;       // volume multiplier (0 = mute, 1 = full)

    private AudioSource audioSource;

    void Start()
    {
        // Get or add AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.volume = soundVolume;
    }

    // Optional: update volume if changed during runtime
    void Update()
    {
        if (audioSource != null && audioSource.volume != soundVolume)
            audioSource.volume = soundVolume;
    }

    private void PlayClickSound()
    {
        if (soundEnabled && buttonClickSound != null && audioSource != null)
            audioSource.PlayOneShot(buttonClickSound);
    }

    public void LoadScene(string sceneName)
    {
        PlayClickSound();
        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        PlayClickSound();
        Application.Quit();
    }

    // Optional: public methods to toggle sound from UI
    public void SetSoundEnabled(bool enabled)
    {
        soundEnabled = enabled;
    }

    public void SetSoundVolume(float volume)
    {
        soundVolume = Mathf.Clamp01(volume);
        if (audioSource != null)
            audioSource.volume = soundVolume;
    }
}