using UnityEngine;
using System.Collections;

public class Attack : MonoBehaviour
{
    public Transform transform;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Rotation(float rot)
    {
        transform.eulerAngles = new Vector3(0,0,rot);
    }
}
