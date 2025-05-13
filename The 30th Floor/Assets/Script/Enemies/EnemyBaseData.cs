using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "Combat/EnemyData")]
public class EnemyBaseData : ScriptableObject
{
    public string enemyID;
    public GameObject prefab;
    public int baseHealth;
    public int attack;

    public virtual int GetScaledHealth(int level) => baseHealth + level * 5;
    public virtual int GetScaledAttack(int level) => attack + level / 2;

}
