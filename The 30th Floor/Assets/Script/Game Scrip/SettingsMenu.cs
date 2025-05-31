using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown;

    private Resolution[] resolutions;

    private void Start()
    {
        var sm = SettingsManager.Instance;
        resolutions = Screen.resolutions;

        List<string> options = new();
        List<Resolution> uniqueResolutions = new();
        HashSet<string> seen = new();

        for (int i = 0; i < resolutions.Length; i++)
        {
            string res = resolutions[i].width + " x " + resolutions[i].height;
            if (!seen.Contains(res))
            {
                seen.Add(res);
                uniqueResolutions.Add(resolutions[i]);
                options.Add(res);
            }
        }

        resolutions = uniqueResolutions.ToArray();
        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(options);

        resolutionDropdown.value = Mathf.Clamp(sm.resolutionIndex, 0, resolutions.Length - 1);
        resolutionDropdown.RefreshShownValue();

        sm.ApplySettings(resolutions);
    }

    public void SetVolume(float volume)
    {
        SettingsManager.Instance.masterVolume = volume;
        SettingsManager.Instance.audioMixer.SetFloat("MasterVolume", volume);
    }

    public void SetMusicVolume(float volume)
    {
        SettingsManager.Instance.musicVolume = volume;
        SettingsManager.Instance.audioMixer.SetFloat("MusicVolume", volume);
    }

    public void SetSFXVolume(float volume)
    {
        SettingsManager.Instance.sfxVolume = volume;
        SettingsManager.Instance.audioMixer.SetFloat("SFXVolume", volume);
    }

    public void SetQuality(int qualityIndex)
    {
        SettingsManager.Instance.qualityIndex = qualityIndex;
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        SettingsManager.Instance.isFullscreen = isFullscreen;
        Screen.fullScreen = isFullscreen;
    }

    public void SetResolution(int resolutionIndex)
    {
        SettingsManager.Instance.resolutionIndex = resolutionIndex;

        if (resolutionIndex < 0 || resolutionIndex >= resolutions.Length)
        {
            Debug.LogError("Índice de resolución fuera de rango.");
            return;
        }

        Resolution selected = resolutions[resolutionIndex];
        Screen.SetResolution(selected.width, selected.height, Screen.fullScreen);
    }

    public void goBackToMenu()
    {
        AudioManager.Instance.PlaySFX("clickSound");

        if (SceneManager.GetActiveScene().name == "Menu")
        {
            Debug.Log("Ya estás en el menú.");
            return;
        }

        GameObject player = PlayerManager.Instance.gameObject;

        if (player != null)
            Destroy(player);

        if (GameManager.Instance != null)
            Destroy(GameManager.Instance.gameObject);

        unFreeze();
        SceneManager.LoadScene("Menu");
        gameObject.SetActive(false);
    }

    private void unFreeze()
    {
        Time.timeScale = 1f;
    }

    public void playHoverSound()
    {
        AudioManager.Instance.PlaySFX("hoverSound");
    }

    public void exitGame()
    {
        AudioManager.Instance.PlaySFX("clickSound");
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }
}
