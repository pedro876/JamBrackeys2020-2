using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                pointsOfInterest.Remove(center);
                renderer.material = offMat;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == fieldLayer) return;
        time = 0f;
        if (!pointsOfInterest.Contains(center))
        {
            pointsOfInterest.Add(center);
            renderer.material = onMat;
        }
    }
}