using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class PrefabsSpawner : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private CorridorFirstDungeonGenerator dungeonGenerator;

    [Header("Prefab Lists")]
    [SerializeField] private List<GameObject> enemyPrefabs;
    [SerializeField] private List<GameObject> itemPrefabsRooms;
    [SerializeField] private List<GameObject> itemPrefabsCorridors;

    [Header("Decoración")]
    [SerializeField] private GameObject torchPrefab;
    [SerializeField] private GameObject torchSecondaryPrefab;

    [Header("Spawn Settings")]
    [SerializeField] private int maxItems = 20;
    [SerializeField] private int maxEnemies = 3;
    [Range(0f, 1f)][SerializeField] private float spawnPercent = 0.8f;
    [SerializeField] private int clearanceRadius = 1;
    [SerializeField] private int itemSpacingRadius = 1;
    [SerializeField] private float centerExclusionRadius = 1.5f;

    private HashSet<Vector2Int> floorPositions;

    private void OnEnable()
    {
        if (dungeonGenerator != null)
            dungeonGenerator.OnDungeonGenerated += SpawnAll;
    }

    private void OnDisable()
    {
        if (dungeonGenerator != null)
            dungeonGenerator.OnDungeonGenerated -= SpawnAll;
    }

    private void SpawnAll()
    {
        Debug.Log("Total rooms: " + dungeonGenerator.Rooms.Count);
        floorPositions = dungeonGenerator.FloorPositions;
        var roomPositions = dungeonGenerator.RoomPositions;
        var occupiedPositions = new List<Vector2Int>();

        foreach (var room in dungeonGenerator.Rooms)
        {
            SpawnItemsInRoom(itemPrefabsRooms, room, maxItems, occupiedPositions);
        }
        SpawnItemsInRoom(itemPrefabsCorridors, new DungeonRoom(-1, dungeonGenerator.CorridorPositions), maxItems, occupiedPositions);

        var mainRoom = GetClusterContainingPosition(roomPositions, Vector2Int.zero);
        var validEnemyPositions = floorPositions.Except(mainRoom).ToHashSet();

        foreach (var room in dungeonGenerator.Rooms)
        {
            if (!room.Tiles.Contains(Vector2Int.zero)) // evitar sala de inicio
                SpawnEnemiesInRoom(enemyPrefabs, room, maxEnemies, occupiedPositions);

        }
        foreach (var room in dungeonGenerator.Rooms)
        {
            Debug.Log("EL CENTRO DE LA ROOM " + room.Center);
            Debug.Log("LOS ENEMIGOS DE LA ROOM " + room.SpawnedEnemies.Count);
            Debug.Log("EL ID DEL ROOM" + room.ID);

        }

        SpawnTorchesInRooms(occupiedPositions);


    }

    private void SpawnTorchesInRooms(List<Vector2Int> occupied)
    {
        foreach (var room in dungeonGenerator.Rooms)
        {
            var tiles = room.Tiles;
            Vector2Int averagePos = room.Center;

            // Antorcha principal: posición cercana al centro
            var offsetCandidates = tiles
                .Where(p => Vector2Int.Distance(p, averagePos) > 0.5f && Vector2Int.Distance(p, averagePos) <= 2.5f)
                .OrderBy(p => Random.value)
                .ToList();

            Vector2Int torchPos1 = offsetCandidates
                .FirstOrDefault(p => HasItemSpacing(p, occupied, itemSpacingRadius) && HasClearanceAround(p));

            bool fallback = false;
            if (torchPos1 == default || occupied.Contains(torchPos1))
            {
                torchPos1 = averagePos;
                fallback = true;
            }

            if (!occupied.Contains(torchPos1) || fallback)
            {
                Instantiate(torchPrefab, new Vector3(torchPos1.x, torchPos1.y, 0f), Quaternion.identity);
                occupied.Add(torchPos1);
            }

            // Antorcha secundaria: más alejada
            var sorted = tiles
                .OrderByDescending(pos => Vector2Int.Distance(pos, averagePos))
                .Where(pos => HasClearanceInsideCluster(pos, tiles))
                .OrderBy(p => Random.value)
                .ToList();

            Vector2Int torchPos2 = sorted
                .FirstOrDefault(p => HasItemSpacing(p, occupied, itemSpacingRadius) && HasClearanceAround(p));

            if (torchSecondaryPrefab != null && torchPos2 != default && !occupied.Contains(torchPos2))
            {
                Instantiate(torchSecondaryPrefab, new Vector3(torchPos2.x, torchPos2.y, 0f), Quaternion.identity);
                occupied.Add(torchPos2);
            }
        }
    }



    private void SpawnEnemiesInRoom(List<GameObject> prefabs, DungeonRoom room, int maxCount, List<Vector2Int> occupied)
    {
        if (prefabs == null || prefabs.Count == 0 || room == null)
            return;

        var clusterFiltered = ExcludeCenterTiles(room.Tiles, centerExclusionRadius);
        var positions = clusterFiltered;
        Shuffle(positions);

        // Decide aleatoriamente cuántos enemigos se van a instanciar en esta sala
        int enemiesToSpawn = Random.Range(0, maxCount + 1); // incluye 0 y maxCount
        int spawned = 0;

        foreach (var pos in positions)
        {
            if (spawned >= enemiesToSpawn) break;
            if (!HasClearanceAround(pos)) continue;
            if (!HasItemSpacing(pos, occupied, itemSpacingRadius)) continue;

            var go = Instantiate(prefabs[Random.Range(0, prefabs.Count)], new Vector3(pos.x, pos.y, 0f), Quaternion.identity);
            room.AddEnemy(go);
            occupied.Add(pos);
            spawned++;
        }

        Debug.Log($"Spawneados {spawned} enemigos en la sala {room.ID}");
    }




    private void SpawnItemsInRoom(List<GameObject> prefabs, DungeonRoom room, int maxCount, List<Vector2Int> occupied)
    {
        if (prefabs == null || prefabs.Count == 0 || room == null)
            return;

        var clusterFiltered = ExcludeCenterTiles(room.Tiles, centerExclusionRadius);
        var positions = clusterFiltered;
        Shuffle(positions);

        int spawned = 0;

        foreach (var pos in positions)
        {
            if (spawned >= maxCount) break;
            if (Random.value > spawnPercent) continue;
            if (!HasClearanceAround(pos)) continue;
            if (!HasItemSpacing(pos, occupied, itemSpacingRadius)) continue;

            var go = Instantiate(prefabs[Random.Range(0, prefabs.Count)], new Vector3(pos.x, pos.y, 0f), Quaternion.identity);
            room.AddObject(go);
            occupied.Add(pos);
            spawned++;
        }
    }

    private bool HasClearanceAround(Vector2Int pos)
    {
        for (int x = -clearanceRadius; x <= clearanceRadius; x++)
        {
            for (int y = -clearanceRadius; y <= clearanceRadius; y++)
            {
                if (!floorPositions.Contains(pos + new Vector2Int(x, y)))
                    return false;
            }
        }
        return true;
    }

    private bool HasItemSpacing(Vector2Int pos, List<Vector2Int> existing, int spacingRadius)
    {
        foreach (var other in existing)
        {
            int dx = Math.Abs(pos.x - other.x);
            int dy = Math.Abs(pos.y - other.y);
            if (Math.Max(dx, dy) <= spacingRadius)
                return false;
        }
        return true;
    }

    private HashSet<Vector2Int> GetClusterContainingPosition(HashSet<Vector2Int> allPositions, Vector2Int start)
    {
        var cluster = new HashSet<Vector2Int>();
        if (!allPositions.Contains(start)) return cluster;

        var queue = new Queue<Vector2Int>();
        queue.Enqueue(start);
        cluster.Add(start);

        while (queue.Count > 0)
        {
            var pos = queue.Dequeue();
            foreach (var dir in Direction2D.cardinalDirectionsList)
            {
                var nb = pos + dir;
                if (allPositions.Contains(nb) && !cluster.Contains(nb))
                {
                    cluster.Add(nb);
                    queue.Enqueue(nb);
                }
            }
        }
        return cluster;
    }

    private List<Vector2Int> ExcludeCenterTiles(HashSet<Vector2Int> cluster, float minDistanceFromCenter)
    {
        Vector2Int average = Vector2Int.zero;
        foreach (var p in cluster)
            average += p;
        average /= cluster.Count;

        return cluster
            .Where(p => Vector2Int.Distance(p, average) > minDistanceFromCenter)
            .ToList();
    }

    private bool HasClearanceInsideCluster(Vector2Int pos, HashSet<Vector2Int> cluster)
    {
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (!cluster.Contains(pos + new Vector2Int(x, y)))
                    return false;
            }
        }
        return true;
    }

    private void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int r = Random.Range(i, list.Count);
            var tmp = list[r];
            list[r] = list[i];
            list[i] = tmp;
        }
    }
}




