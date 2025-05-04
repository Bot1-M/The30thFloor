using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class BoardManager : MonoBehaviour
{
    public enum CellType { Floor, WallTop, WallBottom, WallLeft, WallRight, CornerBottomLeft, CornerBottomRight }

    public class CellData
    {
        public bool isWalkable;
        public bool isOccupied;
        public GameObject occupant;
        public CellType cellType;
    }

    private Tilemap tilemap;
    [SerializeField] private Tilemap overlayTilemap; // Tilemap adicional para superponer el objetivo o decoraciones
    private Dictionary<Vector2Int, CellData> gridData = new();

    [Header("Grid Settings")]
    public int width = 18;
    public int height = 8;

    [Header("Tiles")]
    public Tile[] groundTiles;
    public Tile[] topWallTiles, rigthWallTiles, leftWallTiles, bottomWallTiles;
    public Tile[] bottomCornerWallTiles;
    public Tile goalTile;
    public List<Tile> decorationTile; // Antorchas u otras decoraciones

    void Start()
    {
        tilemap = GetComponentInChildren<Tilemap>();
        GenerateBoard();
    }

    void GenerateBoard()
    {
        tilemap.ClearAllTiles();
        overlayTilemap.ClearAllTiles();
        gridData.Clear();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector2Int pos = new(x, y);
                CellData cell = CreateCellData(pos);
                gridData[pos] = cell;

                // Pintar tile base si no es WallTop (se gestiona aparte)
                if (cell.cellType != CellType.WallTop)
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), GetTileForCell(pos, cell));
                }
            }
        }

        // Superponer el objetivo visual encima de la celda (14, 6)
        Vector3Int goalPos = new Vector3Int(14, 6, 0);
        overlayTilemap.SetTile(goalPos, goalTile);
    }

    private CellData CreateCellData(Vector2Int pos)
    {
        CellType type;
        bool walkable = true;

        if (pos.y == 0 && pos.x == 0)
        {
            type = CellType.CornerBottomLeft;
            walkable = false;
        }
        else if (pos.y == 0 && pos.x == width - 1)
        {
            type = CellType.CornerBottomRight;
            walkable = false;
        }
        else if (pos.y == height - 1 && pos.x == 0)
        {
            type = CellType.WallLeft;
            walkable = false;
        }
        else if (pos.y == height - 1 && pos.x == width - 1)
        {
            type = CellType.WallRight;
            walkable = false;
        }
        else if (pos.y == height - 1)
        {
            type = CellType.WallTop;
            walkable = false;

            // Pintar pared superior
            tilemap.SetTile(new Vector3Int(pos.x, pos.y, 0), topWallTiles[Random.Range(0, topWallTiles.Length)]);

            if (decorationTile.Count > 0 && Random.Range(0, 10) < 4)
            {
                int decorationTileIndex = Random.Range(0, decorationTile.Count);
                overlayTilemap.SetTile(new Vector3Int(pos.x, pos.y, 0), decorationTile[decorationTileIndex]);
                decorationTile.RemoveAt(decorationTileIndex);
            }
        }
        else if (pos.y == 0)
        {
            type = CellType.WallBottom;
            walkable = false;
        }
        else if (pos.x == 0)
        {
            type = CellType.WallLeft;
            walkable = false;
        }
        else if (pos.x == width - 1)
        {
            type = CellType.WallRight;
            walkable = false;
        }
        else
        {
            type = CellType.Floor;
        }

        return new CellData
        {
            cellType = type,
            isWalkable = walkable,
            isOccupied = false,
            occupant = null
        };
    }

    private Tile GetTileForCell(Vector2Int pos, CellData cell)
    {
        return cell.cellType switch
        {
            CellType.Floor => groundTiles[Random.Range(0, groundTiles.Length)],
            CellType.WallTop => null,
            CellType.WallBottom => bottomWallTiles[Random.Range(0, bottomWallTiles.Length)],
            CellType.WallLeft => leftWallTiles[Random.Range(0, leftWallTiles.Length)],
            CellType.WallRight => rigthWallTiles[Random.Range(0, rigthWallTiles.Length)],
            CellType.CornerBottomLeft => bottomCornerWallTiles[0],
            CellType.CornerBottomRight => bottomCornerWallTiles[1],
            _ => null,
        };
    }

    public bool IsWalkable(Vector2Int pos) => gridData.ContainsKey(pos) && gridData[pos].isWalkable;

    public void SetOccupied(Vector2Int pos, GameObject occupant)
    {
        if (gridData.ContainsKey(pos))
        {
            gridData[pos].isOccupied = occupant != null;
            gridData[pos].occupant = occupant;
        }
    }

    public CellData GetCellData(Vector2Int pos) => gridData.ContainsKey(pos) ? gridData[pos] : null;
}