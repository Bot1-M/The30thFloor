using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour
{
    public enum CellType
    {
        Floor,
        WallTop,
        WallBottom,
        WallLeft,
        WallRight,
        CornerBottomLeft,
        CornerBottomRight
    }

    public class CellData
    {
        public bool isWalkable;
        public bool isOccupied;
        public GameObject occupant;
    }

    private Tilemap tilemap;
    [SerializeField] private Tilemap overlayTilemap;

    private CellData[,] boardData;

    [Header("Grid Settings")]
    public int width = 18;
    public int height = 8;

    [Header("Tiles")]
    public Tile[] groundTiles;
    public Tile[] topWallTiles, rigthWallTiles, leftWallTiles, bottomWallTiles;
    public Tile[] bottomCornerWallTiles;

    public List<Tile> decorationTile;

    public bool IsReady { get; private set; }

    public event System.Action OnBoardReady;

    public void Init()
    {
        tilemap = GetComponentInChildren<Tilemap>();
        GenerateBoard();
    }

    void GenerateBoard()
    {
        tilemap.ClearAllTiles();
        overlayTilemap.ClearAllTiles();
        boardData = new CellData[width, height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Tile tile = null;
                bool walkable = true;

                if (x == 0 && y == 0)
                {
                    tile = bottomCornerWallTiles[0];
                    walkable = false;
                }
                else if (x == width - 1 && y == 0)
                {
                    tile = bottomCornerWallTiles[1];
                    walkable = false;
                }
                else if (y == height - 1 && x == 0)
                {
                    tile = leftWallTiles[Random.Range(0, leftWallTiles.Length)];
                    walkable = false;
                }
                else if (y == height - 1 && x == width - 1)
                {
                    tile = rigthWallTiles[Random.Range(0, rigthWallTiles.Length)];
                    walkable = false;
                }
                else if (y == height - 1)
                {
                    tile = topWallTiles[Random.Range(0, topWallTiles.Length)];
                    walkable = false;

                    if (decorationTile.Count > 0 && Random.Range(0, 10) < 4)
                    {
                        int decorationTileIndex = Random.Range(0, decorationTile.Count);
                        overlayTilemap.SetTile(new Vector3Int(x, y, 0), decorationTile[decorationTileIndex]);
                        decorationTile.RemoveAt(decorationTileIndex);
                    }
                }
                else if (y == 0)
                {
                    tile = bottomWallTiles[Random.Range(0, bottomWallTiles.Length)];
                    walkable = false;
                }
                else if (x == 0)
                {
                    tile = leftWallTiles[Random.Range(0, leftWallTiles.Length)];
                    walkable = false;
                }
                else if (x == width - 1)
                {
                    tile = rigthWallTiles[Random.Range(0, rigthWallTiles.Length)];
                    walkable = false;
                }
                else
                {
                    tile = groundTiles[Random.Range(0, groundTiles.Length)];
                    walkable = true;
                }

                boardData[x, y] = new CellData
                {
                    isWalkable = walkable,
                    isOccupied = false,
                    occupant = null
                };

                tilemap.SetTile(new Vector3Int(x, y, 0), tile);
            }
        }

        OnBoardReady?.Invoke();
        IsReady = true;
    }

    public CellData GetCellData(Vector2Int pos)
    {
        if (IsInsideBoard(pos))
            return boardData[pos.x, pos.y];

        Debug.LogWarning($"GetCellData: posición fuera de límites {pos}");
        return null;
    }

    public bool IsWalkable(Vector2Int pos)
    {
        return IsInsideBoard(pos) && boardData[pos.x, pos.y].isWalkable;
    }

    public void SetOccupied(Vector2Int pos, GameObject occupant)
    {
        if (IsInsideBoard(pos))
        {
            boardData[pos.x, pos.y].isOccupied = occupant != null;
            boardData[pos.x, pos.y].occupant = occupant;
        }
    }

    public List<Vector2Int> GetFreeCellsInRange(int minX, int maxX, int minY, int maxY)
    {
        List<Vector2Int> result = new();

        for (int x = maxX; x >= minX; x--)
        {
            for (int y = minY; y <= maxY; y++)
            {
                if (!IsInsideBoard(x, y)) continue;

                var cell = boardData[x, y];
                if (cell.isWalkable && !cell.isOccupied)
                {
                    result.Add(new Vector2Int(x, y));
                }
            }
        }

        return result;
    }

    private bool IsInsideBoard(Vector2Int pos) => IsInsideBoard(pos.x, pos.y);

    private bool IsInsideBoard(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }

    public Tilemap GetTilemap()
    {
        return tilemap;
    }

    public void ShowOverlay(List<Vector2Int> cells, Tile tile)
    {
        foreach (var cell in cells)
        {
            overlayTilemap.SetTile(new Vector3Int(cell.x, cell.y, 0), tile);
        }
    }

    public void ClearOverlay()
    {
        overlayTilemap.ClearAllTiles();
    }

    public Vector3 GridToWorldCenter(Vector2Int gridPosition)
    {
        return tilemap.GetCellCenterWorld(new Vector3Int(gridPosition.x, gridPosition.y, 0));
    }


    public void SpawnExitAt(Vector2Int position, GameObject exitPrefab)
    {
        if (!IsInsideBoard(position))
        {
            Debug.LogWarning("Posición fuera del tablero: " + position);
            return;
        }

        var cellData = GetCellData(position);
        if (cellData == null || !cellData.isWalkable || cellData.isOccupied)
        {
            Debug.LogWarning("La celda no está disponible para colocar la salida.");
            return;
        }

        Vector3 worldPos = GridToWorldCenter(position);
        GameObject exit = Instantiate(exitPrefab, worldPos, Quaternion.identity);
        SetOccupied(position, exit);
    }



}
