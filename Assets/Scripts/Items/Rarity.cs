using UnityEngine;

public enum Rarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary,
    Mythic
}

public static class RarityUtility
{
    public static Color GetColor(Rarity r)
    {
        switch (r)
        {
            case Rarity.Common: return new Color(0.75f, 0.75f, 0.75f);
            case Rarity.Uncommon: return new Color(0.2f, 0.85f, 0.2f);
            case Rarity.Rare: return new Color(0.2f, 0.4f, 1f);
            case Rarity.Epic: return new Color(0.7f, 0.2f, 0.85f);
            case Rarity.Legendary: return new Color(1f, 0.6f, 0.1f);
            case Rarity.Mythic: return new Color(1f, 0.2f, 0.4f);
        }
        return Color.white;
    }

    public static int GetStatBudget(Rarity r)
    {
        switch (r)
        {
            case Rarity.Common: return 1;
            case Rarity.Uncommon: return 2;
            case Rarity.Rare: return 3;
            case Rarity.Epic: return 4;
            case Rarity.Legendary: return 5;
            case Rarity.Mythic: return 6;
        }
        return 1;
    }

    public static int GetSellValueMultiplier(Rarity r)
    {
        switch (r)
        {
            case Rarity.Common: return 1;
            case Rarity.Uncommon: return 2;
            case Rarity.Rare: return 5;
            case Rarity.Epic: return 10;
            case Rarity.Legendary: return 25;
            case Rarity.Mythic: return 50;
        }
        return 1;
    }
}
