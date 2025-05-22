using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
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

    private Queue<Vector2Int> cellPath = new(); // celdas a recorrer

    private Animator animator;

    private bool facingRight = true;

    //private bool boardIsReady = false;

    private Action onTurnComplete;

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
                    MoveTo(cellPath.Dequeue(), false); // siguiente paso
                }
                else
                {
                    if (animator != null) animator.SetBool("isWalking", false);
                    Debug.Log("Movimiento completo");
                }
            }

            return;
        }

        if (board == null) return;

        TryHandleMouseClick();
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
            if (animator != null) animator.SetBool("isWalking", false);
            Debug.Log("isWalking: " + animator.GetBool("isWalking"));


        }
        else
        {
            FlipIfNeeded(transform.position, targetWorldPos);
            isMoving = true;
            moveTarget = targetWorldPos;
            if (animator != null) animator.SetBool("isWalking", true);
            Debug.Log("isWalking: " + animator.GetBool("isWalking"));

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
        return data != null && data.isWalkable;
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
                    visited.Add(next);
                    queue.Enqueue((next, steps + 1));
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

    //private void OnTurnStarted()
    //{
    //    hasActed = false;
    //    if (boardIsReady)
    //        StartCoroutine(DeferredShowMovementRange());
    //}

    //private IEnumerator DeferredShowMovementRange()
    //{
    //    yield return null; // espera al siguiente frame
    //    ShowMovementRange();
    //}

    private void TrySubscribeToTurnSystem()
    {
        var sceneManager = FightingSceneManager.Instance;

        if (sceneManager == null) return;

        var turnMgr = sceneManager.turnManager;

        if (turnMgr == null) return; // turnManager aún no ha sido creado (lo hace en Start)

        //turnMgr.OnTick += OnTurnStarted;
        subscribedToTurnEvent = true;

        ShowMovementRange(); // Mostrar las celdas en el primer turno
    }

    private void OnDestroy()
    {
        //if (subscribedToTurnEvent && FightingSceneManager.Instance?.turnManager != null)
        //{
        //    FightingSceneManager.Instance.turnManager.OnTick -= OnTurnStarted;
        //}
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

    private void StartTilePathMovement(List<Vector2Int> path)
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

        TryAttackEnemy();

        onTurnComplete?.Invoke(); // Notificamos que hemos terminado el turno
    }

    private void TryAttackEnemy()
    {
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        foreach (var dir in directions)
        {
            Vector2Int adjacent = cellPos + dir;
            var cell = board.GetCellData(adjacent);

            if (cell != null && cell.isOccupied && cell.occupant != null)
            {
                var enemy = cell.occupant.GetComponent<EnemyTacticalController>();
                if (enemy != null)
                {
                    Debug.Log("Jugador ataca al enemigo adyacente!");
                    enemy.TakeDamage(PlayerManager.Instance.Data.attack);
                    return; // solo ataca al primero que encuentra
                }
            }
        }
    }

}
