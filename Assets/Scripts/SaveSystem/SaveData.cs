using UnityEngine;
using System.IO;
using System.Text.Json;
using NUnit.Framework;

public class SaveData
{
    
    private string filePath;
    public SaveData()
    {
        filePath = Path.Combine(Application.persistentDataPath, "characterData.json");
        
    }

    public void SaveCharacterData(CharacterData characterData)
    {
<<<<<<< HEAD
        public string characterName;
        public int characterLevel;
        public float characterHealth;
        public float characterMana;
        public int characterExperience;
=======
        string json = JsonSerializer.Serialize(characterData, new JsonSerializerOptions
        {
            WriteIndented = true
        });
        
        File.WriteAllText(filePath, json);
        
        Debug.Log("Saved under"  + filePath);
>>>>>>> c01df8b2a38e33d1d5189006fde81f04ec2a8d8c
    }

}

