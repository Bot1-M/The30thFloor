using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


/// <summary>
/// Administra la escena de combate táctico. Coordina el tablero, HUD, sistema de turnos y transiciones.
/// </summary>
public class FightingSceneManager : MonoBehaviour
{
    /// <summary>
    /// Instancia única (singleton) accesible globalmente.
    /// </summary>
    public static FightingSceneManager Instance { get; private set; }

    public TurnManager turnManager;
    public GameObject exitPrefab;

    public event Action OnTurnManagerReady;
    public HealthBar healthBar;

    public Transform deathPanel;
    public TextMeshProUGUI pointsText;

    public GameObject pauseMenu;

    [SerializeField] private TMP_Text lbSpeed;
    [SerializeField] private TMP_Text lbAttack;
    [SerializeField] private TMP_Text lbHealth;
    [SerializeField] private TMP_Text lbMap;
    [SerializeField] private TMP_Text lbPoints;

    [SerializeField] private BoardManager boardManager;
    private GameObject player;

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
            PlayerManager.Instance.tacticalController.SetPauseMenu(pauseMenu);
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

        UpdateUI();

        List<ITurnTaker> turnOrder = new();

        turnOrder.Add(player.GetComponent<PlayerTacticalController>());

        foreach (var enemy in GameObject.FindGameObjectsWithTag("EnemyFighting"))
        {
            var enemyTaker = enemy.GetComponent<ITurnTaker>();
            if (enemyTaker != null)
                turnOrder.Add(enemyTaker);
        }

        turnManager.InitTurnOrder(turnOrder);
        Debug.Log("FightingSceneManager Start FIN()");

        turnManager.OnCombatFinished += OnCombatFinished;
    }

    /// <summary>
    /// Coloca al jugador en su celda inicial del combate.
    /// </summary>
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


    /// <summary>
    /// Actualiza la interfaz de usuario con los datos del jugador.
    /// </summary
    public void UpdateUI()
    {
        if (player == null)
        {
            Debug.Log("Player no encontrado.");
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

    /// <summary>
    /// Método llamado cuando el combate finaliza. Inicia la transición a la escena principal.
    /// </summary>
    private void OnCombatFinished()
    {
        Debug.Log("Combate finalizado. Volviendo a la escena principal...");
        AudioManager.Instance.PlaySFX("resultPoint");
        StartCoroutine(DelayedReturnToMain());
    }

    /// <summary>
    /// Espera 1 segundo y transiciona de vuelta a la escena "Main".
    /// </summary>
    private IEnumerator DelayedReturnToMain()
    {
        yield return new WaitForSeconds(1f);

        var transition = FindAnyObjectByType<SceneTransitionManager>();
        if (transition != null)
            transition.FadeToScene("Main");
        else
            SceneManager.LoadScene("Main");
    }

    /// <summary>
    /// Muestra el panel de muerte y destruye al jugador.
    /// </summary
    public void Death()
    {
        ScoreManager.SubmitScore();
        pointsText.text = PlayerManager.Instance.Data.totalPoints.ToString();
        deathPanel.gameObject.SetActive(true);
        Destroy(player);
    }

    public void playHoverSound()
    {
        AudioManager.Instance.PlaySFX("hoverSound");
    }

    public void playClickingSound()
    {
        AudioManager.Instance.PlaySFX("clickSound");
    }

    public void goBackToMenu()
    {
        AudioManager.Instance.PlaySFX("finalResultShow");
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

    public void exitGame()
    {
        AudioManager.Instance.PlaySFX("clickSound");
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }

}


