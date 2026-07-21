using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D rigidbody;
    private float horizontal;
    private float vertical;
    public float speed;

    private bool canDash = true;
    private bool isDashing;
    public float dashingPower;
    public float dashingTime;
    public float dashingCooldown;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isDashing)
        {
            return;
        }
        horizontal = Input.GetAxisRaw("Horizontal") * speed;
        vertical = Input.GetAxisRaw("Vertical") * speed;
        if(Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }
    }

    //Runs 50 times per second
    void FixedUpdate()
    {
        if (isDashing)
        {
            return;
        }
        rigidbody.linearVelocity = new Vector2(horizontal, vertical);
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        rigidbody.linearVelocity = new Vector2(dashingPower * (horizontal / (Mathf.Abs(horizontal) + Mathf.Abs(vertical))), dashingPower * (vertical / (Mathf.Abs(horizontal) + Mathf.Abs(vertical))));
        yield return new WaitForSeconds(dashingTime);
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }
}
