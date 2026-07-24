using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    private string filePath;
    // Awake is called when the script instance is being loaded
    void Awake()
    {
        filePath = Path.Combine(Application.persistentDataPath, "characterData.json");
        LoadData();
        Debug.Log(filePath);
        SceneManager.LoadScene("MainMenu");
    }
    public void SaveData()
    {
        string json = JsonUtility.ToJson(new SaveData(), true);
        File.WriteAllText(filePath, json);
        Debug.Log("Data Saved: " + json);
    }

    public void LoadData()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            SaveData charaterData = JsonUtility.FromJson<SaveData>(json);
        }
        else
        {
            SaveData();
        }
        
    }

}
