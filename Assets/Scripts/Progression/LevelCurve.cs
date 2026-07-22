using UnityEngine;

[CreateAssetMenu(fileName = "LevelCurve", menuName = "Scriptable Objects/LevelCurve")]
public class LevelCurve : ScriptableObject
{
    public int baseXpForLevel = 100;
    public float xpGrowth = 1.5f;

    public int XpForNextLevel(int currentLevel)
    {
        if (currentLevel <= 0) return baseXpForLevel;
        return Mathf.RoundToInt(baseXpForLevel * Mathf.Pow(xpGrowth, currentLevel - 1));
    }

    public int LevelFromXp(int totalXp)
    {
        if (totalXp <= 0) return 1;
        int lvl = 1;
        int accrued = 0;
        while (true)
        {
            int need = XpForNextLevel(lvl);
            if (accrued + need > totalXp) return lvl;
            accrued += need;
            lvl++;
            if (lvl > 1000) return lvl;
        }
    }
}
