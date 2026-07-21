using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public Attack attack;
    public float direction;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        attack.Rotation(direction);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
