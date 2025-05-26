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

    public Animator animator;

    //TODO VER QUE FUNCIONE ENTRE ESCENAS
    public HealthBar healthBar;

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
        animator = GetComponent<Animator>();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (playerInput != null)
        {
            playerInput.DeactivateInput(); // asegura que se desactiva correctamente
            playerInput.ActivateInput();   // vuelve a activarlo (rehace pairing)
        }

        if (scene.name == "Fighting")
        {
            playerInput.SwitchCurrentActionMap("Combat");
            explorationController.enabled = false;
            tacticalController.enabled = true;
        }
        else if (scene.name == "Main")
        {
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


