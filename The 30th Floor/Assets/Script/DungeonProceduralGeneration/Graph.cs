using System;
using System.Collections.Generic;
using UnityEngine;

public class Graph
{
    private static List<Vector2Int> neighbors4directions = new List<Vector2Int>
    {
        new Vector2Int(0, 1),   // Up
        new Vector2Int(1, 0),   // Right
        new Vector2Int(0, -1),  // Down
        new Vector2Int(-1, 0)   // Left
    };

    private static List<Vector2Int> neighbors8directions = new List<Vector2Int>
    {
        new Vector2Int(0, 1),   // Up
        new Vector2Int(1, 0),   // Right
        new Vector2Int(0, -1),  // Down
        new Vector2Int(-1, 0),  // Left
        new Vector2Int(1, 1),   // Up-Right
        new Vector2Int(1, -1),  // Down-Right
        new Vector2Int(-1, 1),   // Up-Left
        new Vector2Int(-1, -1), // Down-Left
    };

    List<Vector2Int> graph;

    public Graph(IEnumerable<Vector2Int> vertices)
    {
        graph = new List<Vector2Int>(vertices);
    }

    public List<Vector2Int> GetNeighbors4Directions(Vector2Int vertex)
    {
        return GetNeighbors(vertex, neighbors4directions);
    }

    public List<Vector2Int> GetNeighbors8Directions(Vector2Int vertex)
    {
        return GetNeighbors(vertex, neighbors8directions);
    }

    private List<Vector2Int> GetNeighbors(Vector2Int startPosition, List<Vector2Int> neighborsList)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();

        foreach (var neighborDirection in neighborsList)
        {
            Vector2Int potencialNeighbor = startPosition + neighborDirection;
            if (graph.Contains(potencialNeighbor))
            {
                neighbors.Add(potencialNeighbor);
            }
        }
        return neighbors;
    }
}
