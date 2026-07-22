using UnityEngine;

public class TurretBehavior : IEnemyBehavior
{
    public void OnSpawned(EnemyController controller, EnemyData data) { }
    public void OnDespawned(EnemyController controller) { }

    public void Tick(EnemyController controller, float dt)
    {
        Transform target = controller.CurrentTarget;
        if (target == null) { controller.DesiredState = EnemyState.Idle; return; }

        float dist = Vector2.Distance(controller.Position, target.position);
        if (dist > controller.Data.attackRange)
        {
            controller.DesiredState = EnemyState.Idle;
            return;
        }

        controller.DesiredState = EnemyState.Attacking;
        controller.TriggerAttack();
    }
}
