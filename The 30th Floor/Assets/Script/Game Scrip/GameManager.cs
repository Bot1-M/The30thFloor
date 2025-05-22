using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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
        // Singleton pattern to ensure only one instance of GameManager exists
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
        dungeonGenerator.GenerateDungeon();
        player.explorationController.Init();

        healthBar.SetMaxHealth(player.Data.maxHealth);
        healthBar.SetHealth(player.Data.currentHealth);

        setLabelUiDoc();
        UpdateUI();


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
