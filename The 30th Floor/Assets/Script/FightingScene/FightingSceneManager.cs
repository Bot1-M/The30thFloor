using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
public class FightingSceneManager : MonoBehaviour
{
    public static FightingSceneManager Instance { get; private set; }

    public TurnManager turnManager;

    [SerializeField] private BoardManager boardManager;
    private GameObject player;

    private int turnNumber = 0;

    public UIDocument uiDocument;
    private Label lbSpeed;
    private Label lbAttack;
    private Label lbDefense;
    private Label lbHealth;
    private Label lbMap;
    private Label lbPoints;

    public HealthBar healthBar;


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

        turnManager = new TurnManager();

        //turnManager.OnTick += OnTurnHappen;

        //////////
        //////// CODIGO PARA TERMINAR LA FIGHTING SCENE
        ///
        //turnManager.OnCombatFinished += OnCombatEnded;


        boardManager.Init();


        if (player == null || boardManager == null)
        {
            Debug.LogWarning("Player, spawn point o BoardManager no encontrados.");
            return;
        }

        if (boardManager.IsReady)
        {
            SpawnPlayer();
        }
        else
        {
            boardManager.OnBoardReady += SpawnPlayer;
        }


        // Desactivar controller de exploración aquí si hace falta

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

    void OnTurnHappen()
    {
        turnNumber += 1;
        Debug.Log("Turn Number : " + turnNumber);
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
        lbDefense = uiDocument.rootVisualElement.Q<Label>("lbDefense");
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
        lbDefense.text = playerData.defense.ToString();
        lbSpeed.text = playerData.spaceMovement.ToString();
        lbMap.text = playerData.level.ToString();
        lbPoints.text = playerTactical.fightPoints.ToString();
    }

    //////////
    //////// CODIGO PARA TERMINAR LA FIGHTING SCENE
    ///

    //private void OnCombatEnded()
    //{
    //    Debug.Log("¡Combate finalizado con éxito!");

    //    // Aquí puedes:
    //    // - Transicionar a la escena de exploración
    //    // - Mostrar pantalla de victoria
    //    // - Dar recompensas, etc.

    //    SceneTransitionManager sceneTransition = FindObjectOfType<SceneTransitionManager>();
    //    if (sceneTransition != null)
    //    {
    //        sceneTransition.FadeToScene("Main");
    //    }
    //}


}


