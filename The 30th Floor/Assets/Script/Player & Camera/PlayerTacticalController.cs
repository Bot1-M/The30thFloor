using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;


/// <summary>
/// Controlador táctico del jugador durante los combates por turnos.
/// Gestiona movimiento por celdas, ataques, orientación y eventos de turno.
/// </summary>
public class PlayerTacticalController : MonoBehaviour, ITurnTaker
{
    public int fightPoints = 0;
    public Vector2Int Cell => cellPos;

    [SerializeField] private GameObject pauseMenuCombat;
    private BoardManager board;
    private Vector2Int cellPos;

    [SerializeField] private HealthBar healthBar;

    [SerializeField] private Tile highlightTile;

    private bool subscribedToTurnEvent = false;

    private bool hasActed = false;

    private List<Vector2Int> reachableCellsThisTurn = new();

    //SLOW THE MOVEMENT OF THE PLAYER

    [SerializeField] private float moveSpeed = 5.0f;

    private bool isMoving = false;
    private Vector3 moveTarget;

    private Queue<Vector2Int> cellPath = new();

    private Animator animator;

    private bool facingRight = true;

    private Action onTurnComplete;

    /// <summary>
    /// Indica si el jugador se está moviendo actualmente.
    /// </summary>
    public bool IsMoving() => isMoving;

