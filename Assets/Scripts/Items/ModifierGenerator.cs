using System.Collections.Generic;
using UnityEngine;

public static class ModifierGenerator
{
    public static List<ModifierRoll> Roll(ModifierData[] pool, int budget, RunRng rng)
    {
        var result = new List<ModifierRoll>(budget);
        if (pool == null || pool.Length == 0 || budget <= 0) return result;
        if (rng == null) rng = new RunRng(0);

        for (int i = 0; i < budget; i++)
        {
            ModifierData chosen = WeightedPick(pool, rng);
            if (chosen == null) break;

            float t = rng.Value();
            float value = Mathf.Lerp(chosen.minValue, chosen.maxValue, t);

            result.Add(new ModifierRoll { stat = chosen.stat, value = value });
        }
        return result;
    }

    private static ModifierData WeightedPick(ModifierData[] pool, RunRng rng)
    {
        float total = 0f;
        for (int i = 0; i < pool.Length; i++)
        {
            if (pool[i] != null && pool[i].weight > 0f) total += pool[i].weight;
        }
        if (total <= 0f) return null;

        float pick = rng.Range(0f, total);
        float cum = 0f;
        for (int i = 0; i < pool.Length; i++)
        {
            ModifierData m = pool[i];
            if (m == null || m.weight <= 0f) continue;
            cum += m.weight;
            if (pick <= cum) return m;
        }
        return pool[pool.Length - 1];
    }
}
