using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public Attack attack;
    private float horizontal;
    private float vertical;
    public float direction;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
        if(vertical != 0 || horizontal != 0)
        {
            direction = 90 * vertical / (Mathf.Abs(horizontal) + Mathf.Abs(vertical));
            if (horizontal < 0.0F)
            {
                direction -= 90;
                if (vertical == 0.0F)
                {
                    direction -= 90;
                }
                else if (vertical > 0)
                {
                    direction += 180;
                }
            }
        }
        attack.Rotation(direction);
    }
}
