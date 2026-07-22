using UnityEngine;

public class XPService : MonoBehaviour
{
    public static XPService Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void Award(int amount, Vector3 origin)
    {
        if (amount <= 0) return;
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AwardMetaXp(amount);
        }
    }
}
