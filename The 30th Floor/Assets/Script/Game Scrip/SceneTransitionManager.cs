using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public CanvasGroup fadeCanvasGroup;
    public float fadeDuration = 1f;

    void Start()
    {
        StartCoroutine(FadeIn());
    }

    public void FadeToScene(string sceneName)
    {

        // Bloquea temporalmente el input
        if (PlayerManager.Instance != null)
            //PlayerManager.Instance.playerInput.enabled = false;

            StartCoroutine(FadeOutAndLoadScene(sceneName));
    }

    private IEnumerator FadeIn()
    {
        float t = fadeDuration;
        while (t > 0)
        {
            t -= Time.unscaledDeltaTime;
            fadeCanvasGroup.alpha = t / fadeDuration;
            yield return null;
        }

        fadeCanvasGroup.alpha = 0;

        // Restaurar el tiempo si se usó Time.timeScale = 0
        Time.timeScale = 1f;

        // Reactivar input con un frame de margen
        yield return null;

        if (PlayerManager.Instance != null)
            PlayerManager.Instance.playerInput.enabled = true;
    }

    private IEnumerator FadeOutAndLoadScene(string sceneName)
    {
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            fadeCanvasGroup.alpha = t / fadeDuration;
            yield return null;
        }

        fadeCanvasGroup.alpha = 1;

        // Cargar la nueva escena
        SceneManager.LoadScene(sceneName);
    }
}