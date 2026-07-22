using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : ScriptableObject
{
    public string archetypeId;
    public float maxHealth;
    public float contactDamage;
    public float moveSpeed;
    public float attackInterval;
    public float aggroRadius;
    public float attackRange;
    public GameObject projectilePrefab;
    public GameObject enemyPrefab;
    public int xpReward;
    public EnemyData summonData;
    public DropTable dropTable;
}
