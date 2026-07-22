public enum EnemyArchetype
{
    Charger,
    Shooter,
    Summoner,
    Turret
}

public interface IDamageable
{
    void TakeDamage(Damage damage);
    bool IsAlive { get; }
}
