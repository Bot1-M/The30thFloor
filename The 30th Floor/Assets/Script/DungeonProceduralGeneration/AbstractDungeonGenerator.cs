using UnityEngine;

public abstract class AbstractDungeonGenerator : MonoBehaviour
{

    [SerializeField]
    protected TilemapVisualizer tilemapVisualizer;

    [SerializeField]
    protected Vector2Int startPosition = Vector2Int.zero;

    public void GenerateDungeon()
    {
        if (tilemapVisualizer == null)
        {
            Debug.LogWarning("TilemapVisualizer ya destruido, no se puede generar dungeon.");
            return;
        }

        tilemapVisualizer.Clear();
        RunProceduralGeneration();
    }


    protected abstract void RunProceduralGeneration();

    
}
