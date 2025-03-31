using System.Collections.Generic;
using UnityEngine;

public class ProceduralGenerationAlgorithms : MonoBehaviour
{
    

    public static HashSet<Vector2Int> SimpleRandomWalk(Vector2Int start, int walkLength)
    {
        HashSet<Vector2Int> path = new HashSet<Vector2Int>();

        path.Add(start);
        var previousPosition = start;

        for (int i = 0; i < walkLength; i++)
        {
            var newPosition = previousPosition + Direction2D.GetRandomCardinalDirection();
            path.Add(newPosition);
            previousPosition = newPosition;
        }

        return path;
    }
}


public static class Direction2D
{
    private static Vector2Int Up = new Vector2Int(0, 1);
    private static Vector2Int Down = new Vector2Int(0, -1);
    private static Vector2Int Left = new Vector2Int(-1, 0);
    private static Vector2Int Right = new Vector2Int(1, 0);

    public static List<Vector2Int> cardinalDirectionList = new List<Vector2Int> { Up, Down, Left, Right };

    public static Vector2Int GetRandomCardinalDirection()
    {
        int randomIndex = Random.Range(0, cardinalDirectionList.Count);
        return cardinalDirectionList[randomIndex];
    }
}