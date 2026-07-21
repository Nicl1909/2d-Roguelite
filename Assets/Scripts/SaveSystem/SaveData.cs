using UnityEngine;

public class SaveData
{
    public void SaveCharacterData(CharacterData characterData)
    {
        PlayerPrefs.SetString("CharacterName", characterData.characterName);
        PlayerPrefs.SetInt("CharacterLevel", characterData.characterLevel);
        PlayerPrefs.SetFloat("CharacterHealth", characterData.characterHealth);
        PlayerPrefs.SetFloat("CharacterMana", characterData.characterMana);
        PlayerPrefs.SetInt("CharacterExperience", characterData.characterExperience);
        PlayerPrefs.Save();
    }
}
