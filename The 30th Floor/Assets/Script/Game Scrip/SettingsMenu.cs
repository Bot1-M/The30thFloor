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

    private void Start()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);

    }

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("MasterVolume", volume);
    }

    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", volume);
    }

    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFXVolume", volume);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
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
