using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SlimeCombatTrigger : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SceneTransitionManager sceneTransition = FindAnyObjectByType<SceneTransitionManager>();
            if (sceneTransition != null)
            {
                sceneTransition.FadeToScene("Fighting");
            }
        }
    }

}
