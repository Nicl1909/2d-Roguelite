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
        string json = JsonSerializer.Serialize(characterData, new JsonSerializerOptions
        {
            WriteIndented = true
        });
        
        File.WriteAllText(filePath, json);
        
        Debug.Log("Saved under"  + filePath);
    }

}

