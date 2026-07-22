using UnityEngine;

public class SummonerBehavior : IEnemyBehavior
{
    private float spawnTimer;

    public void OnSpawned(EnemyController controller, EnemyData data)
    {
        spawnTimer = controller.Data.attackInterval * 2f;
    }

    public void OnDespawned(EnemyController controller) { }

    public void Tick(EnemyController controller, float dt)
    {
        Transform target = controller.CurrentTarget;
        if (target == null) { controller.DesiredState = EnemyState.Idle; return; }

        controller.DesiredState = EnemyState.Chasing;
        controller.MoveIntent = -TargetingService.DirectionTo(controller.Position, target) * 0.6f;

        spawnTimer -= dt;
        if (spawnTimer <= 0f)
        {
            SpawnAdd(controller);
            spawnTimer = controller.Data.attackInterval;
        }
    }

    private void SpawnAdd(EnemyController controller)
    {
        EnemyData add = controller.AddToSummon;
        if (add == null || add.enemyPrefab == null) return;

        float a = controller.RunRng.Range(0f, Mathf.PI * 2f);
        Vector2 pos = controller.Position + new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * 0.5f;
        EnemyFactory.Spawn(add, pos, controller.RunRng);
    }
}
