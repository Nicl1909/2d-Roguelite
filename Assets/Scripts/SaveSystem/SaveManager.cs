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
        string characterName = saveData.characterName;
        int characterLevel = saveData.characterLevel;
        float characterHealth = saveData.characterHealth;
        float characterMana = saveData.characterMana;
        int characterExperience = saveData.characterExperience;

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
