using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField] GameObject bullet;
    [SerializeField] Transform tambor;
    [SerializeField] Transform bulletSpawn;
    [SerializeField] Transform target;
    Animator anim;
    Transform cam;
    [SerializeField] int maxAmmo = 6;
    int ammo = 6;
    bool canShoot = false;
    bool loadingBullet = false;
    [SerializeField] float loadingMaxTime = 0.2f;
    float loadingTime = 0f;
    Quaternion lastRot;
    //Quaternion desiredRot;
    [SerializeField] float normalLerp = 0.9f;
    Vector3 lastNormal;
    [SerializeField] float rayMaxDist = 10f;
    [SerializeField] float turnOnOffSpd = 0.9f;
    float turnedOn = 1f;
    [SerializeField]Material revolverMat;
    [SerializeField]Material tamborMat;  
    [SerializeField] float lightIntensity = 1.8f;
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main.transform;
        lastNormal = -cam.forward;
        revolverMat.SetColor("_EmissionColor", Color.black);
        tamborMat.SetColor("_EmissionColor", Color.black);
        //desiredRot = cylinder.rotation;
        lastRot = tambor.localRotation;
        anim = GetComponentInChildren<Animator>();
    }

    private void FixedUpdate()
    {
        float currentEmiIntensity = revolverMat.GetColor("_EmissionColor").r;
        revolverMat.SetColor("_EmissionColor", Color.white * Mathf.Lerp(currentEmiIntensity, lightIntensity * turnedOn, turnOnOffSpd));
        currentEmiIntensity = tamborMat.GetColor("_EmissionColor").r;
        tamborMat.SetColor("_EmissionColor", Color.white * Mathf.Lerp(currentEmiIntensity, lightIntensity * turnedOn, turnOnOffSpd));
    }

    public void TurnOnOff(bool on) { turnedOn = on ? 1f : 0f; }

    IEnumerator ShootBullet()
    {
        yield return new WaitForSeconds(6f / 30f);
        GameObject newBullet = Instantiate(bullet);
        newBullet.transform.position = bulletSpawn.position;
        newBullet.transform.LookAt(target.position);
        ammo--;
        if (ammo == 0) TurnOnOff(false);
        loadingBullet = true;
        loadingTime = 0f;
        lastRot = tambor.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTarget();
        
        if (Input.GetButtonDown("Fire1") && canShoot && ammo > 0 && !loadingBullet)
        {
            StartCoroutine("ShootBullet");
            //desiredRot = Quaternion.Euler(lastRot.eulerAngles + new Vector3(0f, 0f, -60f));
            anim.SetTrigger("shoot");
            Debug.Log("shoot");
        }

        if (loadingBullet)
        {
            loadingTime += Time.deltaTime;

            float degrees = GameManager.SmoothStep(0f, 1f, loadingTime / loadingMaxTime) * 60f;
            tambor.localRotation = lastRot;
            tambor.RotateAround(tambor.position, -tambor.up, degrees);
            //cylinder.rotation = Quaternion.Lerp(lastRot, desiredRot, loadingTime / loadingMaxTime);

            if(loadingTime > loadingMaxTime)
            {
                loadingBullet = false;
            }
        }

        Vector3 nextRot = transform.rotation.eulerAngles;
        
    }

    private void UpdateTarget()
    {
        RaycastHit hit;
        target.position = cam.position + cam.forward * rayMaxDist;
        Vector3 targetForward = cam.forward;
        if(Physics.Raycast(cam.position + cam.forward, cam.forward, out hit, rayMaxDist))
        {
            target.position = hit.point + hit.normal*0.08f;
            targetForward = -hit.normal;
        }
        target.forward = Vector3.Lerp(lastNormal, targetForward, normalLerp * Time.deltaTime);
        lastNormal = target.forward;
    }

    public void SetCanShoot(bool _canShoot) { canShoot = _canShoot; if (_canShoot) ammo = maxAmmo; }
}
