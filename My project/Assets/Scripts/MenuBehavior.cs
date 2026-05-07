using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuBehavior : MonoBehaviour
{
    [Header("Audio")]
    public AudioClip buttonClickSound;
    public bool soundEnabled = true;
    [Range(0f, 1f)]
    public float soundVolume = 1f;

    [Header("Points Reset")]
    public string gameSceneName = "GameScene";   // name of the scene where the game starts
    public bool resetPointsOnPlay = true;        // enable/disable points reset

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.volume = soundVolume;
    }

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

        // Reset points and unlocks if the target scene is the game scene
        if (resetPointsOnPlay && sceneName == gameSceneName)
        {
            ResetProgress();
        }

        SceneManager.LoadScene(sceneName);
    }

    private void ResetProgress()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogWarning("GameManager.Instance not found. Progress not reset.");
            return;
        }

        // Reset total points
        GameManager.Instance.TotalPoints = 0;
        Debug.Log("Points reset to 0.");

        // Reset unlocked foods – only keep those with unlockCost == 0 (default items)
        int keptCount = 0;
        int resetCount = 0;

        foreach (var food in GameManager.Instance.unlockableFoods)
        {
            if (food.unlockCost == 0)
            {
                // Default items – keep unlocked
                if (!food.isUnlocked)
                    food.isUnlocked = true;
                keptCount++;
            }
            else
            {
                // Purchased items – lock them
                if (food.isUnlocked)
                {
                    food.isUnlocked = false;
                    resetCount++;
                }
            }
        }

        // Persist the changes
        GameManager.Instance.SaveUnlockStates();
        Debug.Log($"Unlock reset complete. {keptCount} default foods kept unlocked, {resetCount} purchased foods locked.");
    }

    public void QuitGame()
    {
        PlayClickSound();
        Application.Quit();
    }

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