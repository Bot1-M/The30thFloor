using System.Collections;
using UnityEngine;

public class SlimeCombatTrigger : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Time.timeScale = 0f; // Pausar el juego
            SceneTransitionManager sceneTransition = FindAnyObjectByType<SceneTransitionManager>();
            if (sceneTransition != null)
            {
                sceneTransition.FadeToScene("Fighting");
            }
        }
    }

}
