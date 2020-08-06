using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponShake : MonoBehaviour
{
    [SerializeField] float spd = 1f;
    [SerializeField] float xRange = 1f;
    [SerializeField] float yRange = 1f;
    [SerializeField] float speedMult = 0.01f;
    [SerializeField] float toRBCost = 0.2f;
    [SerializeField] [Range(0f, 1f)] float rbSpeedLerp = 0.9f;
    Vector3 rbSpeed = Vector3.zero;

    Vector3 originalPos;
    float time = 0f;
    Rigidbody rb;

    [SerializeField] float frozenAngle = 0f;
    [SerializeField] float unFrozenAngle = -65f;
    [SerializeField] float rotLerp = 0.5f;
    public bool timeIsFrozen { get; set; }


    private void Start()
    {
        originalPos = transform.localPosition;
        rb = GetComponentInParent<Rigidbody>();

        transform.localRotation = Quaternion.Euler(unFrozenAngle, 0f, 0f);
    }

    private void FixedUpdate()
    {
        rbSpeed = Vector3.Lerp(rbSpeed, rb.velocity, rbSpeedLerp);

        time = (time + Time.fixedDeltaTime) % (2f*Mathf.PI);
        transform.localPosition = originalPos;
        transform.localPosition = originalPos
            + ((Vector3.up * Mathf.Sin(time * spd) * yRange) + (Vector3.right * Mathf.Sin(time * spd) * xRange))
            * (rbSpeed.magnitude * speedMult + 1f);
        transform.position -= rbSpeed * toRBCost;

        Quaternion targetRot = Quaternion.Euler(timeIsFrozen ? frozenAngle : unFrozenAngle, 0f, 0f);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRot, rotLerp);
    }
}
