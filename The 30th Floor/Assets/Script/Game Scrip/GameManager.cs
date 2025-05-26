using System;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public CorridorFirstDungeonGenerator dungeonGenerator;
    public PlayerManager player;

    [SerializeField] private HealthBar healthBar;

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

        setLabelUiDoc();
        UpdateUI();

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMusic(AudioManager.Instance.explorationClips[
                Random.Range(0, AudioManager.Instance.explorationClips.Length)]);
        }

    }

    // Update is called once per frame
    void Update()
    {
        UpdateUI();
    }

    public void setLabelUiDoc()
    {

        lbMap = uiDocument.rootVisualElement.Q<Label>("lbMap");
        lbPlayerName = uiDocument.rootVisualElement.Q<Label>("lbPlayerName");
        lbHealth = uiDocument.rootVisualElement.Q<Label>("lbHealth");
        lbPoints = uiDocument.rootVisualElement.Q<Label>("lbPoints");
    }

    public void UpdateUI()
    {
        lbPlayerName.text = player.Data.playerName;
        lbPoints.text = player.Data.totalPoints.ToString();
        lbHealth.text = player.Data.currentHealth + "/" + player.Data.maxHealth;
        lbMap.text = player.Data.level.ToString();
    }


}
