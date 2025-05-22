using System;
using System.Collections;
using UnityEngine;

public class EnemyTacticalController : MonoBehaviour, ITurnTaker
{
    public int health;
    public int attackPower;

    private Vector2Int currentCell;
    private BoardManager board;
    private Transform playerTransform;

    private Action onTurnComplete;

    private void Start()
    {
        board = FindAnyObjectByType<BoardManager>();
        playerTransform = GameObject.FindWithTag("Player").transform;

        // Registro al sistema de turnos
        //FightingSceneManager.Instance.turnManager.OnTick += OnTurnTick;
    }

    private void OnDestroy()
    {
        //if (FightingSceneManager.Instance != null && FightingSceneManager.Instance.turnManager != null)
        //{
        //    FightingSceneManager.Instance.turnManager.OnTick -= OnTurnTick;
        //}
    }

    public void Init(Vector2Int gridPos, int hp, int atk)
    {
        currentCell = gridPos;
        health = hp;
        attackPower = atk;
    }

    //private void OnTurnTick()
    //{
    //    Vector2Int playerCell = PlayerManager.Instance.tacticalController.Cell;

    //    // Verifica si está adyacente al jugador
    //    if (IsAdjacent(currentCell, playerCell))
    //    {
    //        Debug.Log("Enemy attacks the player!");
    //        PlayerManager.Instance.tacticalController.TakeDamage(attackPower);
    //    }
    //    else
    //    {
    //        Vector2Int direction = GetStepToward(currentCell, playerCell);
    //        Vector2Int nextCell = currentCell + direction;

    //        if (CanMoveTo(nextCell))
    //        {
    //            board.SetOccupied(currentCell, null);
    //            board.SetOccupied(nextCell, gameObject);
    //            currentCell = nextCell;
    //            transform.position = board.GridToWorldCenter(currentCell);
    //        }
    //    }
    //}

    private bool IsAdjacent(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) == 1;
    }

    private Vector2Int GetStepToward(Vector2Int from, Vector2Int to)
    {
        int xDist = to.x - from.x;
        int yDist = to.y - from.y;

        if (Mathf.Abs(xDist) > Mathf.Abs(yDist))
            return new Vector2Int((int)Mathf.Sign(xDist), 0);
        else
            return new Vector2Int(0, (int)Mathf.Sign(yDist));
    }

    private bool CanMoveTo(Vector2Int cell)
    {
        var data = board.GetCellData(cell);
        return data != null && data.isWalkable && !data.isOccupied;
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
        if (health <= 0)
        {
            board.SetOccupied(currentCell, null);

            if (FightingSceneManager.Instance != null)
            {
                FightingSceneManager.Instance.turnManager.RemoveTurnTaker(this);
            }

            Destroy(gameObject);
        }
    }

    public void StartTurn(Action onComplete)
    {
        Debug.Log("TURNO DEL ENEMIGO");
        onTurnComplete = onComplete;
        StartCoroutine(ExecuteEnemyAction());
    }

    private IEnumerator ExecuteEnemyAction()
    {
        yield return new WaitForSeconds(0.2f);

        Vector2Int playerCell = PlayerManager.Instance.tacticalController.Cell;

        if (IsAdjacent(currentCell, playerCell) && PlayerManager.Instance.Data.currentHealth > 0)
        {
            Debug.Log("Enemy attacks the player!");
            PlayerManager.Instance.tacticalController.TakeDamage(attackPower);
        }
        else
        {
            Vector2Int direction = GetStepToward(currentCell, playerCell);
            Vector2Int nextCell = currentCell + direction;

            if (CanMoveTo(nextCell))
            {
                board.SetOccupied(currentCell, null);
                board.SetOccupied(nextCell, gameObject);
                currentCell = nextCell;
                transform.position = board.GridToWorldCenter(currentCell);
            }
        }

        yield return new WaitForSeconds(0.2f);
        onTurnComplete?.Invoke();
    }

}


