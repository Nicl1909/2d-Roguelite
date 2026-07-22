using UnityEngine;

[CreateAssetMenu(fileName = "newWeaponData", menuName = "Scriptable Objects/WeaponData")]
public class WeaponData : ScriptableObject
{
    public string weaponName;
    public float attackDamage;
    public float attackRange;
    public float attackTime;
    public float attackCoodown;

    public float attackKnockback;
    public DamageType damageType;

    public WeaponType weaponType;
    public Rarity weaponRarity;
    public Sprite weaponIcon;

    [Header("Loot")]
    public int baseValue = 10;
    public int statBudget = 1;
    public ModifierData[] modifierPool;
}