    private bool isPaused = false;

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            TogglePauseMenu();
            return;
        }

        if (!subscribedToTurnEvent)
            TrySubscribeToTurnSystem();



        if (isPaused)
            return;


        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, moveTarget, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, moveTarget) < 0.01f)
            {
                transform.position = moveTarget;
                isMoving = false;

                if (cellPath.Count > 0)
                {
                    MoveTo(cellPath.Dequeue(), false);
                }
                else
                {
                    if (animator != null) PlayerManager.Instance.animator.SetBool("isWalking", false);
                    Debug.Log("Movimiento completo");
                }
            }

            return;
        }

        if (board == null) return;

        TryHandleMouseClick();
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryManualAttack(); // solo si se presiona E
        }

    }


    /// <summary>
    /// Mueve al jugador a una celda específica.
    /// </summary>
    public void MoveTo(Vector2Int cell, bool immediate = false)
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        cellPos = cell;
        Vector3Int cell3D = new Vector3Int(cell.x, cell.y, 0);
        Vector3 targetWorldPos = board.GetTilemap().GetCellCenterWorld(cell3D);

        if (immediate)
        {
            isMoving = false;
            transform.position = targetWorldPos;
            if (animator != null) PlayerManager.Instance.animator.SetBool("isWalking", false);
            Debug.Log("isWalking: " + PlayerManager.Instance.animator.GetBool("isWalking"));


        }
        else
        {
            FlipIfNeeded(transform.position, targetWorldPos);
            isMoving = true;
            moveTarget = targetWorldPos;
            if (animator != null) PlayerManager.Instance.animator.SetBool("isWalking", true);
            Debug.Log("isWalking: " + PlayerManager.Instance.animator.GetBool("isWalking"));

        }

        Debug.Log($" MoveTo: {cell} {(immediate ? "[instant]" : "[smooth]")}");
    }

    /// <summary>
    /// Ejecuta la lógica cuando se hace clic en una celda válida.
    /// </summary>
    private void HandleMovementTo(Vector2Int target)
    {
        if (hasActed)
        {
            Debug.Log("Ya has actuado este turno.");
            return;
        }

        if (!reachableCellsThisTurn.Contains(target))
        {
            Debug.Log("¡Celda fuera del rango permitido!");
            return;
        }

        List<Vector2Int> path = GetPath(cellPos, target);
        if (path.Count == 0)
        {
            Debug.Log("No hay camino válido hasta el destino.");
            return;
        }

        hasActed = true;
        board.ClearOverlay();
        StartTilePathMovement(path);
        cellPos = target;
        //FightingSceneManager.Instance.turnManager.Tick();
        StartCoroutine(ExecuteMovementThenEndTurn(path));

    }

    /// <summary>
    /// Detecta clic y gestiona el movimiento si es válido.
    /// </summary>
    private void TryHandleMouseClick()
    {
        if (!Mouse.current.leftButton.wasPressedThisFrame) return;

        Vector2Int clickedCell = GetClickedCell();
        if (IsValidTarget(clickedCell))
        {
            HandleMovementTo(clickedCell);
        }
        else
        {
            Debug.Log("Celda no válida para moverse: " + clickedCell);
        }
    }

    private Vector2Int GetClickedCell()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector3Int cellClicked = board.GetTilemap().WorldToCell(mouseWorldPos);
        return new Vector2Int(cellClicked.x, cellClicked.y);
    }

    private bool IsValidTarget(Vector2Int cell)
    {
        var data = board.GetCellData(cell);
        if (data == null || !data.isWalkable)
            return false;

        return !data.isOccupied ||
               (data.occupant != null && data.occupant.CompareTag("ExitCombat"));
    }

    /// <summary>
    /// Reduce la vida del jugador y actualiza la interfaz.
    /// </summary>
    public void TakeDamage(int amount)
    {
        if (animator != null) PlayerManager.Instance.animator.SetTrigger("isDamage");
        AudioManager.Instance.PlaySFX("hitSound");
        var data = PlayerManager.Instance.Data;
        data.currentHealth -= amount;
        data.currentHealth = Mathf.Max(0, data.currentHealth);

        if (healthBar != null)
            healthBar.SetHealth(data.currentHealth);

        if (data.currentHealth <= 0)
        {
            AudioManager.Instance.PlaySFX("deathMaleCry");
            FightingSceneManager.Instance.Death();
            Debug.Log("El jugador ha muerto.");
        }
    }

    /// <summary>
    /// Muestra el rango de movimiento basado en la velocidad del jugador.
    /// </summary>
    private void ShowMovementRange()
    {
        if (board == null) return;

        var data = PlayerManager.Instance.Data;
        reachableCellsThisTurn = GetReachableCells(cellPos, data.spaceMovement);
        board.ClearOverlay();
        board.ShowOverlay(reachableCellsThisTurn, highlightTile);
    }

    private List<Vector2Int> GetReachableCells(Vector2Int start, int range)
    {
        List<Vector2Int> result = new();
        Queue<(Vector2Int pos, int steps)> queue = new();
        HashSet<Vector2Int> visited = new();

        queue.Enqueue((start, 0));
        visited.Add(start);

        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        while (queue.Count > 0)
        {
            var (current, steps) = queue.Dequeue();
            if (steps > range) continue;

            result.Add(current);

            foreach (var dir in directions)
            {
                Vector2Int next = current + dir;
                if (!visited.Contains(next) && board.IsWalkable(next))
                {
                    var cellData = board.GetCellData(next);
                    if (cellData != null && (!cellData.isOccupied ||
                        (cellData.occupant != null && cellData.occupant.CompareTag("ExitCombat"))))
                    {
                        visited.Add(next);
                        queue.Enqueue((next, steps + 1));
                    }
                }

            }
        }

        return result;
    }

    /// <summary>
    /// Posiciona al jugador en el tablero y configura componentes.
    /// </summary>
    public void Spawn(BoardManager boardManager, Vector2Int cell)
    {
        if (pauseMenuCombat == null)
        {
            pauseMenuCombat = GameObject.FindGameObjectWithTag("SettingsMenu");
        }

        SetHealthBar();

        board = boardManager;
        MoveTo(cell, true);
        animator = GetComponent<Animator>();

        // Forzar orientación hacia la derecha
        Vector3 scale = transform.localScale;
        if (scale.x < 0)
            scale.x *= -1;
        transform.localScale = scale;

        if (board.IsReady)
        {
            ShowMovementRange();
        }
        else
        {
            board.OnBoardReady += OnBoardReadyForPlayer;
        }
    }


    private void OnBoardReadyForPlayer()
    {
        board.OnBoardReady -= OnBoardReadyForPlayer;
        ShowMovementRange();
    }

    private void SetHealthBar()
    {
        healthBar = FightingSceneManager.Instance.healthBar;
    }

    private void TrySubscribeToTurnSystem()
    {
        var sceneManager = FightingSceneManager.Instance;

        if (sceneManager == null) return;

        var turnMgr = sceneManager.turnManager;

        if (turnMgr == null) return; // turnManager aún no ha sido creado (lo hace en Start)

        subscribedToTurnEvent = true;

        ShowMovementRange(); // Mostrar las celdas en el primer turno
    }

    private List<Vector2Int> GetPath(Vector2Int start, Vector2Int end)
    {
        Dictionary<Vector2Int, Vector2Int> cameFrom = new();
        Queue<Vector2Int> queue = new();
        HashSet<Vector2Int> visited = new();

        queue.Enqueue(start);
        visited.Add(start);
        cameFrom[start] = start;

        Vector2Int[] dirs = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            if (current == end)
                break;

            foreach (var dir in dirs)
            {
                Vector2Int next = current + dir;
                if (visited.Contains(next)) continue;

                if (board.IsWalkable(next))
                {
                    visited.Add(next);
                    cameFrom[next] = current;
                    queue.Enqueue(next);
                }
            }
        }

        if (!cameFrom.ContainsKey(end))
            return new(); // no path

        List<Vector2Int> path = new();
        Vector2Int step = end;
        while (step != start)
        {
            path.Add(step);
            step = cameFrom[step];
        }
        path.Reverse();
        return path;
    }

    internal void StartTilePathMovement(List<Vector2Int> path)
    {
        if (path == null || path.Count == 0) return;

        cellPath = new Queue<Vector2Int>(path);
        MoveTo(cellPath.Dequeue(), false); // mover a la primera celda suavemente
    }

    private void FlipIfNeeded(Vector3 from, Vector3 to)
    {
        float directionX = to.x - from.x;

        if (directionX < -0.01f && facingRight)
        {
            Flip();
        }
        else if (directionX > 0.01f && !facingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
        facingRight = !facingRight;
    }

    public void StartTurn(Action onComplete)
    {
        hasActed = false;
        onTurnComplete = onComplete;
        ShowMovementRange();
    }

    private IEnumerator ExecuteMovementThenEndTurn(List<Vector2Int> path)
    {
        StartTilePathMovement(path);
        while (isMoving)
            yield return null;

        yield return new WaitForSeconds(0.2f);

        onTurnComplete?.Invoke(); // Notificamos que hemos terminado el turno
    }

    private void TryManualAttack()
    {
        if (hasActed)
        {
            Debug.Log("Ya has actuado este turno.");
            return;
        }

        // Buscar enemigo adyacente primero
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        foreach (var dir in directions)
        {
            Vector2Int adjacent = cellPos + dir;
            var cell = board.GetCellData(adjacent);

            if (cell != null && cell.isOccupied && cell.occupant != null)
            {
                var enemy = cell.occupant.GetComponent<SlimeTacticalController>();
                if (enemy != null)
                {
                    // FLIPEAR ANTES DE ATACAR
                    Vector3 enemyPos = board.GridToWorldCenter(adjacent);
                    FlipIfNeeded(transform.position, enemyPos);

                    break;
                }
            }
        }

        // Ahora sí: atacar
        board.ClearOverlay();
        AudioManager.Instance.PlaySFX("hitSound");
        PlayerManager.Instance.animator.SetTrigger("Attack");
        hasActed = true;
        StartCoroutine(AttackRoutine());
    }

    private bool TryAttackEnemy()
    {
        Vector2Int[] directions = { Vector2Int.left, Vector2Int.right, Vector2Int.up, Vector2Int.down };

        foreach (var dir in directions)
        {
            Vector2Int adjacent = cellPos + dir;
            var cell = board.GetCellData(adjacent);

            if (cell != null && cell.isOccupied && cell.occupant != null)
            {
                var enemy = cell.occupant.GetComponent<SlimeTacticalController>();
                if (enemy != null)
                {
                    Vector3 enemyPos = board.GridToWorldCenter(adjacent);
                    FlipIfNeeded(transform.position, enemyPos);

                    Debug.Log("Jugador ataca al enemigo adyacente con la tecla E!");
                    enemy.TakeDamage(PlayerManager.Instance.Data.attack);
                    return true;
                }
            }
        }

        return false;
    }
    private IEnumerator AttackRoutine()
    {
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"));

        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
        float attackDuration = state.length;

        yield return new WaitForSeconds(attackDuration); // espera a que termine

        if (TryAttackEnemy())
        {
            Debug.Log("Ataque completado, finalizando turno...");
        }

        onTurnComplete?.Invoke(); // continuar con el turno
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("ExitCombat"))
        {
            PlayerManager.Instance.animator.SetBool("isWalking", false);
            PlayerManager.Instance.GetComponent<SpriteRenderer>().enabled = false;
            SceneTransitionManager stm = FindAnyObjectByType<SceneTransitionManager>();
            if (stm != null)
            {
                stm.FadeToScene("Main");
                board.ClearOverlay();
                Time.timeScale = 0f; // Pausar el juego
            }
            else
            {
                Debug.LogWarning("SceneTransitionManager no encontrado.");
                SceneManager.LoadScene("Main"); // fallback
                board.ClearOverlay();
                Time.timeScale = 0f; // Pausar el juego
            }
        }
    }

    private void TogglePauseMenu()
    {
        isPaused = !isPaused;

        if (pauseMenuCombat != null)
            pauseMenuCombat.SetActive(isPaused);
    }

    public void SetPauseMenu(GameObject pauseMenu)
    {
        this.pauseMenuCombat = pauseMenu;
    }

}
