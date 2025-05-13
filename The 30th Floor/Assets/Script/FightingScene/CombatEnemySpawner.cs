using System.Collections.Generic;
using UnityEngine;

public class CombatEnemySpawner : MonoBehaviour
{
    [SerializeField] private BoardManager board;
    [SerializeField] private EnemyPoolByLevel enemyPool;

    void Start()
    {
        if (board == null)
            board = FindFirstObjectByType<BoardManager>();

        if (board != null)
            board.OnBoardReady += SpawnEnemies;
    }

    void OnDestroy()
    {
        if (board != null)
            board.OnBoardReady -= SpawnEnemies;
    }

    public void SpawnEnemies()
    {
        if (enemyPool == null)
        {
            Debug.LogError("EnemyPoolByLevel no asignado en CombatEnemySpawner.");
            return;
        }

        int combatLevel = PlayerManager.Instance?.Data?.level ?? 1;

        List<Vector2Int> validPositions = board.GetFreeCellsInRange(5, 14, 1, 7);
        HashSet<Vector2Int> usedPositions = new();
        List<EnemyBaseData> candidates = enemyPool.GetEnemiesForLevel(combatLevel);

        if (candidates.Count == 0)
        {
            Debug.LogWarning("No hay enemigos válidos para el nivel " + combatLevel);
            return;
        }

        int maxEnemies = GetMaxEnemiesForLevel(combatLevel);
        int numToSpawn = Random.Range(1, maxEnemies + 1);

        for (int i = 0; i < numToSpawn; i++)
        {
            if (validPositions.Count == 0) break;

            Vector2Int position = GetFurthestPosition(validPositions, usedPositions);
            usedPositions.Add(position);
            validPositions.Remove(position);

            EnemyBaseData selected = candidates[Random.Range(0, candidates.Count)];

            if (selected.prefab == null)
            {
                Debug.LogWarning($"El prefab de {selected.enemyID} es null.");
                continue;
            }

            var instanceData = new EnemyInstanceData(selected, combatLevel);
            instanceData.SetGridPosition(position);

            GameObject go = Instantiate(selected.prefab, board.GridToWorldPosition(position), Quaternion.identity);
            board.SetOccupied(position, go);
        }
    }

    private int GetMaxEnemiesForLevel(int level)
    {
        int baseEnemies = 2;
        float multiplier = 1f;

        if (level >= 10)
            multiplier *= 1.5f;
        if (level >= 5)
            multiplier *= 1.5f;

        return Mathf.CeilToInt(baseEnemies * multiplier);
    }

    private Vector2Int GetFurthestPosition(List<Vector2Int> candidates, HashSet<Vector2Int> placed)
    {
        if (placed.Count == 0)
            return candidates[Random.Range(0, candidates.Count)];

        Vector2Int best = candidates[0];
        float bestMinDistance = float.MinValue;

        foreach (var candidate in candidates)
        {
            float minDist = float.MaxValue;
            foreach (var other in placed)
            {
                float dist = Vector2Int.Distance(candidate, other);
                if (dist < minDist)
                    minDist = dist;
            }

            if (minDist > bestMinDistance)
            {
                bestMinDistance = minDist;
                best = candidate;
            }
        }

        return best;
    }
}
