using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    public TextMeshProUGUI finalScoreText;

    [Header("Audio")]
    public AudioClip buttonClickSound;   // assign in Inspector
    public bool soundEnabled = true;     // option to enable/disable sound
    [Range(0f, 1f)]
    public float soundVolume = 1f;       // volume multiplier

    private AudioSource audioSource;

    void Start()
    {
        // Get or add AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.volume = soundVolume;

        // Show and unlock cursor for UI interaction
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        int finalScore = PlayerPrefs.GetInt("FinalScore", 0);
        finalScoreText.text = "Final Score: " + finalScore;
    }

    // Update volume if changed during runtime
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

    public void RestartGame()
    {
        PlayClickSound();
        SceneManager.LoadScene("GameScene");
    }

    public void QuitGame()
    {
        PlayClickSound();

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
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