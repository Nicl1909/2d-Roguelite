public static class DungeonEvents
{
    public static System.Action<int> OnRoomStarted;
    public static System.Action<int> OnRoomCleared;
    public static System.Action OnBossKilled;
    public static System.Action<int> OnBossPhaseChanged;

    public static void RaiseRoomStarted(int idx) => OnRoomStarted?.Invoke(idx);
    public static void RaiseRoomCleared(int idx) => OnRoomCleared?.Invoke(idx);
    public static void RaiseBossKilled() => OnBossKilled?.Invoke();
    public static void RaiseBossPhaseChanged(int phase) => OnBossPhaseChanged?.Invoke(phase);
}
