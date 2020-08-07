using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
    bool isOnFloor = true;
    Vector2 axis = Vector2.zero;

    [SerializeField] float standHeight = 2f;
    [SerializeField] float crouchHeight = 1f;
    [SerializeField] float crouchSpdMult = 0.5f;
    bool crouch = false;
    public bool hidden = false;

    [SerializeField] float volumeLerp = 0.5f;
    //[SerializeField] AudioClip crouchWalkSound;
    //[SerializeField] AudioClip walkingSound;

    [SerializeField] AudioSource walkMoveAudio;
    [SerializeField] AudioSource crouchMoveAudio;

    [SerializeField] UnityEvent OnCrouch;
    [SerializeField] UnityEvent OnStand;
    [SerializeField] UnityEvent OnJump;


    CapsuleCollider cld;
    int floorLayer;
    int notPlayerLayerMask;

    private void Awake()
    {
        GameManager.player = transform;
        notPlayerLayerMask = ~LayerMask.GetMask("Player", "Field");
    }

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
        bool aux = crouch;
        crouch = Input.GetButton("Crouch");
        //walkMoveAudio.clip = crouch ? crouchWalkSound : walkingSound;
        RaycastHit hit;

        bool underObj = (Physics.Raycast(transform.position, Vector3.up, out hit, Mathf.Abs(standHeight - crouchHeight + 0.5f), notPlayerLayerMask));

        if (!crouch && aux)
            crouch = underObj;

        hidden = crouch && underObj;

        if (crouch != aux)
            if (crouch) OnCrouch.Invoke();
            else OnStand.Invoke();

        cld.height = crouch ? crouchHeight : standHeight;
        
        mouseXInput = Mathf.Lerp(mouseXInput, Input.GetAxis("Mouse X"), xRotLerp);
        mouseYInput = Mathf.Lerp(mouseYInput, Input.GetAxis("Mouse Y"), yRotLerp);

        axis.x = Input.GetAxis("Horizontal");
        axis.y = Input.GetAxis("Vertical");
        if(axis.magnitude > 1f) axis = axis.normalized;

        isOnFloor = OnFloor();

        if (jumping)
        {
            jumpTime += Time.deltaTime;
            if (isOnFloor && jumpTime > jumpCooldown)
            {
                jumping = false;
            }
        }
        else if (Input.GetButtonDown("Jump") && isOnFloor)
        {
            OnJump.Invoke();
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
        float maxSpd = movementSpd * (crouch ? crouchSpdMult : 1f);
        Vector3 spd = (transform.forward * axis.y + transform.right * axis.x) * maxSpd;

        walkMoveAudio.volume = Mathf.Lerp(walkMoveAudio.volume, (spd.magnitude / movementSpd) * (isOnFloor && !crouch ? 1f : 0f), volumeLerp);
        crouchMoveAudio.volume = Mathf.Lerp(crouchMoveAudio.volume, (spd.magnitude / movementSpd*crouchSpdMult) * (isOnFloor && crouch ? 1f : 0f), volumeLerp);

        spd.y = rb.velocity.y;
        rb.velocity = Vector3.Lerp(rb.velocity, spd, movementLerp);
    }

    bool OnFloor()
    {
        bool onFloor = false;

        Vector3 feetPos = transform.position + cld.center - transform.up * (cld.height / 2f);
        Collider[] clds = Physics.OverlapSphere(feetPos, 0.4f);
        foreach(Collider clds_i in clds)
        {
            onFloor = onFloor || clds_i.gameObject.layer == floorLayer;
        }
        return onFloor;
    }
}