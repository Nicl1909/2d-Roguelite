using System.Collections.Generic;
using UnityEngine;

public enum RoomKind
{
    Combat,
    Rest,
    Boss,
    Reward
}

[System.Serializable]
public class SpawnPoint
{
    public string id;
    public Vector2 position;
}

[System.Serializable]
public class Wave
{
    public string id;
    public int count;
    public EnemyData enemy;
    public List<SpawnPoint> points;
    public float spawnDelay;
}

[System.Serializable]
public class Room
{
    public string id;
    public RoomKind kind;
    public int roomIndex;
    public List<Wave> waves;
    public Vector2 size;
    public BossData boss;
    public List<SpawnPoint> ambientPoints;
}
