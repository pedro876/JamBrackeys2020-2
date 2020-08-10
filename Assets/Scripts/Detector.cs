using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Detector : MonoBehaviour
{
    [SerializeField] MeshRenderer renderer;
    [SerializeField] Material offMat;
    [SerializeField] Material onMat;
    public static HashSet<Transform> pointsOfInterest = new HashSet<Transform>();

    [SerializeField] Transform center;
    [SerializeField] float maxTime = 5f;
    float time = 0f;

    int fieldLayer;

    FreezeInTime freezeInTime;
    [SerializeField] UnityEvent onDetection;
    [SerializeField] UnityEvent onTimeout;

    private void Start()
    {
        renderer.material = offMat;
        time = maxTime;
        fieldLayer = LayerMask.NameToLayer("Field");
        freezeInTime = GetComponent<FreezeInTime>();
    }

    private void Update()
    {

        if(time < maxTime)
        {
            if(!freezeInTime.frozen) time += Time.deltaTime;
            if(time >= maxTime)
            {
                onTimeout.Invoke();
                pointsOfInterest.Remove(center);
                renderer.material = offMat;
            }
        }
    }

    private void OnDestroy()
    {
        pointsOfInterest.Remove(center);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == fieldLayer) return;
        time = 0f;
        if (!pointsOfInterest.Contains(center))
        {
            onDetection.Invoke();
            pointsOfInterest.Add(center);
            renderer.material = onMat;
        }
    }
}