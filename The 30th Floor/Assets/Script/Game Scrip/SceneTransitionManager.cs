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
        // Aquí puedes bloquear el input del jugador
        if (PlayerManager.Instance != null)
            PlayerManager.Instance.playerInput.enabled = false;

        // Opcional: parar todo el juego si quieres hard freeze
        // Time.timeScale = 0f;

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

        // Restaurar el tiempo cuando la nueva escena empieza
        Time.timeScale = 1f;

        // Volver a permitir input si quieres
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

        // Cargar la siguiente escena (ya en negro)
        SceneManager.LoadScene(sceneName);
    }
}
