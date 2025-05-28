using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    private float followSpeed = 2f;

    [SerializeField]
    private Transform player;

    public void Init(Transform player)
    {
        this.player = player;

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
