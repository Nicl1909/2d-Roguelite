using UnityEngine;

public enum ModifierStat
{
    Damage,
    AttackSpeed,
    Health,
    Mana,
    CritChance,
    DamageReduction,
    Range,
    ProjectileCount
}

[CreateAssetMenu(fileName = "ModifierData", menuName = "Scriptable Objects/Modifier")]
public class ModifierData : ScriptableObject
{
    public string id;
    public ModifierSlot slot;
    public ModifierStat stat;
    public float minValue;
    public float maxValue;
    public float weight = 1f;
}
