using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private float horizontalInput;
    private bool facingRight = true;

    [Header("Menu de pausa")]
    [SerializeField] private GameObject pauseMenu;

    private bool isPaused = false;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void Init()
    {
        rb = GetComponent<Rigidbody2D>();
        SpawnPlayer();
    }

    void Update()
    {
        if (rb == null) return;

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            TogglePauseMenu();
        }

        if (!isPaused)
        {
            horizontalInput = Input.GetAxis("Horizontal");
            rb.linearVelocity = moveInput * moveSpeed;
            MirrorPlayer();
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        if (isPaused) return;

        if (context.canceled)
        {
            AudioManager.Instance.StopWalkingSound();
            PlayerManager.Instance.animator.SetBool("isWalking", false);
        }
        else
        {
            AudioManager.Instance.StartWalkingSound();
            PlayerManager.Instance.animator.SetBool("isWalking", true);
        }

        moveInput = context.ReadValue<Vector2>();
    }

    private void MirrorPlayer()
    {
        if (horizontalInput < 0 && facingRight && transform.localScale.x > 0)
            Flip();
        else if (horizontalInput > 0 && !facingRight && transform.localScale.x < 0)
            Flip();
    }

    void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
        facingRight = !facingRight;
    }

    private void SpawnPlayer()
    {
        transform.position = Vector3.zero;
    }

    private void TogglePauseMenu()
    {
        isPaused = !isPaused;

        if (pauseMenu != null)
        {
            pauseMenu.SetActive(isPaused);
        }

        Time.timeScale = isPaused ? 0f : 1f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Coin"))
        {
            Destroy(collision.gameObject);
            AudioManager.Instance.PlaySFX("pointsAdded");
            PlayerManager.Instance.Data.totalPoints += 100;
            Debug.Log("Total Points: " + PlayerManager.Instance.Data.totalPoints);
        }

        if (collision.gameObject.CompareTag("Finish"))
        {
            PlayerManager.Instance.Data.level += 1;
            Debug.Log("Salida activada. Regenerando dungeon...");
            //GameManager.Instance.dungeonGenerator.GenerateDungeon();
            PlayerManager.Instance.explorationController.Init();
        }
    }
}

