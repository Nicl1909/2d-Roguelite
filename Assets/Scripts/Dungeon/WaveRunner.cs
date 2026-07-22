using System.Collections.Generic;
using UnityEngine;

public class WaveRunner : MonoBehaviour
{
    public bool IsRunning { get; private set; }
    public bool IsCleared { get; private set; }

    private Room currentRoom;
    private Wave currentWave;
    private RunRng runRng;
    private DifficultyCurve difficulty;
    private int waveIndex;
    private float waveTimer;
    private int spawnedThisWave;
    private int aliveCount;
    private readonly List<GameObject> liveInstances = new List<GameObject>();

    public void StartRoom(Room room, RunRng rng, DifficultyCurve curve)
    {
        currentRoom = room;
        runRng = rng != null ? rng : new RunRng(0);
        difficulty = curve;
        waveIndex = 0;
        waveTimer = 0f;
        spawnedThisWave = 0;
        aliveCount = 0;
        IsCleared = false;
        IsRunning = true;
        AdvanceWave();
    }

    public void Stop()
    {
        IsRunning = false;
        IsCleared = true;
        for (int i = 0; i < liveInstances.Count; i++)
        {
            if (liveInstances[i] != null) Destroy(liveInstances[i]);
        }
        liveInstances.Clear();
    }

    void Update()
    {
        if (!IsRunning || IsCleared) return;
        if (currentRoom == null) { IsCleared = true; return; }

        PruneDead();

        if (currentWave != null && currentWave.points != null && currentWave.points.Count > 0)
        {
            int target = ComputeTargetSpawn(currentWave);
            if (spawnedThisWave < target)
            {
                waveTimer -= Time.deltaTime;
                if (waveTimer <= 0f && currentWave.enemy != null)
                {
                    SpawnOne();
                }
            }
        }

        if (AllCurrentWaveResolved(out _))
        {
            if (waveIndex + 1 >= (currentRoom.waves != null ? currentRoom.waves.Count : 0))
            {
                IsCleared = true;
            }
            else
            {
                waveIndex++;
                waveTimer = 0f;
                spawnedThisWave = 0;
                AdvanceWave();
            }
        }
    }

    private void AdvanceWave()
    {
        currentWave = (currentRoom.waves != null && waveIndex < currentRoom.waves.Count)
            ? currentRoom.waves[waveIndex]
            : null;

        if (currentRoom.kind == RoomKind.Boss)
        {
            SpawnBoss();
            return;
        }

        if (currentWave == null) { IsCleared = true; return; }
        waveTimer = currentWave.spawnDelay;
        spawnedThisWave = 0;
    }

    private int ComputeTargetSpawn(Wave wave)
    {
        int baseCount = wave.count > 0 ? wave.count : 1;
        if (difficulty != null)
        {
            return difficulty.GetSpawnCount(baseCount, currentRoom.roomIndex);
        }
        return baseCount;
    }

    private void SpawnOne()
    {
        if (currentWave == null || currentWave.enemy == null) return;
        if (runRng == null) return;

        Vector2 pos;
        if (currentWave.points != null && currentWave.points.Count > 0)
        {
            int idx = runRng.Range(0, currentWave.points.Count);
            pos = currentWave.points[idx].position;
        }
        else
        {
            pos = new Vector2(runRng.Range(-5f, 5f), runRng.Range(-5f, 5f));
        }

        GameObject go = EnemyFactory.Spawn(currentWave.enemy, pos, runRng);
        if (go != null)
        {
            liveInstances.Add(go);
            aliveCount++;
            spawnedThisWave++;
        }
        waveTimer = currentWave.spawnDelay;
    }

    private void SpawnBoss()
    {
        if (currentRoom.boss == null || currentRoom.boss.enemy == null)
        {
            IsCleared = true;
            return;
        }

        EnemyData bossData = currentRoom.boss.enemy;
        Vector2 pos = currentRoom.boss.spawnPosition;
        GameObject go = EnemyFactory.Spawn(bossData, pos, runRng);
        if (go == null) { IsCleared = true; return; }

        BossController bc = go.GetComponent<BossController>();
        if (bc == null) bc = go.AddComponent<BossController>();
        if (bc != null && currentRoom.boss.phaseThresholds != null)
        {
            bc.ConfigurePhases(currentRoom.boss.phaseThresholds, currentRoom.roomIndex);
        }

        liveInstances.Add(go);
        aliveCount++;
    }

    private void PruneDead()
    {
        if (liveInstances.Count == 0) return;
        int removed = 0;
        for (int i = liveInstances.Count - 1; i >= 0; i--)
        {
            GameObject go = liveInstances[i];
            bool dead = go == null;
            if (!dead)
            {
                Health hp = go.GetComponent<Health>();
                if (hp != null && !hp.IsAlive) dead = true;
            }
            if (dead)
            {
                liveInstances.RemoveAt(i);
                aliveCount--;
                removed++;
            }
        }
    }

    private bool AllCurrentWaveResolved(out int remaining)
    {
        remaining = aliveCount;
        if (currentWave == null) return true;

        int target = ComputeTargetSpawn(currentWave);
        if (spawnedThisWave < target) return false;

        bool anyAlive = false;
        for (int i = 0; i < liveInstances.Count; i++)
        {
            GameObject go = liveInstances[i];
            if (go == null) continue;
            Health hp = go.GetComponent<Health>();
            if (hp == null) continue;
            if (hp.IsAlive) { anyAlive = true; break; }
        }
        return !anyAlive;
    }
}
