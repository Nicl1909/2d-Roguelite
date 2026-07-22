using UnityEngine;

[CreateAssetMenu(fileName = "DifficultyCurve", menuName = "Scriptable Objects/DifficultyCurve")]
public class DifficultyCurve : ScriptableObject
{
    public AnimationCurve hpMultiplier;
    public AnimationCurve damageMultiplier;
    public AnimationCurve spawnCountMultiplier;

    public float GetHpMultiplier(int roomIndex)
    {
        if (hpMultiplier == null || hpMultiplier.length == 0) return 1f + roomIndex * 0.2f;
        return Mathf.Max(0.1f, hpMultiplier.Evaluate(roomIndex));
    }

    public float GetDamageMultiplier(int roomIndex)
    {
        if (damageMultiplier == null || damageMultiplier.length == 0) return 1f + roomIndex * 0.15f;
        return Mathf.Max(0.1f, damageMultiplier.Evaluate(roomIndex));
    }

    public int GetSpawnCount(int baseCount, int roomIndex)
    {
        float t = 1f;
        if (spawnCountMultiplier != null && spawnCountMultiplier.length > 0)
        {
            t = spawnCountMultiplier.Evaluate(roomIndex);
            t = Mathf.Max(0.1f, t);
        }
        else
        {
            t = 1f + roomIndex * 0.25f;
        }
        return Mathf.Max(1, Mathf.RoundToInt(baseCount * t));
    }
}
