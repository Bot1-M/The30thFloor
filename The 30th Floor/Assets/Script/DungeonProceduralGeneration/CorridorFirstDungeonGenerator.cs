using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CorridorFirstDungeonGenerator : SimpleRandomWalkDungeonGenerator
{
    public HashSet<Vector2Int> FloorPositions { get; private set; }
    public List<DungeonRoom> Rooms { get; private set; } = new();
    public HashSet<Vector2Int> RoomPositions => new(Rooms.SelectMany(r => r.Tiles));
    public HashSet<Vector2Int> CorridorPositions { get; private set; }

    public event Action OnDungeonGenerated;

    [SerializeField]
    private int corridorLength = 25, corridorCount = 7;

    [SerializeField]
    [Range(0.1f, 1)]
    private float roomPercent = 1f;

    protected override void RunProceduralGeneration()
    {
        CorridorFirstGeneration();
    }

    private void CorridorFirstGeneration()
    {
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        HashSet<Vector2Int> potentialRoomPositions = new HashSet<Vector2Int>();

        List<List<Vector2Int>> corridors = CreateCorridors(floorPositions, potentialRoomPositions);
        HashSet<Vector2Int> roomPositions = CreateRooms(potentialRoomPositions);
        List<Vector2Int> deadEnds = FindAllDeadEnds(floorPositions);
        CreateRoomsAtDeadEnd(deadEnds, roomPositions);

        floorPositions.UnionWith(roomPositions);

        for (int i = 0; i < corridors.Count; i++)
        {
            corridors[i] = IncreaseCorridorSizeByThree(corridors[i]);
            floorPositions.UnionWith(corridors[i]);
        }

        Rooms = GetRoomClusters(roomPositions);
        CorridorPositions = new HashSet<Vector2Int>();
        foreach (var corridor in corridors)
        {
            CorridorPositions.UnionWith(corridor);
        }
        FloorPositions = new HashSet<Vector2Int>();
        FloorPositions.UnionWith(RoomPositions);
        FloorPositions.UnionWith(CorridorPositions);

        tilemapVisualizer.PaintFloorTiles(floorPositions);
        WallGenerator.CreateWalls(floorPositions, tilemapVisualizer);
        OnDungeonGenerated?.Invoke();
    }

    private List<Vector2Int> IncreaseCorridorSizeByThree(List<Vector2Int> corridor)
    {
        List<Vector2Int> newCorridor = new List<Vector2Int>();
        for (int i = 1; i < corridor.Count; i++)
        {
            for (int x = -1; x < 2; x++)
            {
                for (int y = -1; y < 2; y++)
                {
                    newCorridor.Add(corridor[i - 1] + new Vector2Int(x, y));
                }
            }
        }
        return newCorridor;
    }

    private void CreateRoomsAtDeadEnd(List<Vector2Int> deadEnds, HashSet<Vector2Int> roomFloors)
    {
        foreach (var position in deadEnds)
        {
            if (!roomFloors.Contains(position))
            {
                var room = RunRandomWalk(randomWalkParameters, position);
                roomFloors.UnionWith(room);
            }
        }
    }

    private List<Vector2Int> FindAllDeadEnds(HashSet<Vector2Int> floorPositions)
    {
        List<Vector2Int> deadEnds = new List<Vector2Int>();
        foreach (var position in floorPositions)
        {
            int neighboursCount = 0;
            foreach (var direction in Direction2D.cardinalDirectionsList)
            {
                if (floorPositions.Contains(position + direction))
                    neighboursCount++;
            }
            if (neighboursCount == 1)
                deadEnds.Add(position);
        }
        return deadEnds;
    }

    private HashSet<Vector2Int> CreateRooms(HashSet<Vector2Int> potentialRoomPositions)
    {
        HashSet<Vector2Int> roomPositions = new HashSet<Vector2Int>();
        int roomToCreateCount = Mathf.RoundToInt(potentialRoomPositions.Count * roomPercent);

        List<Vector2Int> roomsToCreate = potentialRoomPositions.OrderBy(x => Guid.NewGuid()).Take(roomToCreateCount).ToList();

        foreach (var roomPosition in roomsToCreate)
        {
            var roomFloor = RunRandomWalk(randomWalkParameters, roomPosition);
            roomPositions.UnionWith(roomFloor);
        }
        return roomPositions;
    }

    private List<List<Vector2Int>> CreateCorridors(HashSet<Vector2Int> floorPositions, HashSet<Vector2Int> potentialRoomPositions)
    {
        var currentPosition = startPosition;
        potentialRoomPositions.Add(currentPosition);
        List<List<Vector2Int>> corridors = new List<List<Vector2Int>>();

        for (int i = 0; i < corridorCount; i++)
        {
            var corridor = ProceduralGenerationAlgorithms.RandomWalkCorridor(currentPosition, corridorLength);
            corridors.Add(corridor);
            currentPosition = corridor[corridor.Count - 1];
            potentialRoomPositions.Add(currentPosition);
            floorPositions.UnionWith(corridor);
        }
        return corridors;
    }

    public List<DungeonRoom> GetRoomClusters(HashSet<Vector2Int> roomPositions)
    {
        List<DungeonRoom> clusters = new List<DungeonRoom>();
        HashSet<Vector2Int> unvisited = new HashSet<Vector2Int>(roomPositions);

        int id = 0;

        while (unvisited.Count > 0)
        {
            Vector2Int start = unvisited.First();
            HashSet<Vector2Int> cluster = new HashSet<Vector2Int>();
            Queue<Vector2Int> queue = new Queue<Vector2Int>();
            queue.Enqueue(start);
            cluster.Add(start);
            unvisited.Remove(start);

            while (queue.Count > 0)
            {
                Vector2Int pos = queue.Dequeue();
                foreach (var dir in Direction2D.cardinalDirectionsList)
                {
                    Vector2Int neighbor = pos + dir;
                    if (unvisited.Contains(neighbor))
                    {
                        queue.Enqueue(neighbor);
                        cluster.Add(neighbor);
                        unvisited.Remove(neighbor);
                    }
                }
            }

            clusters.Add(new DungeonRoom(id, cluster));
            id++;
        }

        return clusters;
    }

}
