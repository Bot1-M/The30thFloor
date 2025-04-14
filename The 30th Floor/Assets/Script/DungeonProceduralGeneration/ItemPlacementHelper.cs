using System.Collections.Generic;
using UnityEngine;

public class ItemPlacementHelper
{

    Dictionary<PlacementeType, HashSet<Vector2Int>> tileByType = new Dictionary<PlacementeType, HashSet<Vector2Int>>();

    HashSet<Vector2Int> roomFloorNoCorridor;

    public ItemPlacementHelper(HashSet<Vector2Int> roomFloor, HashSet<Vector2Int> roomFloorNoCorridor)
    {
        Graph graph = new Graph(roomFloor);
        this.roomFloorNoCorridor = roomFloorNoCorridor;
        foreach(var position in roomFloorNoCorridor)
        {
            int neighborsCount8Dir = graph.GetNeighbors8Directions(position).Count;
            PlacementeType type = neighborsCount8Dir < 8 ? PlacementeType.NearWall : PlacementeType.OpenSpace;
            if (!tileByType.ContainsKey(type) == false)
            {
                tileByType[type] = new HashSet<Vector2Int>();
            }

            if(type == PlacementeType.NearWall&& graph.GetNeighbors4Directions(position).Count == 0)
            {
                tileByType[PlacementeType.OpenSpace].Add(position);
            }
            else
            {
                tileByType[type].Add(position);
            }
            {
                tileByType[type].Add(position);
            }
        }

    }

}

public enum PlacementeType
{
    OpenSpace,
    NearWall
}
