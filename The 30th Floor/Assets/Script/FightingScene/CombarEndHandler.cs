using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CombatEndHandler : MonoBehaviour
{
    private void Start()
    {
        // Si ya está inicializado (por ejemplo, si este Start ocurre más tarde)
        if (FightingSceneManager.Instance?.turnManager != null)
        {
            SubscribeToCombatEnd();
        }
        else
        {
            FightingSceneManager.Instance.OnTurnManagerReady += SubscribeToCombatEnd;
        }
    }

    private void SubscribeToCombatEnd()
    {
        Debug.Log("CombatEndHandler: suscribiéndose a OnCombatFinished");

        var tm = FightingSceneManager.Instance.turnManager;
        tm.OnCombatFinished += MovePlayerAfterCombat;
    }


    private void OnDestroy()
    {
        if (FightingSceneManager.Instance?.turnManager != null)
        {
            FightingSceneManager.Instance.turnManager.OnCombatFinished -= MovePlayerAfterCombat;
        }
    }

    private void MovePlayerAfterCombat()
    {
        Debug.Log("Jugador va a ser movido tras el combate...");

        var player = PlayerManager.Instance.tacticalController;
        var board = FindAnyObjectByType<BoardManager>();

        Vector2Int start = player.Cell;
        Vector2Int fallbackTarget = new Vector2Int(16, 7);
        Vector2Int target = FindClosestReachableCell(start, fallbackTarget, board);

        if (target == start)
        {
            Debug.LogWarning("No se encontró ninguna celda alcanzable para moverse.");
            return;
        }

        // Asegurar que la celda está libre y caminable
        var data = board.GetCellData(target);
        if (data == null)
        {
            Debug.LogError("No hay datos de celda en la celda seleccionada.");
            return;
        }

        board.SetOccupied(target, null);
        data.isWalkable = true;

        board.SetOccupied(start, null);
        board.SetOccupied(target, player.gameObject);

        List<Vector2Int> path = GetPath(start, target, board);
        if (path.Count > 0)
        {
            player.StartCoroutine(MoveAndFinish(path));
        }
        else
        {
            Debug.LogWarning($"No se pudo encontrar camino a {target}");
        }
    }



    private List<Vector2Int> GetPath(Vector2Int start, Vector2Int end, BoardManager board)
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
            if (current == end) break;

            foreach (var dir in dirs)
            {
                Vector2Int next = current + dir;
                if (visited.Contains(next)) continue;

                if (board.IsWalkable(next) && !board.GetCellData(next).isOccupied)
                {
                    visited.Add(next);
                    cameFrom[next] = current;
                    queue.Enqueue(next);
                }
            }
        }

        if (!cameFrom.ContainsKey(end)) return new(); // sin camino

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

    private IEnumerator MoveAndFinish(List<Vector2Int> path)
    {
        var player = PlayerManager.Instance.tacticalController;

        // Empezamos movimiento suave
        player.StartTilePathMovement(path);

        // Esperamos hasta que termine de moverse
        while (player.IsMoving())
            yield return null;

        Debug.Log("Jugador movido suavemente al final del combate.");
    }

    private Vector2Int FindClosestReachableCell(Vector2Int from, Vector2Int target, BoardManager board)
    {
        List<Vector2Int> candidates = new();

        // Buscamos en un rango pequeño alrededor
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                Vector2Int candidate = target + new Vector2Int(dx, dy);
                var data = board.GetCellData(candidate);
                if (data != null && data.isWalkable && !data.isOccupied)
                {
                    candidates.Add(candidate);
                }
            }
        }

        // Ordenar por distancia y devolver el primero con path válido
        foreach (var candidate in candidates.OrderBy(c => Vector2Int.Distance(from, c)))
        {
            var path = GetPath(from, candidate, board);
            if (path.Count > 0)
            {
                Debug.Log($"Celda alternativa encontrada: {candidate}");
                return candidate;
            }
        }

        Debug.LogWarning("No se encontró ninguna celda cercana caminable.");
        return from; // fallback, no moverse
    }

}
