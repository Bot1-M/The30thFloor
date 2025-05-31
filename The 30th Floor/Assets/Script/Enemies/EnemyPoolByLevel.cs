using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyPoolByLevel", menuName = "Combat/EnemyPoolByLevel")]
public class EnemyPoolByLevel : ScriptableObject
{
    [System.Serializable]
    public class EnemyEntry
    {
        public EnemyBaseData enemyData;
        public int minLevelToAppear = 1;
    }

    public List<EnemyEntry> enemies;

    public List<EnemyBaseData> GetEnemiesForLevel(int level)
    {
        List<EnemyBaseData> result = new();
        foreach (var entry in enemies)
        {
            if (level >= entry.minLevelToAppear && entry.enemyData != null)
                result.Add(entry.enemyData);
        }
        return result;
    }
}
