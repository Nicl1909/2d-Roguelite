using UnityEngine;

public static class EnemyFactory
{
    public static GameObject Spawn(EnemyData data, Vector2 position, RunRng rng)
    {
        if (data == null)
        {
            Debug.LogWarning("EnemyFactory.Spawn called with null data.");
            return null;
        }
        if (data.enemyPrefab == null)
        {
            Debug.LogWarning($"EnemyFactory.Spawn: data '{data.name}' has no enemyPrefab.");
            return null;
        }

        GameObject go;
        try
        {
            go = Object.Instantiate(data.enemyPrefab, position, Quaternion.identity);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"EnemyFactory.Spawn instantiate failed: {e.Message}");
            return null;
        }
        if (go == null) return null;

        EnemyController ec = go.GetComponent<EnemyController>();
        if (ec == null) ec = go.AddComponent<EnemyController>();
        if (ec == null)
        {
            Debug.LogError("EnemyFactory.Spawn: cannot attach EnemyController.");
            Object.Destroy(go);
            return null;
        }

        if (!ec.Initialize(data, rng))
        {
            Object.Destroy(go);
            return null;
        }
        return go;
    }
}
