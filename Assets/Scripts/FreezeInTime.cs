using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeInTime : MonoBehaviour
{
    Vector3 velocity = Vector3.zero;
    Vector3 angularVelocity = Vector3.zero;
    Rigidbody rb;
    [SerializeField] bool frozen = false;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb)
        {
            if (frozen) Freeze();
            else UnFreeze();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (rb && !frozen)
        {
            velocity = rb.velocity;
            angularVelocity = rb.angularVelocity;
        }
    }

    public void Freeze()
    {
        if (rb)
        {
            frozen = true;
            rb.isKinematic = true;
        }
    }

    public void UnFreeze()
    {
        if (rb)
        {
            frozen = false;
            rb.isKinematic = false;
            rb.velocity = velocity;
            rb.angularVelocity = angularVelocity;
        }
    }
}
