using UnityEngine;

[CreateAssetMenu(fileName = "DropTable", menuName = "Scriptable Objects/DropTable")]
public class DropTable : ScriptableObject
{
    [System.Serializable]
    public class Entry
    {
        public ScriptableObject item;
        public Rarity rarity;
        [Range(0f, 1f)] public float weight = 0.5f;
    }

    public Entry[] entries;
    public float nothingWeight = 0.5f;
}
