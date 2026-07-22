using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeData", menuName = "Scriptable Objects/UpgradeData")]
public class UpgradeData : ScriptableObject
{
    public string id;
    public string displayName;
    public ModifierStat stat;
    public float valuePerLevel;
    public int costXpBase = 50;
    public float costXpGrowth = 1.6f;

    public int CostForLevel(int level)
    {
        if (level < 0) level = 0;
        return Mathf.RoundToInt(costXpBase * Mathf.Pow(costXpGrowth, level));
    }
}

public enum MetaResource
{
    MetaXp
}

public static class MetaModifierLoader
{
    public static readonly float[] DefaultGrowth = new float[] { 0f, 0.5f, 1.2f, 2.0f, 3.0f, 4.5f };

    public static float GetDefaultBonus(ModifierStat stat, int level)
    {
        if (level < 0 || level >= DefaultGrowth.Length) return 0f;
        float growth = DefaultGrowth[level];
        switch (stat)
        {
            case ModifierStat.Damage: return growth * 0.2f;
            case ModifierStat.AttackSpeed: return growth * 0.1f;
            case ModifierStat.Health: return growth * 25f;
            case ModifierStat.Mana: return growth * 15f;
            case ModifierStat.CritChance: return growth * 0.05f;
            case ModifierStat.DamageReduction: return growth * 0.03f;
            case ModifierStat.Range: return growth * 0.5f;
            case ModifierStat.ProjectileCount: return Mathf.Floor(growth * 0.4f);
        }
        return 0f;
    }
}
