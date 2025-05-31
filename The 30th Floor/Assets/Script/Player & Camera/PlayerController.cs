using JetBrains.Annotations;
using TMPro;
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
    public GameObject pauseMenu;

    private bool isPaused = false;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void Init()
    {
        rb = GetComponent<Rigidbody2D>();
        ResetVisualDefaults();
        MirrorPlayer();
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
            GameManager.Instance.AdvanceToNextLevel();
        }
    }

    public void setPause(GameObject pause)
    {
        pauseMenu = pause;
    }

    public void ResetVisualDefaults()
    {
        // Forzar rotación a 0
        transform.rotation = Quaternion.identity;

        // Forzar escala a 0.4 (derecha por defecto)
        Vector3 defaultScale = new Vector3(0.4f, 0.4f, 0.4f);
        transform.localScale = defaultScale;

        // Set facingRight en base a la escala
        facingRight = transform.localScale.x > 0;

        Debug.Log("Visuales del jugador reiniciados: rotación 0, escala 0.4, facingRight=" + facingRight);
    }

}

