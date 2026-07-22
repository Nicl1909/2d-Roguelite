using UnityEngine;

[RequireComponent(typeof(Health))]
public class EnemyController : MonoBehaviour, IDamageable
{
    public EnemyData Data;
    public EnemyData AddToSummon;
    public TargetingMode Targeting = TargetingMode.Nearest;

    public Vector2 Position => transform.position;
    public Transform CurrentTarget { get; private set; }
    public EnemyState CurrentState { get; private set; } = EnemyState.Idle;
    public EnemyState DesiredState { get; set; } = EnemyState.Idle;
    public Vector2 MoveIntent { get; set; }
    public RunRng RunRng { get; private set; }

    private IEnemyBehavior behavior;
    private Health health;
    private float attackTimer;

    void Awake()
    {
        health = GetComponent<Health>();
    }

    public bool Initialize(EnemyData data, RunRng runRng)
    {
        if (data == null)
        {
            Debug.LogError("EnemyController.Initialize called with null EnemyData.");
            return false;
        }

        Data = data;
        AddToSummon = data.summonData;
        RunRng = runRng != null
            ? runRng
            : (GameManager.Instance != null && GameManager.Instance.CurrentRun != null
                ? GameManager.Instance.CurrentRun.rng
                : new RunRng(0));

        behavior = ResolveBehavior(data.archetypeId);

        if (health != null)
        {
            health.Configure(data.maxHealth, 0.25f);
        }

        try { gameObject.tag = "Enemy"; } catch { }

        attackTimer = Mathf.Max(0f, data.attackInterval);

        if (behavior != null)
        {
            try { behavior.OnSpawned(this, data); } catch (System.Exception e) { Debug.LogError($"Behavior OnSpawned threw: {e.Message}"); }
        }
        return true;
    }

    void Update()
    {
        if (Data == null) return;
        if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameState.Dungeon) return;

        CurrentTarget = TargetingService.FindTarget(Position, Data.aggroRadius, Targeting);
        CurrentState = DesiredState;

        if (behavior != null)
        {
            try { behavior.Tick(this, Time.deltaTime); }
            catch (System.Exception e) { Debug.LogError($"Behavior Tick threw: {e.Message}"); }
        }

        if (attackTimer > 0f) attackTimer -= Time.deltaTime;
    }

    void OnDisable()
    {
        if (behavior != null)
        {
            try { behavior.OnDespawned(this); } catch { }
        }
    }

    public void TriggerAttack()
    {
        if (Data == null) return;
        if (attackTimer > 0f) return;

        CurrentTarget = TargetingService.FindTarget(Position, Data.aggroRadius, Targeting);
        if (CurrentTarget == null) return;

        Vector2 dir = TargetingService.DirectionTo(Position, CurrentTarget);
        if (dir.sqrMagnitude < 0.0001f) return;

        Damage dmg = new Damage
        {
            amount = Data.contactDamage,
            type = DamageType.Physical,
            crit = false,
            source = gameObject
        };

        FireProjectile(dir, dmg);
        attackTimer = Data.attackInterval;
    }

    private void FireProjectile(Vector2 direction, Damage damage)
    {
        if (Data == null) return;
        if (Data.projectilePrefab == null) return;
        if (direction.sqrMagnitude < 0.0001f) direction = Vector2.right;

        GameObject go;
        try
        {
            go = Object.Instantiate(Data.projectilePrefab, (Vector2)transform.position + direction, Quaternion.identity);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Projectile instantiate failed: {e.Message}");
            return;
        }
        if (go == null) return;

        Projectile p = go.GetComponent<Projectile>();
        if (p == null) p = go.AddComponent<Projectile>();
        if (p == null) return;
        p.Launch(transform.position, direction, 8f, 4f, damage);
    }

    public void TakeDamage(Damage damage)
    {
        if (health != null) health.ApplyDamage(damage);
    }

    public bool IsAlive => health != null && health.IsAlive;

    public void OnKilled(GameObject source)
    {
        if (Data != null && Data.xpReward > 0 && XPHandler.Instance != null)
        {
            try { XPHandler.Instance.Award(Data.xpReward, transform.position); }
            catch (System.Exception e) { Debug.LogError($"XPHandler.Award threw: {e.Message}"); }
        }

        if (Data != null && LootService.Instance != null)
        {
            try { LootService.Instance.TryDrop(Data, transform.position); }
            catch (System.Exception e) { Debug.LogError($"LootService.TryDrop threw: {e.Message}"); }
        }

        try { Object.Destroy(gameObject); } catch { }
    }

    private IEnemyBehavior ResolveBehavior(string id)
    {
        if (string.IsNullOrEmpty(id)) return new ChargerBehavior();
        switch (id)
        {
            case "charger": return new ChargerBehavior();
            case "shooter": return new ShooterBehavior();
            case "turret": return new TurretBehavior();
            case "summoner": return new SummonerBehavior();
            default:
                if (System.Enum.TryParse<EnemyArchetype>(id, true, out var arch))
                {
                    switch (arch)
                    {
                        case EnemyArchetype.Charger: return new ChargerBehavior();
                        case EnemyArchetype.Shooter: return new ShooterBehavior();
                        case EnemyArchetype.Summoner: return new SummonerBehavior();
                        case EnemyArchetype.Turret: return new TurretBehavior();
                    }
                }
                return new ChargerBehavior();
        }
    }
}
