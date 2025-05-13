using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
public class FightingSceneManager : MonoBehaviour
{
    public static FightingSceneManager Instance { get; private set; }

    public TurnManager turnManager;

    [SerializeField] private BoardManager boardManager;
    private GameObject player;

    private int turnNumber = 0;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        player = GameObject.FindWithTag("Player");
                
    }

    void Start()
    {
        // AQUII SI ENTRA Y NADA EN NULL   

        turnManager = new TurnManager();

        turnManager.OnTick += OnTurnHappen;

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

        SceneManager.sceneLoaded += OnSceneLoaded;

        // Desactivar controller de exploración aquí si hace falta

    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Fighting")
        {
            var spawnPoint = GameObject.Find("PlayerSpawnCombat");
            if (spawnPoint != null)
            {
                transform.position = spawnPoint.transform.position;
            }
        }


    }

    private void SpawnPlayer()
    {
        Debug.Log("Board listo, ejecutando Spawn del jugador.");
        var tactical = player.GetComponent<PlayerTacticalController>();
        if (tactical != null)
        {
            tactical.Spawn(boardManager, new Vector2Int(2, 2));
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

}


