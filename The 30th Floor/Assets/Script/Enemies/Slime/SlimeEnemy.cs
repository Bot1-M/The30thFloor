using UnityEngine;

public class SlimeEnemy : MonoBehaviour
{
    private Transform player;
    [SerializeField] private float distanceToPlayer = 0f;
    private bool hasTriggered = false;

    public Vector3 spawnPoint;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spawnPoint = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(player.position, transform.position);
        animator.SetFloat("Distance", distance);

        if (!hasTriggered && distance <= distanceToPlayer)
        {
            hasTriggered = true;
            Time.timeScale = 0f; // Pausar el juego
            Debug.Log("Distancia alcanzada, cambiando a escena Fighting");
            FindFirstObjectByType<SceneTransitionManager>().FadeToScene("Fighting");
            player.GetComponent<PlayerTacticalController>().enabled = false; // Desactivar el controlador del jugador
        }
    }

    public void Spin(Vector3 objective)
    {
        if (transform.position.x < objective.x)
        {
            spriteRenderer.flipX = false;
        }
        else
        {
            spriteRenderer.flipX = true;
        }
    }
}
