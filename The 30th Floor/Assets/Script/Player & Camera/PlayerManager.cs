using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    public PlayerData Data { get; set; } = new PlayerData();

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
        else if (scene.name == "Menu")
        {
            playerInput.enabled = false;
            tacticalController.enabled = false;
            explorationController.enabled = false;
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

    public static void AddHealth(PlayerData data, int amount)
    {
        if (amount <= 0) return;

        data.currentHealth += amount;
        data.currentHealth = Mathf.Min(data.currentHealth, data.maxHealth);
        Debug.Log($"+{amount} HP (actual: {data.currentHealth}/{data.maxHealth})");
    }

    public static void AddMaxHealth(PlayerData data, int amount)
    {
        if (amount <= 0) return;

        data.maxHealth += amount;
        Debug.Log($"+{amount} Max HP (nuevo máximo: {data.maxHealth})");
    }

    public static void AddAttack(PlayerData data, int amount)
    {
        if (amount <= 0) return;

        data.attack += amount;
        Debug.Log($"+{amount} ATK (nuevo: {data.attack})");
    }

    public static void AddMovement(PlayerData data, int amount)
    {
        if (amount <= 0) return;

        data.spaceMovement += amount;
        Debug.Log($"+{amount} MOV (nuevo: {data.spaceMovement})");
    }

    public static void AddPoints(PlayerData data, int amount)
    {
        if (amount <= 0) return;

        data.totalPoints += amount;
        Debug.Log($"+{amount} PTS (nuevo total: {data.totalPoints})");
    }
}


