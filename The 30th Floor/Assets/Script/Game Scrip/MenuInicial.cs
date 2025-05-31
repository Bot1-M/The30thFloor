using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuInicial : MonoBehaviour
{
    [SerializeField] private TMP_InputField playerNameInputField;

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

    public void comebackToMenuFromStartGame()
    {
        transitionUI.GameBackToMenuTransition();
        //SceneManager.LoadScene("MenuPrincipal"); // Cambia "MenuPrincipal" por el nombre de tu escena de menú principal
    }

    public void comebackToMenuFromConfig()
    {
        transitionUI.ConfigBackToMenuTransition();
    }
    public void comebackToMenuFromTutorial()
    {
        transitionUI.TutorialBackToMenuTransition();
    }

    public void configuracion()
    {
        transitionUI.ConfigTransition();
    }

    public void tutorial()
    {
        transitionUI.TutorialTransition();
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

    public void empezarPartida()
    {
        if (PlayerManager.Instance == null)
        {
            GameObject playerManagerPrefab = Resources.Load<GameObject>("Player");
            Instantiate(playerManagerPrefab);
        }

        if (string.IsNullOrEmpty(playerNameInputField.text))
        {
            Debug.LogWarning("El nombre del jugador no puede estar vacío.");
            return;
        }

        if (playerNameInputField.text.Length > 4)
        {
            Debug.LogWarning("El nombre del jugador debe tener menos de 4 caracteres.");
            return;
        }
        else
        {

            PlayerManager.Instance.Data.playerName = playerNameInputField.text;
            PlayerManager.Instance.enabled = true;
            SceneManager.LoadScene("Main");
        }
    }


}
