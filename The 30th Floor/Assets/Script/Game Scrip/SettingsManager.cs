using UnityEngine;
using UnityEngine.Audio;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }

    public AudioMixer audioMixer;

    public float masterVolume = 0f;
    public float musicVolume = 0f;
    public float sfxVolume = 0f;

    public int resolutionIndex = 0;
    public int qualityIndex = 2;
    public bool isFullscreen = true;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ApplySettings(Resolution[] resolutions)
    {
        audioMixer.SetFloat("MasterVolume", masterVolume);
        audioMixer.SetFloat("MusicVolume", musicVolume);
        audioMixer.SetFloat("SFXVolume", sfxVolume);

        QualitySettings.SetQualityLevel(qualityIndex);
        if (resolutions != null && resolutionIndex < resolutions.Length)
        {
            Resolution res = resolutions[resolutionIndex];
            Screen.SetResolution(res.width, res.height, isFullscreen);
        }
    }
}
