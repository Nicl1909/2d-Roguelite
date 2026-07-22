using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Attack : MonoBehaviour
{
    public WeaponData runtimeWeapon;

    public GameObject projectilePrefab;
    public Transform muzzle;

    private InputAction inputActionFire;
    private InputAction inputActionAbility;

    private bool isCharging;
    private float chargeTimer;
    private bool abilityActive;

    private Coroutine attackRoutine;

    void Start()
    {
        inputActionFire = InputSystem.actions.FindAction("Attack");
        inputActionAbility = InputSystem.actions.FindAction("Interact");
    }

    public void SetWeapon(WeaponData weapon)
    {
        runtimeWeapon = weapon;
    }

    void Update()
    {
        if (runtimeWeapon == null) return;

        bool wantsFire = inputActionFire != null && inputActionFire.IsPressed();
        bool wantsAbility = inputActionAbility != null && inputActionAbility.IsPressed();

        if (wantsAbility && !abilityActive && runtimeWeapon != null)
        {
            TriggerAbility();
        }

        if (wantsFire)
        {
            if (!isCharging)
            {
                isCharging = true;
                chargeTimer = 0f;
            }
            chargeTimer += Time.deltaTime;
        }
        else if (isCharging)
        {
            ReleaseShot();
        }
    }

    private void ReleaseShot()
    {
        if (runtimeWeapon == null) { isCharging = false; return; }
        if (attackRoutine != null) StopCoroutine(attackRoutine);
        attackRoutine = StartCoroutine(FireBurst());
    }

    private IEnumerator FireBurst()
    {
        isCharging = false;

        float charge = Mathf.Clamp01(chargeTimer / Mathf.Max(0.01f, runtimeWeapon.attackTime));
        int shots = Mathf.Max(1, Mathf.RoundToInt(1 + charge * 2));

        float cooldown = Mathf.Max(0.05f, runtimeWeapon.attackCoodown);
        float perShot = cooldown / shots;

        Transform target = TargetingService.FindTarget(transform.position, runtimeWeapon.attackRange > 0f ? runtimeWeapon.attackRange : 999f, TargetingMode.Nearest);
        for (int i = 0; i < shots; i++)
        {
            target = TargetingService.FindTarget(transform.position, runtimeWeapon.attackRange > 0f ? runtimeWeapon.attackRange : 999f, TargetingMode.Nearest);
            if (target == null) break;

            Vector2 dir = TargetingService.DirectionTo(transform.position, target);
            if (muzzle != null)
            {
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                muzzle.rotation = Quaternion.Euler(0, 0, angle);
            }

            SpawnProjectile(dir);
            yield return new WaitForSeconds(perShot);
        }
        chargeTimer = 0f;
    }

    private void TriggerAbility()
    {
        if (runtimeWeapon == null) return;
        if (attackRoutine != null) StopCoroutine(attackRoutine);
        attackRoutine = StartCoroutine(AbilityBurst());
    }

    private IEnumerator AbilityBurst()
    {
        abilityActive = true;

        for (int i = 0; i < 6; i++)
        {
            Transform target = TargetingService.FindTarget(transform.position, runtimeWeapon.attackRange * 1.25f, TargetingMode.Nearest);
            if (target == null) target = TargetingService.FindTarget(transform.position, 999f, TargetingMode.Furthest);
            if (target == null) break;
            Vector2 dir = TargetingService.DirectionTo(transform.position, target);
            SpawnProjectile(dir);
            yield return new WaitForSeconds(0.1f);
        }

        abilityActive = false;
    }

    private void SpawnProjectile(Vector2 dir)
    {
        if (dir.sqrMagnitude < 0.0001f) dir = Vector2.right;

        GameObject prefab = projectilePrefab != null
            ? projectilePrefab
            : (runtimeWeapon != null ? runtimeWeapon.projectilePrefab : null);
        if (prefab == null) return;

        Vector2 origin = muzzle != null ? (Vector2)muzzle.position : (Vector2)transform.position;
        GameObject go;
        try { go = Instantiate(prefab, origin + dir, Quaternion.identity); }
        catch (System.Exception e) { Debug.LogError($"Projectile spawn failed: {e.Message}"); return; }
        if (go == null) return;

        Projectile p = go.GetComponent<Projectile>();
        if (p == null) p = go.AddComponent<Projectile>();
        if (p == null) return;

        Damage dmg = new Damage
        {
            amount = runtimeWeapon.attackDamage,
            type = runtimeWeapon.damageType,
            crit = false,
            source = gameObject
        };

        float speed = 8f;
        float life = 4f;
        p.Launch(origin, dir, speed, life, dmg);
    }

    public bool IsBusy()
    {
        return isCharging || abilityActive;
    }

    public bool GetAttacking() => IsBusy();
}
