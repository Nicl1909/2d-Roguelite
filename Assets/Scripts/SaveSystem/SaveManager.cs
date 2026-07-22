using UnityEngine;

public class SaveManager : MonoBehaviour
{

    public SaveData saveData;
    // Awake is called when the script instance is being loaded
    void Awake()
    {
        Debug.Log("SaveManager loaded.");
    }

    public CharacterData GetCharacterData()
    {
        return saveData.LoadCharacterData();
    }

    public void SaveCharacter(CharacterData data)
    {
        saveData.SaveCharacterData(data);
    }

}
