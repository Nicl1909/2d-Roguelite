using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class Attack : MonoBehaviour
{
    public bool isAttacking;
    public bool canAttack = true;
    public float attackTime;
    public float attackCoodown;

    public bool isAbilitying;
    public bool canAbility = true;
    public float abilityTime;
    public float abilityCooldown;

    
    private InputAction inputActionAttack;
    private InputAction inputActionAbility;
    public Transform transform;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inputActionAttack = InputSystem.actions.FindAction("Attack");
        inputActionAbility = InputSystem.actions.FindAction("Interact");
        transform = this.transform;
    }

    // Update is called once per frame
    void Update()
    {
        LAMouse();
        var attackInput = inputActionAttack.IsPressed();
        var abilityInput = inputActionAbility.IsPressed();
        
        if(attackInput && canAttack && !isAbilitying)
        {
            StartCoroutine(Attacking());
        }
        if (abilityInput && canAbility && !isAttacking)
        {
            StartCoroutine(Abilitying());
        }
    }

    private IEnumerator Attacking()
    {
        canAttack = false;
        isAttacking = true;
        
        yield return new WaitForSeconds(attackTime);
        isAttacking = false;
        yield return new WaitForSeconds(attackCoodown);
        canAttack = true;
    }

    private IEnumerator Abilitying()
    {
        canAbility = false;
        isAbilitying = true;

        yield return new WaitForSeconds(abilityTime);
        isAbilitying = false;
        yield return new WaitForSeconds(abilityCooldown);
        canAbility = true;
    }

    private void LAMouse()
    {
        Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = rotation;
    }

    public bool GetAttacking()
    {
        return isAttacking;
    }
}
