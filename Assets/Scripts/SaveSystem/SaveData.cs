using System;
using UnityEngine;
using System.IO;
using System.Text.Json;
using NUnit.Framework;
[Serializable]
public class SaveData
{
    public string characterName = "Timon";
    public int characterLevel = 3;
    public float characterHealth = 10;
    public float characterMana = 10;
    public int characterExperience = 0;
    public string characterClass = "Unclassified";

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

        Debug.Log("Saved under" + filePath);
    }
    }

>>>>>>> d503fd4d74e71c3972951458102da79a0acfb787
}

