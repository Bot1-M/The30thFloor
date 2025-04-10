using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuInicial : MonoBehaviour
{
    public void Jugar()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); // Cambia "Juego" por el nombre de tu escena de juego
    }

    public void Salir()
    {
        Application.Quit(); // Cierra la aplicación
        Debug.Log("Salir del juego"); // Solo para propósitos de depuración
    }
}
