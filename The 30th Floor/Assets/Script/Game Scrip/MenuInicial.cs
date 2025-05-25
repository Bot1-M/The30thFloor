using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuInicial : MonoBehaviour
{
    [SerializeField]
    private MenuTransitionUI transitionUI;
    private void Start()
    {
        AudioManager.Instance.PlayMusic(AudioManager.Instance.menuClips[Random.Range(0, AudioManager.Instance.menuClips.Length)]);
    }

    public void jugar()
    {
        transitionUI.StartGameTransition();
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); // Cambia "Juego" por el nombre de tu escena de juego
    }

    public void comebackToMenu()
    {
        transitionUI.BackToMenuTransition();
        //SceneManager.LoadScene("MenuPrincipal"); // Cambia "MenuPrincipal" por el nombre de tu escena de menú principal
    }

    public void configuracion()
    {
        Debug.Log("Configuración no implementada aún");
    }

    public void tutorial()
    {
        Debug.Log("Tutorial no implementado aún");
    }
    public void salir()
    {
        Application.Quit(); // Cierra la aplicación
        Debug.Log("Salir del juego"); // Solo para propósitos de depuración
    }

    public void playClickingSound()
    {
        AudioManager.Instance.PlaySFX("clickSound");
    }


    public void playHoverSound()
    {
        AudioManager.Instance.PlaySFX("hoverSound");
    }
}
