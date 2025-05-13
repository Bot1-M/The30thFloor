using UnityEngine;
using UnityEngine.SceneManagement;

public class SlimeCombatTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Vector2Int playerGridPos = Vector2Int.RoundToInt(PlayerManager.Instance.transform.position);

        if (collision.CompareTag("Player"))
        {
            // Aquí puedes agregar la lógica para iniciar el combate
            Debug.Log("¡Combate iniciado con el jugador!");
            // Por ejemplo, podrías cargar una escena de combate o activar un sistema de combate.
            FindFirstObjectByType<SceneTransitionManager>().FadeToScene("Fighting");

        }
    }
}
