using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterData
{
    public string characterName;
    public int characterLevel;
    public float characterHealth;
    public float characterMana;
    public int characterExperience;
    public int metaXp;

    public List<MetaUpgrade> upgrades = new List<MetaUpgrade>();

    public int GetUpgradeLevel(string id)
    {
        if (string.IsNullOrEmpty(id) || upgrades == null) return 0;
        for (int i = 0; i < upgrades.Count; i++)
        {
            if (upgrades[i] != null && upgrades[i].id == id) return upgrades[i].level;
        }
        return 0;
    }

    public void SetUpgradeLevel(string id, int level)
    {
        if (string.IsNullOrEmpty(id)) return;
        if (upgrades == null) upgrades = new List<MetaUpgrade>();
        for (int i = 0; i < upgrades.Count; i++)
        {
            if (upgrades[i] != null && upgrades[i].id == id)
            {
                upgrades[i].level = Mathf.Max(0, level);
                return;
            }
        }
        upgrades.Add(new MetaUpgrade { id = id, level = Mathf.Max(0, level) });
    }
}

[System.Serializable]
public class MetaUpgrade
{
    public string id;
    public int level;
}
