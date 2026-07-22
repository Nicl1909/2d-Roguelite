using System;
using System.IO;
using System.Text.Json;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private string filePath;

    void Awake()
    {
        try
        {
            filePath = Path.Combine(Application.persistentDataPath, "characterData.json");
            Debug.Log($"SaveManager ready. Path: {filePath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"SaveManager.Awake failed: {e.Message}");
            filePath = null;
        }
    }

    public bool IsReady => !string.IsNullOrEmpty(filePath);

    public CharacterData LoadCharacterData()
    {
        if (!IsReady) return null;

        if (!File.Exists(filePath))
        {
            return null;
        }

        string json;
        try
        {
            json = File.ReadAllText(filePath);
        }
        catch (Exception e)
        {
            Debug.LogError($"Save load IO error ({filePath}): {e.Message}");
            return null;
        }

        if (string.IsNullOrWhiteSpace(json)) return null;

        CharacterData characterData = null;
        try
        {
            characterData = JsonSerializer.Deserialize<CharacterData>(json);
        }
        catch (Exception e)
        {
            Debug.LogError($"Save parse error ({filePath}): {e.Message}");
            return null;
        }

        if (characterData != null)
        {
            Debug.Log($"Loaded Character Data: Name={characterData.characterName}, Level={characterData.characterLevel}, Health={characterData.characterHealth}, Mana={characterData.characterMana}, Experience={characterData.characterExperience}");
        }
        return characterData;
    }

    public bool SaveCharacterData(CharacterData characterData)
    {
        if (!IsReady) return false;
        if (characterData == null) return false;

        string json;
        try
        {
            json = JsonSerializer.Serialize(characterData, new JsonSerializerOptions
            {
                WriteIndented = true
            });
        }
        catch (Exception e)
        {
            Debug.LogError($"Save serialize failed: {e.Message}");
            return false;
        }

        try
        {
            string dir = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            File.WriteAllText(filePath, json);
            Debug.Log($"Saved under {filePath}");
            return true;
        }
        catch (UnauthorizedAccessException e)
        {
            Debug.LogError($"Save denied: no write access to {filePath} ({e.Message})");
            return false;
        }
        catch (IOException e)
        {
            Debug.LogError($"Save IO error: {e.Message}");
            return false;
        }
        catch (Exception e)
        {
            Debug.LogError($"Save failed: {e.Message}");
            return false;
        }
    }

    public bool HasSave()
    {
        if (!IsReady) return false;
        try { return File.Exists(filePath); }
        catch { return false; }
    }

    public bool DeleteSave()
    {
        if (!IsReady) return false;
        try
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                Debug.Log($"Deleted save at {filePath}");
            }
            return true;
        }
        catch (UnauthorizedAccessException e)
        {
            Debug.LogError($"Delete denied: {e.Message}");
            return false;
        }
        catch (IOException e)
        {
            Debug.LogError($"Delete IO error: {e.Message}");
            return false;
        }
    }
}
