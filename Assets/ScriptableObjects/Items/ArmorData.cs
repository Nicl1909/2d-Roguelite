using UnityEngine;

[CreateAssetMenu(fileName = "ArmorData", menuName = "Scriptable Objects/ArmorData")]
public class ArmorData : ScriptableObject
{
    public string armorName;
    public Rarity rarity;
    public ArmorSlot slot;
    public int baseValue;
    public int statBudget;
    public ModifierData[] modifierPool;
}
