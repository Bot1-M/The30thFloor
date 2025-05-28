using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
public class FightingSceneManager : MonoBehaviour
{
    public static FightingSceneManager Instance { get; private set; }

    public TurnManager turnManager;
    public GameObject exitPrefab;

    public event Action OnTurnManagerReady;
    public HealthBar healthBar;

    public Transform deathPanel;
    public TMPro.TextMeshProUGUI pointsText;

    public UIDocument uiDocument;
    private Label lbSpeed;
    private Label lbAttack;
    private Label lbHealth;
    private Label lbMap;
    private Label lbPoints;
    [SerializeField] private BoardManager boardManager;
    private GameObject player;

    private int turnNumber = 0;


    private void Awake()
    {
        Debug.Log("FightingSceneManager Awake() Principio");

        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        player = GameObject.FindWithTag("Player");
        Debug.Log("FightingSceneManager Awake FIN()");

    }

    void Start()
    {
        // AQUII SI ENTRA Y NADA EN NULL   
        Debug.Log("FightingSceneManager Start () Principio");
        deathPanel.gameObject.SetActive(false);

        turnManager = new TurnManager();

        OnTurnManagerReady?.Invoke();

        boardManager.Init();

        AudioManager.Instance.PlayMusic(AudioManager.Instance.fightingClips);

        if (player == null || boardManager == null)
        {
            Debug.LogWarning("Player, spawn point o BoardManager no encontrados.");
            return;
        }

        if (boardManager.IsReady)
        {
            SpawnPlayer();
            boardManager.SpawnExitAt(new Vector2Int(16, 7), exitPrefab);
        }
        else
        {
            boardManager.OnBoardReady += () =>
            {
                SpawnPlayer();
                boardManager.SpawnExitAt(new Vector2Int(16, 7), exitPrefab);
            };
        }


        // Desactivar controller de exploraci�n aqu� si hace falta

        var playerData = player.GetComponent<PlayerManager>().Data;

        healthBar.SetMaxHealth(playerData.maxHealth);
        healthBar.SetHealth(playerData.currentHealth);

        SetUpUIDocument();
        UpdateUI();

        List<ITurnTaker> turnOrder = new();
        turnOrder.Add(player.GetComponent<PlayerTacticalController>());

        foreach (var enemy in GameObject.FindGameObjectsWithTag("EnemyFighting"))
        {
            var enemyTaker = enemy.GetComponent<EnemyTacticalController>();
            if (enemyTaker != null)
                turnOrder.Add(enemyTaker);
        }

        turnManager.InitTurnOrder(turnOrder);
        Debug.Log("FightingSceneManager Start FIN()");

    }

    private void SpawnPlayer()
    {
        Debug.Log("Board listo, ejecutando Spawn del jugador.");
        var tactical = player.GetComponent<PlayerTacticalController>();
        if (tactical != null)
        {
            tactical.Spawn(boardManager, new Vector2Int(1, 1));
        }
        else
        {
            Debug.LogError("Player no tiene PlayerTacticalController.");
        }
    }

    private void SetUpUIDocument()
    {
        if (uiDocument == null)
        {
            Debug.LogError("UIDocument no asignado.");
            return;
        }
        lbHealth = uiDocument.rootVisualElement.Q<Label>("lbHealth");
        lbAttack = uiDocument.rootVisualElement.Q<Label>("lbAttack");
        lbSpeed = uiDocument.rootVisualElement.Q<Label>("lbSpeed");
        lbMap = uiDocument.rootVisualElement.Q<Label>("lbMap");
        lbPoints = uiDocument.rootVisualElement.Q<Label>("lbFightingPoints");
    }

    public void UpdateUI()
    {
        if (uiDocument == null)
        {
            Debug.LogError("UIDocument no asignado.");
            return;
        }
        if (player == null)
        {
            Debug.LogError("Player no encontrado.");
            return;
        }
        var playerData = player.GetComponent<PlayerManager>().Data;
        var playerTactical = player.GetComponent<PlayerTacticalController>();
        lbHealth.text = playerData.currentHealth + "/" + playerData.maxHealth;
        lbAttack.text = playerData.attack.ToString();
        lbSpeed.text = playerData.spaceMovement.ToString();
        lbMap.text = playerData.level.ToString();
        lbPoints.text = playerTactical.fightPoints.ToString();
    }

    private void Update()
    {
        UpdateUI();
    }

    ////////
    ////// CODIGO PARA TERMINAR LA FIGHTING SCENE


    private void OnCombatEnded()
    {
        Debug.Log("�Combate finalizado con �xito!");

        // Aqu� puedes:
        // - Transicionar a la escena de exploraci�n
        // - Mostrar pantalla de victoria
        // - Dar recompensas, etc.

        SceneTransitionManager sceneTransition = FindObjectOfType<SceneTransitionManager>();
        if (sceneTransition != null)
        {
            sceneTransition.FadeToScene("Main");
        }
    }

    public void Death()
    {
        ScoreManager.SubmitScore();
        pointsText.text = PlayerManager.Instance.Data.totalPoints.ToString();
        deathPanel.gameObject.SetActive(true);
        uiDocument.enabled = false;
        Destroy(player);
    }

    public void BackToMenu()
    {
        Debug.Log("Volviendo al menu principal...");
        SceneManager.LoadScene("Menu");
    }

    public void playHoverSound()
    {
        AudioManager.Instance.PlaySFX("hoverSound");
    }

    public void playClickingSound()
    {
        AudioManager.Instance.PlaySFX("clickSound");
    }


}


