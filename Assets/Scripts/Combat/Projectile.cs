using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Damage Damage;
    public float Speed = 8f;
    public float MaxLifetime = 4f;
    public Vector2 Direction = Vector2.right;

    private float spawnTime;
    private bool initialized;

    public void Launch(Vector2 origin, Vector2 direction, float speed, float lifetime, Damage payload)
    {
        transform.position = origin;
        Direction = direction.sqrMagnitude > 0.0001f ? direction.normalized : Vector2.right;
        Speed = speed > 0f ? speed : 0f;
        MaxLifetime = lifetime > 0f ? lifetime : 4f;
        Damage = payload;
        spawnTime = Time.time;
        initialized = true;
    }

    void Update()
    {
        if (!initialized) return;

        transform.position += (Vector3)(Direction * Speed * Time.deltaTime);

        if (Time.time - spawnTime >= MaxLifetime)
        {
            try { Destroy(gameObject); } catch { }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other == null) return;
        IDamageable d = other.GetComponentInParent<IDamageable>();
        if (d == null) return;
        if (Damage.source != null && other.gameObject == Damage.source) return;
        try { d.TakeDamage(Damage); } catch (System.Exception e) { Debug.LogError($"Projectile damage threw: {e.Message}"); }
        try { Destroy(gameObject); } catch { }
    }
}
