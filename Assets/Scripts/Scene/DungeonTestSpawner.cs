using UnityEngine;

public class DungeonTestSpawner : MonoBehaviour
{
    public EnemyData testEnemy;

    private int spawned;

    void Start()
    {
        if (GameManager.Instance == null || GameManager.Instance.CurrentRun == null)
        {
            return;
        }

        if (testEnemy == null)
        {
            testEnemy = ScriptableObject.CreateInstance<EnemyData>();
            testEnemy.archetypeId = EnemyArchetype.Charger.ToString();
            testEnemy.maxHealth = 30f;
            testEnemy.contactDamage = 8f;
            testEnemy.moveSpeed = 2f;
            testEnemy.attackInterval = 1.5f;
            testEnemy.aggroRadius = 8f;
            testEnemy.attackRange = 1f;
            testEnemy.xpReward = 25;
            testEnemy.enemyPrefab = BootstrapEnemyPrefab();
            testEnemy.summonData = null;
            testEnemy.projectilePrefab = null;
        }

        RunRng rng = GameManager.Instance.CurrentRun.rng;

        for (int i = 0; i < 3; i++)
        {
            float angle = (Mathf.PI * 2f) * ((float)i / 3f);
            Vector3 pos = new Vector3(Mathf.Cos(angle) * 4f, Mathf.Sin(angle) * 4f, 0f);
            EnemyFactory.Spawn(testEnemy, pos, rng);
            spawned++;
        }
    }

    private GameObject BootstrapEnemyPrefab()
    {
        GameObject e = new GameObject("BootstrapEnemyPrefab");
        DontDestroyOnLoad(e);
        e.hideFlags = HideFlags.HideAndDontSave;
        e.SetActive(false);

        SpriteRenderer sr = e.AddComponent<SpriteRenderer>();
        sr.sprite = RuntimeSpriteFactory.Circle();
        sr.color = new Color(0.85f, 0.25f, 0.25f, 1f);

        BoxCollider2D col = e.AddComponent<BoxCollider2D>();
        col.size = new Vector2(0.5f, 0.5f);
        col.isTrigger = true;

        e.AddComponent<Health>();
        e.AddComponent<EnemyContactDamage>();
        e.AddComponent<EnemyController>();

        return e;
    }
}
