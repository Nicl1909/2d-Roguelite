using UnityEngine;

public class RoomDirector : MonoBehaviour
{
    public RoomSet roomSet;
    public DifficultyCurve difficulty;
    public bool endAfterBoss = true;

    private WaveRunner runner;
    private bool bossSpawned;

    void OnEnable()
    {
        GameEvents.OnStateChanged += OnStateChanged;
        DungeonEvents.OnBossKilled += HandleBossKilled;
    }

    void OnDisable()
    {
        GameEvents.OnStateChanged -= OnStateChanged;
        DungeonEvents.OnBossKilled -= HandleBossKilled;
    }

    void Awake()
    {
        EnsureRunner();
    }

    private void EnsureRunner()
    {
        if (runner == null) runner = GetComponent<WaveRunner>();
        if (runner == null) runner = gameObject.AddComponent<WaveRunner>();
    }

    private void OnStateChanged(GameState from, GameState to)
    {
        if (to != GameState.Dungeon) return;
        StartRoomForCurrentRun();
    }

    private void HandleBossKilled()
    {
        if (GameManager.Instance == null) return;
        if (!endAfterBoss) { FinishRoom(true); return; }
        FinishRoom(false);
        GameManager.Instance.SetState(GameState.Reward);
    }

    void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.CurrentState != GameState.Dungeon) return;
        EnsureRunner();
        if (bossSpawned) return;
        if (runner != null && runner.IsRunning && runner.IsCleared)
        {
            FinishRoom(true);
        }
    }

    private void StartRoomForCurrentRun()
    {
        EnsureRunner();
        if (GameManager.Instance == null || GameManager.Instance.CurrentRun == null) return;
        if (roomSet == null || roomSet.rooms == null || roomSet.rooms.Count == 0) return;

        int idx = Mathf.Clamp(GameManager.Instance.CurrentRun.roomIndex, 0, roomSet.rooms.Count - 1);
        Room room = roomSet.rooms[idx];
        bossSpawned = room.kind == RoomKind.Boss;

        try { DungeonEvents.RaiseRoomStarted(room.roomIndex); }
        catch (System.Exception e) { Debug.LogError($"RaiseRoomStarted threw: {e.Message}"); }

        runner.StartRoom(room, GameManager.Instance.CurrentRun.rng, difficulty);
    }

    private void FinishRoom(bool advance)
    {
        if (GameManager.Instance == null) return;
        try
        {
            int idx = GameManager.Instance.CurrentRun != null ? GameManager.Instance.CurrentRun.roomIndex : -1;
            DungeonEvents.RaiseRoomCleared(idx);
        }
        catch (System.Exception e) { Debug.LogError($"RaiseRoomCleared threw: {e.Message}"); }

        if (advance)
        {
            GameManager.Instance.AdvanceRoom();
            GameManager.Instance.SetState(GameState.Dungeon);
        }
    }
}
