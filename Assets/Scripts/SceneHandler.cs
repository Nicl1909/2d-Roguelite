using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void fromMaintoGameScene()
    {
        SceneManager.LoadScene("SampleScene");
    }

    
}
