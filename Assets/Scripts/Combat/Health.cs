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
        maxHealth = max;
        currentHealth = max;
        invulnSeconds = invuln;
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

        float effective = damage.amount;
        if (damage.type == DamageType.True)
        {
            effective = damage.amount;
        }

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
            ec.OnKilled(source);
            return;
        }

        if (gameObject.CompareTag("Player"))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.SetState(GameState.Death);
            }
        }

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!gameObject.CompareTag("Player")) return;
        EnemyContactDamage c = other.GetComponent<EnemyContactDamage>();
        if (c == null) return;
        Damage d = new Damage { amount = c.ContactDamage, type = DamageType.Physical, crit = false, source = gameObject };
        ApplyDamage(d);
    }
}
