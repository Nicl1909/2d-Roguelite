using UnityEngine;

public enum TargetingMode
{
    Nearest,
    Furthest,
    LowestHealth
}

public static class TargetingService
{
    public static Transform FindTarget(Vector2 from, float maxRadius, TargetingMode mode)
    {
        if (maxRadius <= 0f) return null;

        float bestValue = mode == TargetingMode.Nearest ? float.MaxValue : float.MinValue;
        Transform best = null;

        GameObject[] candidates;
        try
        {
            candidates = GameObject.FindGameObjectsWithTag("Player");
        }
        catch (UnityException)
        {
            return null;
        }
        if (candidates == null) return null;

        for (int i = 0; i < candidates.Length; i++)
        {
            GameObject go = candidates[i];
            if (go == null) continue;

            float dist = Vector2.Distance(from, go.transform.position);
            if (dist > maxRadius) continue;

            bool take = false;
            switch (mode)
            {
                case TargetingMode.Nearest:
                    take = dist < bestValue; bestValue = dist; break;
                case TargetingMode.Furthest:
                    take = dist > bestValue; bestValue = dist; break;
                case TargetingMode.LowestHealth:
                    float v = EstimateValue(go, dist);
                    take = v > bestValue; bestValue = v; break;
            }
            if (take) best = go.transform;
        }
        return best;
    }

    public static Vector2 DirectionTo(Vector2 from, Transform target)
    {
        if (target == null) return Vector2.zero;
        Vector2 d = (Vector2)target.position - from;
        return d.sqrMagnitude > 0.0001f ? d.normalized : Vector2.zero;
    }

    public static bool InAggro(Vector2 from, Transform target, float radius)
    {
        if (target == null) return false;
        if (radius <= 0f) return false;
        return Vector2.Distance(from, target.position) <= radius;
    }

    private static float EstimateValue(GameObject go, float dist)
    {
        Health hp = go.GetComponent<Health>();
        if (hp == null) return -dist;
        return hp.MaxHealth - hp.CurrentHealth * 10f - dist;
    }
}
