using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Representa una sala individual dentro de la mazmorra.
/// Contiene sus tiles, su centro y cualquier contenido generado.
/// </summary>
public class DungeonRoom
{
    public int ID { get; private set; }
    public HashSet<Vector2Int> Tiles { get; private set; }
    public Vector2Int Center { get; private set; }

    public List<GameObject> SpawnedObjects { get; private set; } = new();

    public List<GameObject> SpawnedEnemies { get; private set; } = new();

    public GameObject SpawnedChest { get; private set; } = new();

    public DungeonRoom(int id, HashSet<Vector2Int> tiles)
    {
        ID = id;
        Tiles = tiles;
        Center = CalculateCenter(tiles);
    }

    private Vector2Int CalculateCenter(HashSet<Vector2Int> tiles)
    {
        Vector2Int sum = Vector2Int.zero;
        foreach (var pos in tiles)
        {
            sum += pos;
        }
        return sum / tiles.Count;
    }

    public void AddObject(GameObject obj)
    {
        SpawnedObjects.Add(obj);
    }

    public void ClearContents()
    {
        foreach (var obj in SpawnedObjects)
        {
            if (obj != null) GameObject.Destroy(obj);
        }
        SpawnedObjects.Clear();
        foreach (var enemy in SpawnedEnemies)
        {
            if (enemy != null) GameObject.Destroy(enemy);
        }
        SpawnedEnemies.Clear();
    }

    public void AddEnemy(GameObject enemy)
    {
        SpawnedEnemies.Add(enemy);
        SpawnedObjects.Add(enemy);
    }

    public void SetChest(GameObject chest)
    {
        SpawnedChest = chest;
        SpawnedObjects.Add(chest);
    }
}

