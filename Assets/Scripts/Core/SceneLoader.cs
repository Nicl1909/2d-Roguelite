using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    void OnEnable()
    {
        GameEvents.OnStateChanged += HandleStateChanged;
    }

    void OnDisable()
    {
        GameEvents.OnStateChanged -= HandleStateChanged;
    }

    private void HandleStateChanged(GameState from, GameState to)
    {
        switch (to)
        {
            case GameState.MainMenu:
                LoadScene("MainMenu");
                break;
            case GameState.Dungeon:
                LoadScene("Dungeon");
                break;
        }
    }

    private static void LoadScene(string sceneName)
    {
        if (SceneManager.GetActiveScene().name == sceneName) return;
        SceneManager.LoadScene(sceneName);
    }

    public void GoToMainMenu()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetState(GameState.MainMenu);
        }
    }

    public void GoToDungeon()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ContinueOrStartNewRun(FallbackSeed());
        }
    }

    private static int FallbackSeed()
    {
        return (int)(System.DateTime.UtcNow.Ticks % int.MaxValue);
    }
}
