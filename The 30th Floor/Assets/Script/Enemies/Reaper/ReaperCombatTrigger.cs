using UnityEngine;

public class ReaperComebackTrigger : MonoBehaviour
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
