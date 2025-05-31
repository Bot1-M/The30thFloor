using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

/// <summary>
/// Controlador general del juego durante la fase de exploración.
/// Se encarga de inicializar el dungeon, actualizar la UI, manejar el jugador y avanzar de nivel.
/// </summary
public class GameManager : MonoBehaviour
{
    /// <summary> Instancia global (singleton) del GameManager. </summary>
    public static GameManager Instance { get; private set; }

    public CorridorFirstDungeonGenerator dungeonGenerator;
    public PlayerManager player;
    public CameraFollow cameraFollow;
    public GameObject pauseMenu;

    public HealthBar healthBar;

    [SerializeField] private TMP_Text lbHealth;
    [SerializeField] private TMP_Text lbMap;
    [SerializeField] private TMP_Text lbPlayerName;
    [SerializeField] private TMP_Text lbPoints;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        //SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = PlayerManager.Instance;

        if (player == null)
        {
            Debug.LogError("PlayerManager.Instance es null. Asegúrate de que PlayerManager exista en la escena o fue instanciado antes.");
            return;
        }

        dungeonGenerator.GenerateDungeon();

        if (player.explorationController != null)
        {
            player.explorationController.Init();
        }
        else
        {
            Debug.LogWarning("explorationController es null.");
        }

        if (healthBar != null)
        {
            healthBar.SetMaxHealth(player.Data.maxHealth);
            healthBar.SetHealth(player.Data.currentHealth);
        }
        else
        {
            Debug.LogWarning("healthBar no está asignado en el GameManager.");
        }

        UpdateUI();

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMusic(AudioManager.Instance.explorationClips[
                Random.Range(0, AudioManager.Instance.explorationClips.Length)]);
        }

        if (cameraFollow == null)
        {
            cameraFollow = FindAnyObjectByType<CameraFollow>();
            if (cameraFollow == null)
            {
                Debug.Log("No se encontró CameraFollow en la escena.");
                return;
            }
        }
        cameraFollow.Init(player.transform);

        if (pauseMenu == null)
        {
            pauseMenu = GameObject.FindWithTag("SettingsMenu");
            if (pauseMenu == null)
            {
                Debug.LogError("No se encontró el objeto PauseMenu en la escena.");
                return;
            }
        }

        player.explorationController.setPause(pauseMenu);

    }

    // Update is called once per frame
    void Update()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        lbPlayerName.text = player.Data.playerName;
        lbPoints.text = player.Data.totalPoints.ToString();
        lbHealth.text = player.Data.currentHealth + "/" + player.Data.maxHealth;
        lbMap.text = player.Data.level.ToString();
    }

    /// <summary>
    /// Avanza al siguiente nivel de dungeon. Se llama desde la casilla "Finish".
    /// Regenera dungeon, reinicia el jugador, reubica cámara y actualiza interfaz.
    /// </summary>
    public void AdvanceToNextLevel()
    {
        Debug.Log("Avanzando al siguiente nivel...");

        // Subir de nivel
        player.Data.level += 1;

        // Regenerar dungeon
        dungeonGenerator.GenerateDungeon();

        // Reposicionar jugador y reinicializar
        player.explorationController.Init();

        // Reposicionar cámara
        if (cameraFollow != null)
        {
            cameraFollow.Init(player.transform);
        }

        // Actualizar barra de vida
        if (healthBar != null)
        {
            healthBar.SetHealth(player.Data.currentHealth);
            healthBar.SetMaxHealth(player.Data.maxHealth);
        }

        // Reproducir música de exploración
        if (AudioManager.Instance != null && AudioManager.Instance.explorationClips.Length > 0)
        {
            AudioManager.Instance.PlayMusic(AudioManager.Instance.explorationClips[
                Random.Range(0, AudioManager.Instance.explorationClips.Length)]);
        }

        // Actualizar UI
        UpdateUI();

        Debug.Log($"Nuevo nivel alcanzado: {player.Data.level}");
    }


}
