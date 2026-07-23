using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D rigidbody;
    public Attack attack;
    
    public float speed;

    private bool canDash = true;
    private bool isDashing;
    public float dashingPower;
    public float dashingTime;
    public float dashingCooldown;

    private InputAction inputActionMove;
    private InputAction inputActionDash;
    
    private Vector2 lastMoveVector;
    
    public Animator animator;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inputActionMove = InputSystem.actions.FindAction("Move");
        inputActionDash = InputSystem.actions.FindAction("Sprint");
    }

    // Update is called once per frame
    void Update()
    {
        var dashInput = inputActionDash.IsPressed();
        
        if (isDashing || attack.GetAttacking())
        {
            return;
        }
        
        if(dashInput && canDash)
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
        if(attack.GetAttacking())
        {
            rigidbody.linearVelocity = new Vector2(0, 0);
            return;
        }
        
        var moveVector = inputActionMove.ReadValue<Vector2>();
        
        if (moveVector.x != 0 || moveVector.y != 0)
        {
            lastMoveVector = moveVector;
            animator.SetBool("isRunning", true);
        }
        else
        {
            animator.SetBool("isRunning", false);
        }
        moveAnimation(moveVector);
        rigidbody.linearVelocity = moveVector * (Time.deltaTime * speed);
    }

    private void moveAnimation(Vector2 direction)
    {
        animator.SetBool("isRight", false);
        animator.SetBool("isLeft", false);
        animator.SetBool("isUp", false);
        animator.SetBool("isDown", false);
        float horizontalPercentage = Mathf.Abs(direction.x) / (Mathf.Abs(direction.x) + Mathf.Abs(direction.y));
        float verticalPercentage = Mathf.Abs(direction.y) / (Mathf.Abs(direction.x) + Mathf.Abs(direction.y));
        if (horizontalPercentage >= verticalPercentage)
        {
            if (direction.x > 0)
            {
                animator.SetBool("isRight", true);
            }
            else
            {
                animator.SetBool("isLeft", true);
            }
        }
        else
        {
            if (direction.y > 0)
            {
                animator.SetBool("isUp", true);
            }
            else
            {
                animator.SetBool("isDown", true);
            }
        }
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        rigidbody.linearVelocity = lastMoveVector * dashingPower;
        yield return new WaitForSeconds(dashingTime);
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }
}
