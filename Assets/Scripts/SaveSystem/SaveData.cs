using UnityEngine;
using System.IO;
using System.Text.Json;

public class SaveData
{
    
    private string filePath;
    public SaveData()
    {
        filePath = Path.Combine(Application.persistentDataPath, "characterData.json");
        
    }

    public void SaveCharacterData(CharacterData characterData)
    {
        string json = JsonSerializer.Serialize(characterData, new JsonSerializerOptions
        {
            WriteIndented = true
        });
        
        File.WriteAllText(filePath, json);
        
        Debug.Log("Saved under"  + filePath);
    }

    public CharacterData LoadCharacterData()
    {
        if (!File.Exists(filePath))
        {
            Debug.LogWarning($"Save file not found at {filePath}");
            return null;
        }

        string json = File.ReadAllText(filePath);
        CharacterData characterData = JsonSerializer.Deserialize<CharacterData>(json);
        
        Debug.Log($"Loaded Character Data: Name={characterData.characterName}, Level={characterData.characterLevel}, Health={characterData.characterHealth}, Mana={characterData.characterMana}, Experience={characterData.characterExperience}");
        
        return characterData;
    }
}


