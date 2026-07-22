using System.Collections.Generic;
using UnityEngine;

public class LootService : MonoBehaviour
{
    public static LootService Instance { get; private set; }

    public static System.Action<ItemInstance> OnItemDropped;
    public static System.Action<ItemInstance> OnItemRecycled;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public bool TryDrop(EnemyData source, Vector3 origin)
    {
        return TryRoll(source != null ? source.dropTable : null, origin);
    }

    public bool TryRoll(DropTable table, Vector3 origin)
    {
        if (table == null) return false;
        if (GameManager.Instance == null || GameManager.Instance.CurrentRun == null) return false;

        RunRng rng = GameManager.Instance.CurrentRun.rng;
        ItemInstance item = RollInstance(table, rng);
        if (item == null) return false;

        try
        {
            OnItemDropped?.Invoke(item);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"OnItemDropped subscriber threw: {e.Message}");
        }
        return true;
    }

    public ItemInstance RollInstance(DropTable table, RunRng rng)
    {
        if (table == null || table.entries == null || table.entries.Length == 0) return null;
        if (rng == null) rng = new RunRng(0);

        int total = (int)Mathf.Round(table.nothingWeight * 1000f);
        for (int i = 0; i < table.entries.Length; i++)
        {
            if (table.entries[i] != null && table.entries[i].weight > 0f)
                total += (int)Mathf.Round(table.entries[i].weight * 1000f);
        }
        if (total <= 0) return null;

        int pick = rng.Range(0, total);

        int cum = (int)Mathf.Round(table.nothingWeight * 1000f);
        if (pick < cum) return null;

        for (int i = 0; i < table.entries.Length; i++)
        {
            DropTable.Entry e = table.entries[i];
            if (e == null || e.item == null || e.weight <= 0f) continue;
            int w = (int)Mathf.Round(e.weight * 1000f);
            if (pick < cum + w)
            {
                return BuildInstance(e.item, e.rarity, rng);
            }
            cum += w;
        }
        return null;
    }

    public ItemInstance BuildInstance(ScriptableObject source, Rarity forcedRarity, RunRng rng)
    {
        if (source == null) return null;

        ItemKind kind = ItemKind.Weapon;
        ModifierData[] pool = null;
        int valueHint = 10;
        int budget = 1;
        string name = source.name;

        if (source is WeaponData wd)
        {
            kind = ItemKind.Weapon;
            pool = wd.modifierPool;
            valueHint = wd.baseValue;
            budget = wd.statBudget;
            name = wd.weaponName;
        }
        else if (source is ArmorData ad)
        {
            kind = ItemKind.Armor;
            pool = ad.modifierPool;
            valueHint = ad.baseValue;
            budget = ad.statBudget;
            name = ad.armorName;
        }

        Rarity rarity = forcedRarity != 0 ? forcedRarity : Rarity.Common;
        int rarityBudget = RarityUtility.GetStatBudget(rarity);
        if (rarityBudget > budget) budget = rarityBudget;

        return new ItemInstance
        {
            id = source.GetInstanceID().ToString() + "_" + (rng != null ? rng.Next() : 0),
            kind = kind,
            rarity = rarity,
            displayName = name,
            source = source,
            modifiers = ModifierGenerator.Roll(pool, budget, rng),
        };
    }

    public int Recycle(ItemInstance item)
    {
        if (item == null) return 0;
        int value = item.SellValue();
        try { OnItemRecycled?.Invoke(item); }
        catch (System.Exception e) { Debug.LogError($"OnItemRecycled threw: {e.Message}"); }
        return value;
    }

    public IReadOnlyList<ItemInstance> ActiveDrops { get; } = new List<ItemInstance>();
}
