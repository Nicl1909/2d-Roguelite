using System.Collections.Generic;
using UnityEngine;

public class MetaUpgradeService : MonoBehaviour
{
    public static MetaUpgradeService Instance { get; private set; }

    public static System.Action<UpgradeData, int> OnUpgradePurchased;

    private RunModifierApplier applier;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        applier = GetComponent<RunModifierApplier>();
        if (applier == null) applier = gameObject.AddComponent<RunModifierApplier>();
    }

    public bool Purchase(UpgradeData upgrade)
    {
        if (upgrade == null) return false;
        if (GameManager.Instance == null) return false;
        CharacterData character = GameManager.Instance.GetCharacterSnapshot();
        if (character == null) return false;

        int currentLvl = character.GetUpgradeLevel(upgrade.id);
        int cost = upgrade.CostForLevel(currentLvl);
        if (character.metaXp < cost) return false;

        if (!GameManager.Instance.SpendMetaXp(cost)) return false;
        character.SetUpgradeLevel(upgrade.id, currentLvl + 1);

        try { OnUpgradePurchased?.Invoke(upgrade, currentLvl + 1); }
        catch (System.Exception e) { Debug.LogError($"OnUpgradePurchased threw: {e.Message}"); }
        return true;
    }

    public int PreviewCost(UpgradeData upgrade)
    {
        if (upgrade == null || GameManager.Instance == null) return 0;
        CharacterData character = GameManager.Instance.GetCharacterSnapshot();
        if (character == null) return 0;
        return upgrade.CostForLevel(character.GetUpgradeLevel(upgrade.id));
    }

    public RunModifierApplier Applier => applier;
}

public class RunModifierApplier : MonoBehaviour
{
    private readonly Dictionary<ModifierStat, float> _additive = new Dictionary<ModifierStat, float>();
    private readonly Dictionary<ModifierStat, float> _multiplicative = new Dictionary<ModifierStat, float>();

    public void ResetForRun()
    {
        _additive.Clear();
        _multiplicative.Clear();

        if (GameManager.Instance == null) return;
        CharacterData character = GameManager.Instance.GetCharacterSnapshot();
        if (character == null) return;

        MetaUpgradeCatalog.BonusesForCharacter(character, _additive, _multiplicative);
    }

    public float GetStat(ModifierStat stat, float baseline)
    {
        float add = 0f;
        float mult = 1f;
        _additive.TryGetValue(stat, out add);
        _multiplicative.TryGetValue(stat, out mult);
        return (baseline + add) * mult;
    }

    public int GetIntStat(ModifierStat stat, int baseline)
    {
        float add = 0f;
        float mult = 1f;
        _additive.TryGetValue(stat, out add);
        _multiplicative.TryGetValue(stat, out mult);
        return Mathf.Max(0, Mathf.RoundToInt((baseline + add) * mult));
    }
}

public static class MetaUpgradeCatalog
{
    public static void BonusesForCharacter(CharacterData character, Dictionary<ModifierStat, float> addOut, Dictionary<ModifierStat, float> multOut)
    {
        if (character == null || character.upgrades == null) return;
        for (int i = 0; i < character.upgrades.Count; i++)
        {
            MetaUpgrade u = character.upgrades[i];
            if (u == null || string.IsNullOrEmpty(u.id)) continue;

            ModifierStat stat = ModifierStat.Damage;
            if (System.Enum.TryParse(u.id, out ModifierStat parsed)) stat = parsed;

            float bonus = MetaModifierLoader.GetDefaultBonus(stat, u.level);
            if (bonus > 0f)
            {
                if (!addOut.ContainsKey(stat)) addOut[stat] = 0f;
                addOut[stat] += bonus;
            }
            else if (bonus < 0f)
            {
                if (!multOut.ContainsKey(stat)) multOut[stat] = 1f;
                multOut[stat] *= 1f + bonus;
            }
        }
    }
}
