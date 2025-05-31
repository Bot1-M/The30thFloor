using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public CorridorFirstDungeonGenerator dungeonGenerator;
    public PlayerManager player;
    public CameraFollow cameraFollow;

    public HealthBar healthBar;

    public UIDocument uiDocument;
    private Label lbHealth;
    private Label lbMap;
    private Label lbPlayerName;
    private Label lbPoints;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Init()
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

        setLabelUiDoc();
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

    }

    // Update is called once per frame
    void Update()
    {
        UpdateUI();
    }

    public void goBackToMenu()
    {
        AudioManager.Instance.PlaySFX("clickSound");
        if (SceneManager.GetActiveScene().name == "Menu")
        {
            Debug.Log("Ya estás en el menú.");
            return;
        }

        Destroy(player.gameObject);
        unFreeze();
        SceneManager.LoadScene("Menu");

        gameObject.SetActive(false);

    }

    private void unFreeze()
    {
        Time.timeScale = 1f;
    }

    public void playHoverSound()
    {
        AudioManager.Instance.PlaySFX("hoverSound");
    }

    public void exitGame()
    {
        AudioManager.Instance.PlaySFX("clickSound");
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }

    public void setLabelUiDoc()
    {
        if (uiDocument == null)
        {
            Debug.Log("UIDocument no asignado.");
            return;
        }

        var root = uiDocument.rootVisualElement;

        lbMap = root.Q<Label>("lbMap");
        if (lbMap == null) Debug.LogWarning("Label 'lbMap' no encontrado en el UI Document.");

        lbPlayerName = root.Q<Label>("lbPlayerName");
        if (lbPlayerName == null) Debug.LogWarning("Label 'lbPlayerName' no encontrado en el UI Document.");

        lbHealth = root.Q<Label>("lbHealth");
        if (lbHealth == null) Debug.LogWarning("Label 'lbHealth' no encontrado en el UI Document.");

        lbPoints = root.Q<Label>("lbPoints");
        if (lbPoints == null) Debug.LogWarning("Label 'lbPoints' no encontrado en el UI Document.");
    }

    public void UpdateUI()
    {
        lbPlayerName.text = player.Data.playerName;
        lbPoints.text = player.Data.totalPoints.ToString();
        lbHealth.text = player.Data.currentHealth + "/" + player.Data.maxHealth;
        lbMap.text = player.Data.level.ToString();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Main")
        {
            if (gameObject.IsUnityNull()) return;
            gameObject.SetActive(true);

            player = PlayerManager.Instance;
            if (player == null)
            {
                Debug.LogError("PlayerManager.Instance es null. Asegúrate de que PlayerManager exista en la escena o fue instanciado antes.");
            }
            dungeonGenerator = FindAnyObjectByType<CorridorFirstDungeonGenerator>();
            if (dungeonGenerator == null)
            {
                Debug.LogError("No se encontró CorridorFirstDungeonGenerator en la escena.");
            }
            cameraFollow = FindAnyObjectByType<CameraFollow>();
            if (cameraFollow == null)
            {
                Debug.LogError("No se encontró CameraFollow en la escena.");
            }
            healthBar = FindAnyObjectByType<HealthBar>();
            if (healthBar == null)
            {
                Debug.LogError("No se encontró HealthBar en la escena.");
            }
            uiDocument = FindAnyObjectByType<UIDocument>();
            if (uiDocument == null)
            {
                Debug.LogError("No se encontró UIDocument en la escena.");
            }
            Init();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

}
