using System.IO;
using System.Text.Json;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private string filePath;

    void Awake()
    {
        filePath = Path.Combine(Application.persistentDataPath, "characterData.json");
        Debug.Log($"SaveManager ready. Path: {filePath}");
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

        if (characterData != null)
        {
            Debug.Log($"Loaded Character Data: Name={characterData.characterName}, Level={characterData.characterLevel}, Health={characterData.characterHealth}, Mana={characterData.characterMana}, Experience={characterData.characterExperience}");
        }
        return characterData;
    }

    public void SaveCharacterData(CharacterData characterData)
    {
        if (characterData == null) return;

        string json = JsonSerializer.Serialize(characterData, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        File.WriteAllText(filePath, json);
        Debug.Log($"Saved under {filePath}");
    }

    public bool HasSave() => File.Exists(filePath);

    public void DeleteSave()
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Debug.Log($"Deleted save at {filePath}");
        }
    }
}
