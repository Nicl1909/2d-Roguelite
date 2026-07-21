using UnityEngine;

public class SaveManager : MonoBehaviour
{

    public SaveData saveData;
    // Awake is called when the script instance is being loaded
    void Awake()
    {
        loadCharacterData();    
    }

    public void loadCharacterData()
    {
        string characterName = PlayerPrefs.GetString("CharacterName", "DefaultName");
        int characterLevel = PlayerPrefs.GetInt("CharacterLevel", 1);
        float characterHealth = PlayerPrefs.GetFloat("CharacterHealth", 100f);
        float characterMana = PlayerPrefs.GetFloat("CharacterMana", 50f);
        int characterExperience = PlayerPrefs.GetInt("CharacterExperience", 0);

        CharacterData characterData = new CharacterData
        {
            characterName = characterName,
            characterLevel = characterLevel,
            characterHealth = characterHealth,
            characterMana = characterMana,
            characterExperience = characterExperience
        };

        Debug.Log($"Loaded Character Data: Name={characterData.characterName}, Level={characterData.characterLevel}, Health={characterData.characterHealth}, Mana={characterData.characterMana}, Experience={characterData.characterExperience}");
    }

}
