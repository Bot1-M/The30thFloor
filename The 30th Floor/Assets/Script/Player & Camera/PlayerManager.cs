using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    public PlayerData Data { get; private set; } = new PlayerData();

    public PlayerInput playerInput;
    public PlayerController explorationController;
    public PlayerTacticalController tacticalController;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        playerInput = GetComponent<PlayerInput>();
        explorationController = GetComponent<PlayerController>();
        tacticalController = GetComponent<PlayerTacticalController>();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Fighting")
        {
            // Activar modo combate
            playerInput.SwitchCurrentActionMap("Combat");
            explorationController.enabled = false;
            tacticalController.enabled = true;
        }
        else if (scene.name == "Main")
        {
            // Activar modo exploración
            playerInput.SwitchCurrentActionMap("Exploration");
            tacticalController.enabled = false;
            explorationController.enabled = true;
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public GameObject GetPlayer()
    {
        return GameObject.FindWithTag("Player");
    }
}


