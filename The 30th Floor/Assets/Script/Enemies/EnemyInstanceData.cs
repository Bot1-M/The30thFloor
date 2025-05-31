using UnityEngine;

[System.Serializable]
public class EnemyInstanceData
{
    public EnemyBaseData enemyData;
    public Vector2Int gridPosition;
    public int quantity;

    public int currentHealth;
    public bool isAlive;
    public int attack;

    public EnemyInstanceData(EnemyBaseData data, int level)
    {
        enemyData = data;
        currentHealth = data.GetScaledHealth(level);
        attack = data.GetScaledAttack(level);
        isAlive = true;
        gridPosition = Vector2Int.zero;
    }

    public void SetGridPosition(Vector2Int pos)
    {
        gridPosition = pos;
    }
}
