using UnityEngine;

public class LootInventoryBridge : MonoBehaviour
{
    void OnEnable()
    {
        LootService.OnItemDropped += HandleDrop;
    }

    void OnDisable()
    {
        LootService.OnItemDropped -= HandleDrop;
    }

    private void HandleDrop(ItemInstance item)
    {
        if (item == null) return;
        if (Inventory.Instance == null) return;
        try { Inventory.Instance.Add(item); }
        catch (System.Exception e) { Debug.LogError($"Inventory.Add threw: {e.Message}"); }
    }
}
