using System;

public static class GameEvents
{
    public static event Action<GameState, GameState> OnStateChanged;

    public static void RaiseStateChanged(GameState from, GameState to)
    {
        OnStateChanged?.Invoke(from, to);
    }
}
