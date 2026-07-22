using UnityEngine;

public class GameBootManager : MonoBehaviour
{
    void Awake()
    {
        Debug.Log("Booting Game...");
    }

    void Start()
    {
        EnsureGameManagerExists();
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetState(GameState.MainMenu);
        }
    }

    private static void EnsureGameManagerExists()
    {
        if (GameManager.Instance != null) return;

        GameObject go = new GameObject("GameManager");
        go.AddComponent<GameManager>();
    }
}
