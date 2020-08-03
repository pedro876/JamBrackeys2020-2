using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Camera")]
    Transform yRotObj;
    Transform xRotObj;
    [SerializeField] [Tooltip("Horizontal camera rotation speed")] float yRotSpd = 1f;
    [SerializeField] [Tooltip("Vertical camera rotation speed")] float xRotSpd = 1f;
    [SerializeField] [Range(0f, 1f)] float yRotLerp = 0.9f;
    [SerializeField] [Range(0f, 1f)] float xRotLerp = 0.9f;
    float mouseXInput = 0f;
    float mouseYInput = 0f;

    [Header("Movement")]
    Rigidbody rb;
    [SerializeField] float movementSpd = 15f;
    [SerializeField] [Range(0f, 1f)] float movementLerp = 0.8f;
    [SerializeField] float jumpForce = 10f;
    [SerializeField] float jumpCooldown = 0.2f;
    [SerializeField] float jumpTime = 0f;
    bool jumping = false;
    Vector2 axis = Vector2.zero;

    CapsuleCollider cld;
    int floorLayer;

    private void Start()
    {
        xRotObj = transform;
        yRotObj = Camera.main.transform;
        rb = GetComponent<Rigidbody>();
        cld = GetComponent<CapsuleCollider>();
        floorLayer = LayerMask.NameToLayer("Floor");
    }

    private void Update()
    {
        mouseXInput = Mathf.Lerp(mouseXInput, Input.GetAxis("Mouse X"), xRotLerp);
        mouseYInput = Mathf.Lerp(mouseYInput, Input.GetAxis("Mouse Y"), yRotLerp);

        axis.x = Input.GetAxis("Horizontal");
        axis.y = Input.GetAxis("Vertical");
        if(axis.magnitude > 1f) axis = axis.normalized;

        if (jumping)
        {
            jumpTime += Time.deltaTime;
            if (OnFloor() && jumpTime > jumpCooldown)
            {
                jumping = false;
            }
        }
        else if (Input.GetButtonDown("Jump") && OnFloor())
        {
            jumping = true;
            jumpTime = 0f;
            rb.AddForce(transform.up * jumpForce, ForceMode.VelocityChange);
        }
    }

    private void FixedUpdate()
    {
        xRotObj.transform.RotateAround(xRotObj.transform.position, transform.up, mouseXInput * xRotSpd /** 100f * Time.fixedDeltaTime*/);
        float degrees = yRotObj.transform.rotation.eulerAngles.x  -mouseYInput * yRotSpd;
        if(degrees < 270f && degrees > 90f) degrees = degrees > 180f ? 270f : 90f;
        yRotObj.transform.localRotation = Quaternion.Euler(degrees, 0f, 0f);
        Vector3 spd = (transform.forward * axis.y + transform.right * axis.x) * movementSpd;
        spd.y = rb.velocity.y;
        rb.velocity = Vector3.Lerp(rb.velocity, spd, movementLerp);
    }

    bool OnFloor()
    {
        bool onFloor = false;

        Vector3 feetPos = transform.position + cld.center - transform.up * (cld.height / 2f);
        Collider[] clds = Physics.OverlapSphere(feetPos, 0.2f);
        foreach(Collider clds_i in clds)
        {
            onFloor = onFloor || clds_i.gameObject.layer == floorLayer;
        }
        return onFloor;
    }
}