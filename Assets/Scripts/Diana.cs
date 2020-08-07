using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Diana : MonoBehaviour
{
    DianaSet dianaSet;
    [SerializeField] Material normalMat;
    [SerializeField] Material hitMat;
    [SerializeField] float hitMaxTime = 5f;
    float hitTime = 0f;
    [HideInInspector] public bool hit = false;

    MeshRenderer renderer;

    [SerializeField] UnityEvent onHit;
    [SerializeField] UnityEvent onEndHit;

    int bulletLayer;

    private void Start()
    {
        dianaSet = GetComponentInParent<DianaSet>();
        renderer = GetComponent<MeshRenderer>();
        renderer.material = normalMat;
        bulletLayer = LayerMask.NameToLayer("Bullet");
    }

    private void Update()
    {
        if(hit && hitTime < hitMaxTime)
        {
            hitTime += Time.deltaTime;
            if(hitTime >= hitMaxTime && !(dianaSet != null && dianaSet.allHit))
            {
                renderer.material = normalMat;
                hit = false;
                onEndHit.Invoke();
            }
        }
    }

    private void GetHit()
    {
        hit = true;
        renderer.material = hitMat;
        hitTime = 0f;
        onHit.Invoke();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hit && other.gameObject.layer == bulletLayer) GetHit();
    }
}
