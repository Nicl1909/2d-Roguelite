using UnityEngine;

public class LootService : MonoBehaviour
{
    public static LootService Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void TryDrop(EnemyData source, Vector3 origin)
    {
    }
}
