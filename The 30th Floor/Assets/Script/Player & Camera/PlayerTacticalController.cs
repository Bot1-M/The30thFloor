using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using static BoardManager;

public class PlayerTacticalController : MonoBehaviour, ITurnTaker
{
    public int fightPoints = 0;
    public Vector2Int Cell => cellPos;

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
    public bool IsMoving() => isMoving;

    void Update()
    {
        if (!subscribedToTurnEvent)
            TrySubscribeToTurnSystem();

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
               (data.occupant != null && data.occupant.CompareTag("Finish"));
    }

    public void TakeDamage(int amount)
    {
        var data = PlayerManager.Instance.Data;
        data.currentHealth -= amount;
        data.currentHealth = Mathf.Max(0, data.currentHealth);

        if (healthBar != null)
            healthBar.SetHealth(data.currentHealth);

        if (data.currentHealth <= 0)
        {
            Debug.Log("El jugador ha muerto.");
        }
    }

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
                        (cellData.occupant != null && cellData.occupant.CompareTag("Finish"))))
                    {
                        visited.Add(next);
                        queue.Enqueue((next, steps + 1));
                    }
                }

            }
        }

        return result;
    }
    public void Spawn(BoardManager boardManager, Vector2Int cell)
    {
        SetHealthBar();

        board = boardManager;
        MoveTo(cell, true);
        animator = GetComponent<Animator>();

        if (board.IsReady)
        {
            //boardIsReady = true;
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
        //boardIsReady = true;
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


    private void OnDisable()
    {
        //if (FightingSceneManager.Instance != null)
        //    FightingSceneManager.Instance.turnManager.OnTick -= OnTurnStarted;
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

        if (!IsEnemyAdjacent())
        {
            Debug.Log("No hay enemigos adyacentes.");
            return;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            board.ClearOverlay();
            PlayerManager.Instance.animator.SetTrigger("Attack");
            if (TryAttackEnemy())
            {
                hasActed = true;
                onTurnComplete?.Invoke();
            }
        }
    }

    private bool IsEnemyAdjacent()
    {
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        foreach (var dir in directions)
        {
            Vector2Int adjacent = cellPos + dir;
            var cell = board.GetCellData(adjacent);

            if (cell != null && cell.isOccupied && cell.occupant != null)
            {
                if (cell.occupant.GetComponent<EnemyTacticalController>() != null)
                {
                    return true;
                }
            }
        }
        return false;
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
                var enemy = cell.occupant.GetComponent<EnemyTacticalController>();
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Finish"))
        {
            SceneManager.LoadScene("Main");
        }
    }


}
