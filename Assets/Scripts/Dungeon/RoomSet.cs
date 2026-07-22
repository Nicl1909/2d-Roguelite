using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomSet", menuName = "Scriptable Objects/RoomSet")]
public class RoomSet : ScriptableObject
{
    public List<Room> rooms;
}

[CreateAssetMenu(fileName = "BossData", menuName = "Scriptable Objects/BossData")]
public class BossData : ScriptableObject
{
    public string id;
    public EnemyData enemy;
    public Vector2 spawnPosition;
    public float[] phaseThresholds;
}
