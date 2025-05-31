using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class EnemyTacticalController : MonoBehaviour, ITurnTaker
{
    public GameObject FloatingTextPrefab;
    public int health;
    public int attackPower;

    private Vector2Int currentCell;
    private BoardManager board;
    private Transform playerTransform;

    private Animator animator;
    private bool facingRight = true;

    private Action onTurnComplete;

    private void Start()
    {
        board = FindAnyObjectByType<BoardManager>();
        playerTransform = GameObject.FindWithTag("Player").transform;
        animator = GetComponent<Animator>();
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

    private void FlipIfNeeded(Vector3 from, Vector3 to)
    {
        float dirX = to.x - from.x;
        if (dirX < -0.01f && facingRight) Flip();
        else if (dirX > 0.01f && !facingRight) Flip();
    }

    private void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
        facingRight = !facingRight;
    }

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
        AudioManager.Instance.PlaySFX("slimeHit");
        if (health <= 0)
        {
            board.SetOccupied(currentCell, null);

            if (FightingSceneManager.Instance != null)
            {
                FightingSceneManager.Instance.turnManager.RemoveTurnTaker(this);
            }

            if (FloatingTextPrefab)
            {
                ShowFloatingText();

            }

            Destroy(gameObject);
        }
    }

    private void ShowFloatingText()
    {
        Debug.Log("Mostrar texto flotante de puntos");
        Transform position = GameObject.FindWithTag("SpawnPointText").transform;
        GameObject go = Instantiate(FloatingTextPrefab, position.position, Quaternion.identity, position);
        go.GetComponent<TextMeshPro>().text = $"+{GetPoints()}";
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
            Vector3 enemyPos = transform.position;
            Vector3 playerWorld = board.GridToWorldCenter(playerCell);
            FlipIfNeeded(enemyPos, playerWorld);

            if (animator != null)
            {
                animator.SetTrigger("Attack");
            }
            yield return new WaitForSeconds(0.3f); // tiempo para que se vea el ataque

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

                Vector3 from = transform.position;
                Vector3 to = board.GridToWorldCenter(nextCell);
                FlipIfNeeded(from, to);

                currentCell = nextCell;
                transform.position = to;
            }
        }

        yield return new WaitForSeconds(0.2f);
        onTurnComplete?.Invoke();
    }

    private int GetPoints()
    {
        int points = 200 + (int)Math.Round(PlayerManager.Instance.Data.level * UnityEngine.Random.Range((float)1.0, (float)2.0));
        PlayerManager.Instance.Data.totalPoints += points;
        PlayerManager.Instance.tacticalController.fightPoints += points;
        return points;
    }


}


