using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FreezeInTime : MonoBehaviour
{
    Vector3 velocity = Vector3.zero;
    Vector3 angularVelocity = Vector3.zero;
    Rigidbody rb;
    [SerializeField] public bool affectRigidbody = true;
    [SerializeField] public bool frozen = false;
    [SerializeField] UnityEvent OnFreeze;
    [SerializeField] UnityEvent OnUnFreeze;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (frozen) Freeze();
        else UnFreeze();
    }

    // Update is called once per frame
    void Update()
    {
        if (rb && !frozen && affectRigidbody)
        {
            velocity = rb.velocity;
            angularVelocity = rb.angularVelocity;
        }
    }

    public void Freeze()
    {
        //Debug.Log("freeze");
        frozen = true;
        OnFreeze.Invoke();
        if (rb && affectRigidbody)
        {
            rb.isKinematic = true;
        }
    }

    public void UnFreeze()
    {
        //Debug.Log("unfreeze");
        frozen = false;
        OnUnFreeze.Invoke();
        if (rb && affectRigidbody)
        {
            rb.isKinematic = false;
            rb.velocity = velocity;
            rb.angularVelocity = angularVelocity;
        }
    }
}
