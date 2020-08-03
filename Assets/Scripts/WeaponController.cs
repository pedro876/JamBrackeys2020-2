using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField] GameObject bullet;
    [SerializeField] Transform cylinder;
    [SerializeField] Transform bulletSpawn;
    [SerializeField] Transform target;
    Transform camera;
    [SerializeField] int maxAmmo = 6;
    int ammo = 6;
    bool canShoot = false;
    bool loadingBullet = false;
    [SerializeField] float loadingMaxTime = 0.2f;
    float loadingTime = 0f;
    //float lastDegree = 0f;
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
        camera = Camera.main.transform;
        lastNormal = -camera.forward;
        revolverMat.SetColor("_EmissionColor", Color.black);
        tamborMat.SetColor("_EmissionColor", Color.black);
    }

    private void FixedUpdate()
    {
        float currentEmiIntensity = revolverMat.GetColor("_EmissionColor").r;
        revolverMat.SetColor("_EmissionColor", Color.white * Mathf.Lerp(currentEmiIntensity, lightIntensity * turnedOn, turnOnOffSpd));
        currentEmiIntensity = tamborMat.GetColor("_EmissionColor").r;
        tamborMat.SetColor("_EmissionColor", Color.white * Mathf.Lerp(currentEmiIntensity, lightIntensity * turnedOn, turnOnOffSpd));
    }

    public void TurnOnOff(bool on) { turnedOn = on ? 1f : 0f; }

    // Update is called once per frame
    void Update()
    {
        UpdateTarget();
        
        if (Input.GetButtonDown("Fire1") && canShoot && ammo > 0 && !loadingBullet)
        {
            GameObject newBullet = Instantiate(bullet);
            newBullet.transform.position = bulletSpawn.position;
            newBullet.transform.LookAt(target.position);
            ammo--;
            if (ammo == 0) TurnOnOff(false);
            loadingBullet = true;
            loadingTime = 0f;
            Debug.Log("shoot");
        }

        if (loadingBullet)
        {
            loadingTime += Time.deltaTime;

            if(loadingTime > loadingMaxTime)
            {
                loadingBullet = false;
            }
        }
    }

    private void UpdateTarget()
    {
        RaycastHit hit;
        target.position = camera.position + camera.forward * rayMaxDist;
        Vector3 targetForward = camera.forward;
        if(Physics.Raycast(camera.position, camera.forward, out hit, rayMaxDist))
        {
            target.position = hit.point + hit.normal*0.08f;
            targetForward = -hit.normal;
        }
        target.forward = Vector3.Lerp(lastNormal, targetForward, normalLerp * Time.deltaTime);
        lastNormal = target.forward;
    }

    public void SetCanShoot(bool _canShoot) { canShoot = _canShoot; if (_canShoot) ammo = maxAmmo; }
}
