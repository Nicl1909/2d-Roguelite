using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D rigidbody;
    public Attack attack;

    public float speed = 5f;

    private bool canDash = true;
    private bool isDashing;
    public float dashingPower = 10f;
    public float dashingTime = 0.2f;
    public float dashingCooldown = 1f;

    private InputAction inputActionMove;
    private InputAction inputActionDash;

    private Vector2 lastMoveVector;

    public Animator animator;

    void Start()
    {
        inputActionMove = InputSystem.actions.FindAction("Move");
        inputActionDash = InputSystem.actions.FindAction("Sprint");
        if (inputActionMove == null) Debug.LogError("InputSystem action 'Move' not found.");
        if (inputActionDash == null) Debug.LogError("InputSystem action 'Sprint' not found.");
        ApplyEquippedWeapon();
    }

    void Update()
    {
        if (attack != null && attack.IsBusy())
        {
            return;
        }

        bool dashInput = inputActionDash != null && inputActionDash.IsPressed();
        if (isDashing) return;

        if (dashInput && canDash)
        {
            StartCoroutine(Dash());
        }
    }

    void FixedUpdate()
    {
        if (rigidbody == null) return;
        if (isDashing) return;
        if (attack != null && attack.IsBusy())
        {
            rigidbody.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 moveVector = inputActionMove != null ? inputActionMove.ReadValue<Vector2>() : Vector2.zero;
        if (moveVector.sqrMagnitude > 1f) moveVector = moveVector.normalized;
        if (moveVector.sqrMagnitude > 0.0001f)
        {
            lastMoveVector = moveVector;
            if (animator != null) animator.SetBool("isRunning", true);
        }
        else
        {
            if (animator != null) animator.SetBool("isRunning", false);
        }
        rigidbody.linearVelocity = moveVector * speed;
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        if (rigidbody != null)
        {
            Vector2 dir = lastMoveVector == Vector2.zero ? Vector2.up : lastMoveVector.normalized;
            rigidbody.linearVelocity = dir * dashingPower;
        }
        yield return new WaitForSeconds(dashingTime);
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }

    public void ApplyEquippedWeapon()
    {
        WeaponData weapon = null;
        if (Inventory.Instance != null)
        {
            ItemInstance eq = Inventory.Instance.equipped.Get(EquipSlot.Weapon);
            if (eq != null) weapon = eq.AsWeapon;
        }

        if (weapon == null && attack != null)
        {
            weapon = attack.runtimeWeapon;
        }

        if (attack != null)
        {
            attack.SetWeapon(weapon);
        }
    }
}
