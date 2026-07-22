using UnityEngine;

public interface IEnemyBehavior
{
    void Tick(EnemyController controller, float dt);
    void OnSpawned(EnemyController controller, EnemyData data);
    void OnDespawned(EnemyController controller);
}

public enum EnemyState
{
    Idle,
    Chasing,
    Attacking
}
