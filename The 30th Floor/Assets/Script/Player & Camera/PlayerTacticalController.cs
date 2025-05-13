using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerTacticalController : MonoBehaviour
{
    private BoardManager board;
    private Vector2Int cellPos;

    public void Spawn(BoardManager boardManager, Vector2Int cell)
    {
        if (boardManager == null)
        {
            Debug.LogError("BoardManager pasado a Spawn es null.");
            return;
        }

        board = boardManager;
        MoveTo(cell);
    }


    public void MoveTo(Vector2Int cell)
    {
        if (board == null)
        {
            Debug.LogError("BoardManager no está inicializado en PlayerTacticalController.");
            return;
        }

        cellPos = cell;

        Vector3Int cell3D = new Vector3Int(cell.x, cell.y, 0);
        transform.position = board.GetTilemap().GetCellCenterWorld(cell3D);

        Debug.Log($"Jugador movido a celda {cell}, posición real: {transform.position}");
    }


    void Update()
    {
        if (board == null)
            return;

        Vector2Int target = cellPos;
        bool moved = false;

        if (Keyboard.current.wKey.wasPressedThisFrame || Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            Debug.Log(" Flecha arriba detectada");

            target += Vector2Int.up;
            moved = true;
        }
        else if (Keyboard.current.sKey.wasPressedThisFrame || Keyboard.current.downArrowKey.wasPressedThisFrame)
        {
            Debug.Log(" Flecha abajo detectada");

            target += Vector2Int.down;
            moved = true;
        }
        else if (Keyboard.current.dKey.wasPressedThisFrame || Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            Debug.Log(" Flecha derecha detectada");

            target += Vector2Int.right;
            moved = true;
        }
        else if (Keyboard.current.aKey.wasPressedThisFrame || Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            Debug.Log(" Flecha abajo detectada");
            target += Vector2Int.left;
            moved = true;
        }

        if (moved)
        {
            BoardManager.CellData cellData = board.GetCellData(target);
            if (cellData != null && cellData.isWalkable)
            {
                FightingSceneManager.Instance.turnManager.Tick();
                MoveTo(target);
            }
            else
            {
                Debug.Log("Posición no caminable: " + target);
            }
        }
    }
}
