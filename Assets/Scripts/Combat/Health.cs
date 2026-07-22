using UnityEngine;

public class Health : MonoBehaviour, IDamageable
{
    [SerializeField] private float currentHealth;
    [SerializeField] private float maxHealth;
    [SerializeField] private float invulnSeconds = 0.25f;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;

    private float invulnUntil;
    private bool dead;

    void Awake()
    {
        if (maxHealth <= 0) maxHealth = 1;
        if (currentHealth <= 0 || currentHealth > maxHealth) currentHealth = maxHealth;
    }

    public void Configure(float max, float invuln = 0.25f)
    {
        maxHealth = max > 0 ? max : 1;
        currentHealth = maxHealth;
        invulnSeconds = invuln > 0 ? invuln : 0f;
        invulnUntil = 0f;
        dead = false;
    }

    public void TakeDamage(Damage damage)
    {
        ApplyDamage(damage);
    }

    public bool IsAlive => !dead;

    public void ApplyDamage(Damage damage)
    {
        if (dead) return;
        if (Time.time < invulnUntil) return;

        float effective = damage.type == DamageType.True ? damage.amount : damage.amount;
        if (effective < 0f) effective = 0f;

        currentHealth -= effective;
        invulnUntil = Time.time + invulnSeconds;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            dead = true;
            Die(damage.source);
        }
    }

    private void Die(GameObject source)
    {
        EnemyController ec = GetComponent<EnemyController>();
        if (ec != null)
        {
            try { ec.OnKilled(source); }
            catch (System.Exception e) { Debug.LogError($"EnemyController.OnKilled threw: {e.Message}"); }
            return;
        }

        bool isPlayer = false;
        try { isPlayer = gameObject.CompareTag("Player"); } catch { }
        if (isPlayer)
        {
            if (GameManager.Instance != null)
            {
                try { GameManager.Instance.SetState(GameState.Death); }
                catch (System.Exception e) { Debug.LogError($"SetState(Death) threw: {e.Message}"); }
            }
        }

        try { Destroy(gameObject); } catch { }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other == null) return;
        bool isPlayer = false;
        try { isPlayer = gameObject.CompareTag("Player"); } catch { }
        if (!isPlayer) return;

        EnemyContactDamage c = other.GetComponent<EnemyContactDamage>();
        if (c == null) return;

        Damage d = new Damage
        {
            amount = c.ContactDamage,
            type = DamageType.Physical,
            crit = false,
            source = gameObject
        };
        ApplyDamage(d);
    }
}
