using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    [SerializeField]
    private float moveSpeed = 5f; // Speed of the player movement

    private Rigidbody2D rb; // Reference to the Rigidbody2D component

    private Vector2 moveInput; // Input vector for movement


    float horizontalInput; // Variable to store horizontal input

    bool facingRight = true; // Boolean to check if the player is facing right

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    public void Init()
    {
        rb = GetComponent<Rigidbody2D>(); // Get the Rigidbody2D component attached to the player GameObject
        SpawnPlayer(); // Call the SpawnPlayer method to set the initial position of the player
    }

    void Update()
    {
        if (rb == null) return;

        horizontalInput = Input.GetAxis("Horizontal"); // Get horizontal input from the keyboard (A/D or Left/Right Arrow keys)
        rb.linearVelocity = moveInput * moveSpeed; // Set the horizontal velocity based on input and speed
        MirrorPlayer();


    }

    public void Move(InputAction.CallbackContext context)
    {
        if (context.canceled) // Check if the input action is canceled
        {
            AudioManager.Instance.StopWalkingSound(); // Stop the walking sound
            PlayerManager.Instance.animator.SetBool("isWalking", false); // Set the walking animation to false
        }
        else
        {
            AudioManager.Instance.PlayWalkingSound();
            PlayerManager.Instance.animator.SetBool("isWalking", true); // Set the walking animation to true
        }
        moveInput = context.ReadValue<Vector2>(); // Read the input value from the context and assign it to moveInput
    }

    private void MirrorPlayer()
    {
        if (horizontalInput < 0 && facingRight) // Check if the horizontal input moving left
        {
            Flip();
        }
        else if (horizontalInput > 0 && !facingRight) // Check if the horizontal input moving right
        {
            Flip();
        }
    }

    void Flip()
    {
        Vector3 currentScale = transform.localScale; // Get the current scale of the player
        currentScale.x *= -1; // Flip the x scale to mirror the player
        transform.localScale = currentScale; // Apply the new scale to the player
        facingRight = !facingRight; // Toggle the facing direction
    }

    private void SpawnPlayer()
    {
        // Spawn the player at the start position of the dungeon
        Vector3 spawnPosition = new Vector3(0, 0, 0); // Replace with your desired spawn position
        transform.position = spawnPosition; // Set the player's position to the spawn position


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
            GameManager.Instance.dungeonGenerator.GenerateDungeon();
            PlayerManager.Instance.explorationController.Init();
        }
    }

}
