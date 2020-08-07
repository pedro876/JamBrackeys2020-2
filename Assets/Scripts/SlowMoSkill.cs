using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Events;

public class SlowMoSkill : MonoBehaviour
{
    [Header("Slow Motion")]
    [SerializeField] Transform cam;
    [SerializeField] Transform revolver;
    CapsuleCollider cld;
    Rigidbody rb;
    [SerializeField] [Range(0f, 1f)] float lerp = 1f;
    [SerializeField] float slowMoMaxTime = 10f;     float slowMoTime = 0f;
    [SerializeField] float regressionMaxTime = 2f;  float regressionTime = 0f;
    [SerializeField] float coolDownMaxTime = 5f;    float coolDownTime = 0f;
    [SerializeField] bool perfectRegresion = true;
    [SerializeField] UnityEvent startSkill;
    [SerializeField] UnityEvent startRewind;
    [SerializeField] UnityEvent startCooldown;
    [SerializeField] UnityEvent endCooldown;

    List<Vector3> positions;
    List<Quaternion> playerRotations;
    List<Quaternion> camRotations;
    List<Vector3> revolverPositions;
    List<Quaternion> revolverRotations;

    Vector3 lastVelocity = Vector3.zero;
    Vector3 lastAngularVelocity = Vector3.zero;

    bool isRewinding = false;
    bool slowMoActive = false;
    bool canSlowMo = true;

    [Header("Field")]
    [SerializeField] Transform field;
    [SerializeField] float maxScale = 100f;
    [SerializeField] [Range(0f, 1f)] float smoothStepEdge0 = 0f;
    [SerializeField] [Range(0f, 1f)] float smoothStepEdge1 = 1f;

    [Header("Post Processing")]
    [SerializeField] Volume volume;
    [SerializeField] float postprocessE0 = 0f;
    [SerializeField] float postprocessE1 = 1f;
    [SerializeField] float distorsion = 0.5f;
    [SerializeField] Color slowMoTint;
    [SerializeField] Color regressionTint;
    [SerializeField] float regressionE0 = 0f;
    [SerializeField] float regressionE1 = 1f;

    LensDistortion lensDistorsion;
    ColorAdjustments colorAdjustments;
    

    // Start is called before the first frame update
    void Start()
    {
        positions = new List<Vector3>();
        playerRotations = new List<Quaternion>();
        camRotations = new List<Quaternion>();
        revolverPositions = new List<Vector3>();
        revolverRotations = new List<Quaternion>();

        volume.profile.TryGet(out lensDistorsion);
        volume.profile.TryGet(out colorAdjustments);
        cld = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("UseSlowMo"))
        {
            if(!slowMoActive && canSlowMo)
            {
                positions.Clear();
                playerRotations.Clear();
                camRotations.Clear();
                revolverPositions.Clear();
                revolverRotations.Clear();
                canSlowMo = false;
                slowMoActive = true;
                slowMoTime = 0f;
                regressionTime = 0f;
                coolDownTime = 0f;
                isRewinding = false;
                startSkill.Invoke();
                lastAngularVelocity = rb.angularVelocity;
                lastVelocity = rb.velocity;
            }
            else if (slowMoActive && slowMoTime < (regressionE0 * slowMoMaxTime))
            {
                slowMoTime = regressionE0 * slowMoMaxTime;
            }
        }

        SkillTimers();

        SetFieldSize();

        PostProcess();
    }

    private void FixedUpdate()
    {
        if (slowMoActive)
        {
            positions.Add(transform.position);
            playerRotations.Add(transform.rotation);
            camRotations.Add(cam.rotation);
            revolverPositions.Add(revolver.position);
            revolverRotations.Add(revolver.rotation);
        }
        else if (isRewinding)
        {
            int index = (int)(GameManager.SmoothStep(0f, 1f, 1f - regressionTime / regressionMaxTime) * positions.Count);
            if(index >= 0 && index < positions.Count)
            {
                transform.position = Vector3.Lerp(transform.position, positions[index], lerp);
                transform.rotation = Quaternion.Lerp(transform.rotation, playerRotations[index], lerp);
                cam.rotation = Quaternion.Lerp(cam.rotation, camRotations[index], lerp);
                revolver.position = Vector3.Lerp(revolver.position, revolverPositions[index], lerp);
                revolver.rotation = Quaternion.Lerp(revolver.rotation, revolverRotations[index], lerp);
            }
        }
    }

    private void PostProcess()
    {
        float step = 0f;
        if (slowMoActive) step = GameManager.SmoothStep(postprocessE0, postprocessE1, slowMoTime / slowMoMaxTime); 
        else if (isRewinding) step = GameManager.SmoothStep(postprocessE0, postprocessE1, 1f - regressionTime / regressionMaxTime);
        lensDistorsion.intensity.value = step * distorsion;
        Color localTint = Color.Lerp(slowMoTint, regressionTint, GameManager.SmoothStep(regressionE0, regressionE1, slowMoTime / slowMoMaxTime));
        colorAdjustments.colorFilter.value = Color.Lerp(Color.white, localTint, step);
    }

    private void SetFieldSize()
    {
        float scale = 0f;
        if (slowMoActive) scale =  slowMoTime / slowMoMaxTime;
        else if (isRewinding) scale = 1f - (regressionTime / regressionMaxTime);

        scale = GameManager.SmoothStep(smoothStepEdge0, smoothStepEdge1, scale);

        scale *= maxScale;
        field.localScale = new Vector3(scale, scale, scale);
    }

    private void SkillTimers()
    {
        if (slowMoActive && !isRewinding)
        {
            //Debug.Log("SlowMo");
            slowMoTime += Time.deltaTime;
            if (slowMoTime > slowMoMaxTime)
            {
                slowMoActive = false;
                isRewinding = true;
                startRewind.Invoke();
                cld.isTrigger = true;
                rb.isKinematic = true;
            }
        }
        else if (isRewinding)
        {
            //Debug.Log("Rewind");
            regressionTime += Time.deltaTime;
            if (regressionTime > regressionMaxTime)
            {
                isRewinding = false;
                if (perfectRegresion)
                {
                    transform.position = positions[0];
                    transform.rotation = playerRotations[0];
                    cam.rotation = camRotations[0];
                    startCooldown.Invoke();
                }
                cld.isTrigger = false;
                rb.isKinematic = false;
                rb.velocity = lastVelocity;
                rb.angularVelocity = lastAngularVelocity;
            }
        }
        else if (!canSlowMo)
        {
            //Debug.Log("Cooldown");
            coolDownTime += Time.deltaTime;
            if (coolDownTime > coolDownMaxTime)
            {
                canSlowMo = true;
                endCooldown.Invoke();
            }
        }
    }
}
