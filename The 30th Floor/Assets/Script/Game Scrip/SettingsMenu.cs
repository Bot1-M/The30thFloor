using System.Collections.Generic;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Menú de configuración del juego.
/// Permite ajustar resolución, volumen, calidad gráfica, modo pantalla completa
/// y navegar entre escenas del menú o salir del juego.
/// </summary>
public class SettingsMenu : MonoBehaviour
{


    public AudioMixer audioMixer;

    private Resolution[] resolutions;
    public TMP_Dropdown resolutionDropdown;

    private const float defaultMasterVolume = 0f;
    private const float defaultMusicVolume = 0f;
    private const float defaultSFXVolume = 0f;
    private const int defaultResolutionIndex = 0;
    private const int defaultQualityIndex = 2;
    private const bool defaultFullscreen = true;


    private void Start()
    {
        resolutions = Screen.resolutions;

        // Filtrar duplicados por resolución (ancho x alto)
        List<string> options = new List<string>();
        List<Resolution> uniqueResolutions = new List<Resolution>();
        HashSet<string> seenResolutions = new HashSet<string>();

        for (int i = 0; i < resolutions.Length; i++)
        {
            string resString = resolutions[i].width + " x " + resolutions[i].height;
            if (!seenResolutions.Contains(resString))
            {
                seenResolutions.Add(resString);
                uniqueResolutions.Add(resolutions[i]);
                options.Add(resString);
            }
        }

        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(options);

        resolutions = uniqueResolutions.ToArray(); // actualizar la lista interna para que SetResolution funcione

        int resIndex = PlayerPrefs.GetInt("resolutionIndex", defaultResolutionIndex);
        resolutionDropdown.value = Mathf.Clamp(resIndex, 0, resolutions.Length - 1);
        SetResolution(resIndex);

        // Calidad
        int quality = PlayerPrefs.GetInt("qualityIndex", defaultQualityIndex);
        QualitySettings.SetQualityLevel(quality);

        // Pantalla completa
        bool isFullscreen = PlayerPrefs.GetInt("fullscreen", defaultFullscreen ? 1 : 0) == 1;
        Screen.fullScreen = isFullscreen;

        // Volúmenes
        float master = PlayerPrefs.GetFloat("masterVolume", defaultMasterVolume);
        float music = PlayerPrefs.GetFloat("musicVolume", defaultMusicVolume);
        float sfx = PlayerPrefs.GetFloat("sfxVolume", defaultSFXVolume);

        audioMixer.SetFloat("MasterVolume", master);
        audioMixer.SetFloat("MusicVolume", music);
        audioMixer.SetFloat("SFXVolume", sfx);
    }

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("MasterVolume", volume);
        PlayerPrefs.SetFloat("masterVolume", volume);
    }

    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", volume);
        PlayerPrefs.SetFloat("musicVolume", volume);
    }

    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFXVolume", volume);
        PlayerPrefs.SetFloat("sfxVolume", volume);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.SetInt("qualityIndex", qualityIndex);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("fullscreen", isFullscreen ? 1 : 0);
    }

    public void SetResolution(int width, int height)
    {
        Screen.SetResolution(width, height, Screen.fullScreen);
    }

    public void SetResolution(int resolutionIndex)
    {
        if (resolutionIndex < 0 || resolutionIndex >= resolutions.Length)
        {
            Debug.LogError("Índice de resolución fuera de rango.");
            return;
        }
        Resolution selectedResolution = resolutions[resolutionIndex];
        Screen.SetResolution(selectedResolution.width, selectedResolution.height, Screen.fullScreen);
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
        {
            Destroy(player);
        }

        if (GameManager.Instance != null)
        {
            Destroy(GameManager.Instance.gameObject);
        }

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
