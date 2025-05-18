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
    private Label lbDefense;
    private Label lbAttack;
    private Label lbMap;
    private Label lbSpeed;
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

    }

    public void setLabelUiDoc()
    {
        lbAttack = uiDocument.rootVisualElement.Q<Label>("lbAttack");
        lbDefense = uiDocument.rootVisualElement.Q<Label>("lbDefense");
        lbHealth = uiDocument.rootVisualElement.Q<Label>("lbHealth");
        lbMap = uiDocument.rootVisualElement.Q<Label>("lbMap");
        lbSpeed = uiDocument.rootVisualElement.Q<Label>("lbSpeed");
    }

    public void UpdateUI()
    {
        lbAttack.text = player.Data.attack.ToString();
        lbDefense.text = player.Data.defense.ToString();
        lbHealth.text = player.Data.currentHealth + "/" + player.Data.maxHealth;
        lbMap.text = dungeonGenerator.Rooms.Count.ToString();
        lbSpeed.text = player.Data.spaceMovement.ToString();
    }
}
