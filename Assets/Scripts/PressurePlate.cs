using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PressurePlate : MonoBehaviour
{
    [SerializeField] UnityEvent OnPressed;
    [SerializeField] UnityEvent OnUnpressed;

    [SerializeField] Material pressedMat;
    [SerializeField] Material unPressedMat;
    /*[SerializeField] Color unpressedColor = Color.red;
    [SerializeField] float unpressedIntensity = 1f;
    [SerializeField] Color pressedColor = Color.green;
    [SerializeField] float pressedIntensity = 2.5f;*/

    [SerializeField] Transform pressurePlateObj;
    [SerializeField] float pressedDisplacement = 0.05f;
    [SerializeField] [Range(0f, 1f)] float pressSpd = 0.3f;
    Vector3 defaultPos;
    Vector3 targetPos;

    int objectsOver = 0;
    int lastObjectsOver = 0;

    int fieldLayer;

    BoxCollider collider;
    MeshRenderer renderer;


    private void Start()
    {
        renderer = GetComponentInChildren<MeshRenderer>();
        //pressedMat.SetColor("_EmissionColor", unpressedColor * unpressedIntensity);
        renderer.material = unPressedMat;
        defaultPos = pressurePlateObj.position;
        targetPos = defaultPos;
        fieldLayer = LayerMask.NameToLayer("Field");
        collider = GetComponent<BoxCollider>();
    }

    private void FixedUpdate()
    {
        pressurePlateObj.position = Vector3.Lerp(pressurePlateObj.position, targetPos, pressSpd);
        if(objectsOver != lastObjectsOver)
        {
            if(objectsOver == 1)
            {
                renderer.material = pressedMat;
                //Debug.Log("pressed");
                //pressedMat.SetColor("_EmissionColor", pressedColor * pressedIntensity);
                targetPos = defaultPos - transform.up * pressedDisplacement;
                OnPressed.Invoke();
            } else if(objectsOver == 0)
            {
                //Debug.Log("unpressed");
                renderer.material = unPressedMat;
                //pressedMat.SetColor("_EmissionColor", unpressedColor * unpressedIntensity);
                targetPos = defaultPos;
                OnUnpressed.Invoke();
            }
        }
        lastObjectsOver = objectsOver;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == fieldLayer) return;
        objectsOver++;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == fieldLayer) return;
        objectsOver--;
        if (objectsOver < 0) objectsOver = 0;
    }
}
