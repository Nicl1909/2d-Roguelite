using UnityEngine;
using System.Collections;

public class Attack : MonoBehaviour
{
    public bool isAttacking;
    public bool canAttack = true;
    public float attackTime;
    public float attackCoodown;

    public Transform transform;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0) && canAttack)
        {
            StartCoroutine(Attacking());
        }
    }

    public void Rotation(float rot)
    {
        transform.eulerAngles = new Vector3(0,0,rot);
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
}
