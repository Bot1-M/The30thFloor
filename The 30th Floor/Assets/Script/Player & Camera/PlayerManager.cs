using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


/// <summary>
/// Gestor principal del jugador. Maneja el acceso global a los datos del jugador,
/// los controladores de movimiento (exploraci�n y combate), y asegura persistencia entre escenas.
/// </summary>
public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    public PlayerData Data { get; set; } = new PlayerData();

    public PlayerInput playerInput;
    public PlayerController explorationController;
    public PlayerTacticalController tacticalController;

    public Animator animator;

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


    /// <summary>
    /// M�todo ejecutado cada vez que se carga una nueva escena.
    /// Activa/desactiva controladores y mapea el esquema de input seg�n la escena activa.
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (playerInput != null)
        {
            Instance.GetComponent<SpriteRenderer>().enabled = true;
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


    /// <summary>
    /// Se desuscribe del evento al destruir el objeto.
    /// </summary>
    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    /// <summary>
    /// Devuelve una referencia al GameObject del jugador.
    /// </summary>
    public GameObject GetPlayer()
    {
        return GameObject.FindWithTag("Player");
    }

    #region M�todos de mejora de estad�sticas

    /// <summary>
    /// A�ade vida al jugador sin exceder el m�ximo.
    /// </summary>
    public static void AddHealth(PlayerData data, int amount)
    {
        if (amount <= 0) return;

        data.currentHealth += amount;
        data.currentHealth = Mathf.Min(data.currentHealth, data.maxHealth);
        Debug.Log($"+{amount} HP (actual: {data.currentHealth}/{data.maxHealth})");
    }

    /// <summary>
    /// Aumenta la vida m�xima del jugador y ajusta la vida actual en proporci�n.
    /// </summary>
    public static void AddMaxHealth(PlayerData data, int amount)
    {
        if (amount <= 0) return;

        float healthRatio = (float)data.currentHealth / data.maxHealth;

        data.maxHealth += amount;
        data.currentHealth = Mathf.RoundToInt(data.maxHealth * healthRatio);

        Debug.Log($"+{amount} Max HP (nuevo m�ximo: {data.maxHealth}, nueva vida actual: {data.currentHealth})");
    }

    /// <summary>
    /// Incrementa el ataque del jugador.
    /// </summary>
    public static void AddAttack(PlayerData data, int amount)
    {
        if (amount <= 0) return;

        data.attack += amount;
        Debug.Log($"+{amount} ATK (nuevo: {data.attack})");
    }

    /// <summary>
    /// Incrementa el alcance de movimiento del jugador en el modo t�ctico.
    /// </summary>
    public static void AddMovement(PlayerData data, int amount)
    {
        if (amount <= 0) return;

        data.spaceMovement += amount;
        Debug.Log($"+{amount} MOV (nuevo: {data.spaceMovement})");
    }

    /// <summary>
    /// A�ade puntos a la puntuaci�n total del jugador.
    /// </summary>
    public static void AddPoints(PlayerData data, int amount)
    {
        if (amount <= 0) return;

        data.totalPoints += amount;
        Debug.Log($"+{amount} PTS (nuevo total: {data.totalPoints})");
    }

    #endregion

}


