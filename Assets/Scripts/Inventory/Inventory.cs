using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance { get; private set; }

    public static System.Action<ItemInstance> OnItemAdded;
    public static System.Action<ItemInstance> OnItemEquipped;
    public static System.Action<ItemInstance> OnItemUnequipped;

    public EquipSlots equipped = new EquipSlots();
    public List<ItemInstance> stash = new List<ItemInstance>();

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public bool Add(ItemInstance item)
    {
        if (item == null) return false;
        stash.Add(item);
        try { OnItemAdded?.Invoke(item); }
        catch (System.Exception e) { Debug.LogError($"OnItemAdded threw: {e.Message}"); }
        return true;
    }

    public bool Equip(ItemInstance item)
    {
        if (item == null) return false;

        EquipSlot slot = ResolveSlot(item);
        bool swapped = equipped.TrySwap(slot, item, out ItemInstance previous);
        if (previous != null) stash.Add(previous);

        try { OnItemEquipped?.Invoke(item); }
        catch (System.Exception e) { Debug.LogError($"OnItemEquipped threw: {e.Message}"); }

        if (!swapped)
        {
            return false;
        }
        return true;
    }

    public bool Unequip(ItemInstance item)
    {
        if (item == null) return false;
        EquipSlot slot = ResolveSlot(item);
        if (!equipped.Remove(slot, item)) return false;
        stash.Add(item);
        try { OnItemUnequipped?.Invoke(item); }
        catch (System.Exception e) { Debug.LogError($"OnItemUnequipped threw: {e.Message}"); }
        return true;
    }

    public int Recycle(ItemInstance item)
    {
        if (item == null) return 0;
        if (!stash.Remove(item)) return 0;
        int value = LootService.Instance != null
            ? LootService.Instance.Recycle(item)
            : item.SellValue();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.AwardMetaXp(value);
        }
        return value;
    }

    public static EquipSlot ResolveSlot(ItemInstance item)
    {
        if (item == null) return EquipSlot.None;
        if (item.AsWeapon != null) return EquipSlot.Weapon;
        if (item.AsArmor != null)
        {
            switch (item.AsArmor.slot)
            {
                case ArmorSlot.Head: return EquipSlot.Head;
                case ArmorSlot.Chest: return EquipSlot.Chest;
                case ArmorSlot.Legs: return EquipSlot.Legs;
                case ArmorSlot.Accessory: return EquipSlot.Accessory;
            }
        }
        return EquipSlot.None;
    }
}

public enum EquipSlot
{
    None,
    Weapon,
    Head,
    Chest,
    Legs,
    Accessory
}

public class EquipSlots
{
    private readonly Dictionary<EquipSlot, ItemInstance> _slots = new Dictionary<EquipSlot, ItemInstance>();

    public ItemInstance Get(EquipSlot slot)
    {
        if (slot == EquipSlot.None) return null;
        _slots.TryGetValue(slot, out ItemInstance item);
        return item;
    }

    public bool TrySwap(EquipSlot slot, ItemInstance next, out ItemInstance previous)
    {
        previous = null;
        if (slot == EquipSlot.None || next == null) return false;

        _slots.TryGetValue(slot, out previous);
        _slots[slot] = next;
        return true;
    }

    public bool Remove(EquipSlot slot, ItemInstance item)
    {
        if (slot == EquipSlot.None) return false;
        if (!_slots.TryGetValue(slot, out ItemInstance current)) return false;
        if (item != null && current != item) return false;
        _slots.Remove(slot);
        return true;
    }

    public IEnumerable<ItemInstance> All() => _slots.Values;
}
