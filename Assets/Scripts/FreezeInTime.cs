using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FreezeInTime : MonoBehaviour
{
    Vector3 velocity = Vector3.zero;
    Vector3 angularVelocity = Vector3.zero;
    Rigidbody rb;
    [SerializeField] bool frozen = false;
    [SerializeField] UnityEvent OnFreeze;
    [SerializeField] UnityEvent OnUnFreeze;
    
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
        frozen = true;
        if(OnFreeze != null) OnFreeze.Invoke();
        if (rb)
        {
            rb.isKinematic = true;
        }

    }

    public void UnFreeze()
    {
        frozen = false;
        if(OnUnFreeze != null) OnUnFreeze.Invoke();
        if (rb)
        {
            rb.isKinematic = false;
            rb.velocity = velocity;
            rb.angularVelocity = angularVelocity;
        }
    }
}
