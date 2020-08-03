using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    const float spd = 50f;
    const float force = 10f;
    const float maxLifeTime = 15f;
    Rigidbody rb;
    int playerLayer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerLayer = LayerMask.NameToLayer("Player");
        StartCoroutine("DeathTimer");
    }

    private void Update()
    {
        rb.velocity = transform.parent.forward * spd;
    }

    IEnumerator DeathTimer()
    {
        yield return new WaitForSeconds(maxLifeTime);
        Destroy(transform.parent.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer != playerLayer)
        {
            Rigidbody otherRb = collision.gameObject.GetComponentInChildren<Rigidbody>();
            if (otherRb)
            {
                otherRb.AddForceAtPosition(transform.parent.forward * force, transform.position);
            }
            Destroy(transform.parent.gameObject);
        }
    }
}
