using UnityEngine;
using UnityEngine.Tilemaps;

public class DungeonSceneBootstrap : MonoBehaviour
{
    private const string PlayerTag = "Player";
    private const string EnemyTag = "Enemy";

    private static DungeonSceneBootstrap _spawned;

    public static DungeonSceneBootstrap SpawnDungeonScene(GameObject services)
    {
        if (_spawned != null) return _spawned;

        GameObject go = new GameObject("DungeonSceneBootstrap");
        DontDestroyOnLoad(go);
        DungeonSceneBootstrap inst = go.AddComponent<DungeonSceneBootstrap>();
        _spawned = inst;

        inst.BuildDungeonScene();
        return inst;
    }

    private void BuildDungeonScene()
    {
        EnsureTags();

        Camera cam = Camera.main;
        if (cam == null)
        {
            GameObject camGo = new GameObject("Main Camera");
            camGo.tag = "MainCamera";
            cam = camGo.AddComponent<Camera>();
            cam.orthographic = true;
            cam.orthographicSize = 12f;
            cam.transform.position = new Vector3(0f, 0f, -10f);
        }

        cam.gameObject.AddComponent<CameraFollow>();

        BuildGrid();
        BuildServices();
        BuildPlayer();
        BuildTestSpawner();
    }

    private static void EnsureTags()
    {
        EnsureTag("Player");
        EnsureTag("Enemy");
    }

    private static void EnsureTag(string tag)
    {
        GameObject probe = new GameObject("__tag_probe__");
        try
        {
            probe.tag = tag;
        }
        catch (UnityException)
        {
            Debug.LogWarning($"Tag '{tag}' is missing in TagManager — add it via Project Settings before play. Falling back to default Untagged.");
        }
        finally
        {
            Destroy(probe);
        }
    }

    private void BuildGrid()
    {
        GameObject grid = GameObject.Find("DungeonGrid");
        if (grid == null)
        {
            grid = new GameObject("DungeonGrid");
            grid.AddComponent<Grid>();
        }

        GameObject floorGo = new GameObject("Floor");
        floorGo.transform.SetParent(grid.transform, false);
        Tilemap floor = floorGo.AddComponent<Tilemap>();
        TilemapRenderer floorR = floorGo.AddComponent<TilemapRenderer>();
        floorR.sortingOrder = -1;
    }

    private void BuildPlayer()
    {
        GameObject existing = GameObject.Find("Player");
        if (existing != null) return;

        GameObject go = new GameObject("Player");
        try { go.tag = "Player"; } catch { }
        go.transform.position = Vector3.zero;

        Rigidbody2D rb = go.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        BoxCollider2D col = go.AddComponent<BoxCollider2D>();
        col.size = new Vector2(0.6f, 0.6f);
        col.isTrigger = false;

        GameObject sprite = new GameObject("Sprite");
        sprite.transform.SetParent(go.transform, false);
        SpriteRenderer sr = sprite.AddComponent<SpriteRenderer>();
        sr.sprite = RuntimeSpriteFactory.Square();
        sr.color = new Color(0.4f, 0.85f, 0.4f, 1f);

        Health hp = go.AddComponent<Health>();
        hp.Configure(100f, 0.25f);

        Attack attack = go.AddComponent<Attack>();
        attack.SetWeapon(BootstrapDefaultWeapon());

        go.AddComponent<PlayerController>();
    }

    private WeaponData BootstrapDefaultWeapon()
    {
        WeaponData wd = ScriptableObject.CreateInstance<WeaponData>();
        wd.weaponName = "Stub Bow";
        wd.attackDamage = 8f;
        wd.attackRange = 12f;
        wd.attackTime = 1.2f;
        wd.attackCoodown = 0.6f;
        wd.attackKnockback = 0f;
        wd.damageType = DamageType.Physical;
        wd.weaponType = WeaponType.Bow;
        wd.weaponRarity = Rarity.Common;
        wd.baseValue = 1;
        wd.statBudget = 0;
        wd.modifierPool = new ModifierData[0];
        wd.weaponIcon = null;
        wd.projectilePrefab = BootstrapProjectilePrefab();
        return wd;
    }

    private GameObject BootstrapProjectilePrefab()
    {
        GameObject p = new GameObject("BootstrapProjectilePrefab");
        DontDestroyOnLoad(p);
        p.hideFlags = HideFlags.HideAndDontSave;
        p.SetActive(false);
        CircleCollider2D col = p.AddComponent<CircleCollider2D>();
        col.radius = 0.12f;
        col.isTrigger = true;
        p.AddComponent<Projectile>();
        return p;
    }

    private void BuildServices()
    {
        GameObject services = GameObject.Find("DungeonServices");
        if (services == null)
        {
            services = new GameObject("DungeonServices");
            DontDestroyOnLoad(services);
        }

        EnsureComponent<XPHandler>(services);
        EnsureComponent<LootService>(services);
        EnsureComponent<Inventory>(services);
        EnsureComponent<LootInventoryBridge>(services);
        EnsureComponent<MetaUpgradeService>(services);
        EnsureComponent<WaveRunner>(services);
        EnsureComponent<RoomDirector>(services);
        EnsureComponent<HudBootstrap>(services);
    }

    private static T EnsureComponent<T>(GameObject host) where T : MonoBehaviour
    {
        T component = host.GetComponent<T>();
        if (component == null) component = host.AddComponent<T>();
        return component;
    }

    private void BuildTestSpawner()
    {
        DungeonTestSpawner spawner = FindFirstObjectByType<DungeonTestSpawner>();
        if (spawner == null)
        {
            GameObject go = new GameObject("DungeonTestSpawnerRoot");
            spawner = go.AddComponent<DungeonTestSpawner>();
        }
    }

    private void OnDestroy()
    {
        if (_spawned == this) _spawned = null;
    }
}
