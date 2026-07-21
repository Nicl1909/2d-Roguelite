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
    public WeaponRarity weaponRarity;
    public Sprite weaponIcon;


    public enum WeaponType
    {
        Sword,
        Bow,
        Staff,
        Dagger,
        Axe
    }
    public enum DamageType

    {
        Physical,
        magical,
        Fire,
        Ice,
        Lightning
        
    }

    public enum WeaponRarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary,
        Mythic
    }
}
