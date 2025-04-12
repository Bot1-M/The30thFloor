using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    [SerializeField] 
    private float moveSpeed = 5f; // Speed of the player movement
    private Rigidbody2D rb; // Reference to the Rigidbody2D component
    private Vector2 moveInput; // Input vector for movement

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Get the Rigidbody2D component attached to the player GameObject

    }

    // Update is called once per frame
    void Update()
    {
        rb.linearVelocity = moveInput * moveSpeed; // Set the horizontal velocity based on input and speed
    }

    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>(); // Read the input value from the context and assign it to moveInput
    }


}
