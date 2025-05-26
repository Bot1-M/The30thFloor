using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    private float followSpeed = 2f;

    [SerializeField]
    private Transform player;

    void Start()
    {
        if (player == null)
        {
            Debug.LogError("Player transform is not assigned in the inspector.");
            return;
        }

        // Set the initial position of the camera to match the player's position
        Vector3 initialPosition = new Vector3(player.position.x, player.position.y, transform.position.z);
        transform.position = initialPosition;
    }

    void Update()
    {
        if (player == null)
            return;

        Vector3 newPosition = new Vector3(player.position.x, player.position.y, transform.position.z);
       

        transform.position = Vector3.Slerp(transform.position, newPosition, followSpeed * Time.deltaTime);
    }
}
