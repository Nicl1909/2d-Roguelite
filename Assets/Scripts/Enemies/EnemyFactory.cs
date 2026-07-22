using UnityEngine;

public static class EnemyFactory
{
    public static GameObject Spawn(EnemyData data, Vector2 position, RunRng rng)
    {
        if (data == null || data.enemyPrefab == null)
        {
            Debug.LogWarning("EnemyFactory.Spawn called without prefab.");
            return null;
        }

        GameObject go = Object.Instantiate(data.enemyPrefab, position, Quaternion.identity);
        EnemyController ec = go.GetComponent<EnemyController>();
        if (ec == null) ec = go.AddComponent<EnemyController>();
        ec.Initialize(data, rng);
        return go;
    }
}
