using System.Collections.Generic;
using UnityEngine;

public class ItemInstance
{
    public string id;
    public ItemKind kind;
    public Rarity rarity;
    public string displayName;

    public ScriptableObject source;

    public WeaponData AsWeapon => source as WeaponData;
    public ArmorData AsArmor => source as ArmorData;

    public List<ModifierRoll> modifiers = new List<ModifierRoll>();

    public int SellValue()
    {
        int baseValue = 0;
        if (AsWeapon != null) baseValue = AsWeapon.baseValue;
        else if (AsArmor != null) baseValue = AsArmor.baseValue;

        int mult = RarityUtility.GetSellValueMultiplier(rarity);
        int modMult = modifiers.Count;
        return baseValue * mult + modMult * 5;
    }
}

public class ModifierRoll
{
    public ModifierStat stat;
    public float value;
}
