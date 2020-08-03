using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowMoField : MonoBehaviour
{
    int playerLayer;

    private void Start()
    {
        playerLayer = LayerMask.NameToLayer("Player");
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer != playerLayer)
        {
            FreezeInTime fit = other.GetComponent<FreezeInTime>();
            if (fit)
            {
                fit.Freeze();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.layer != playerLayer)
        {
            FreezeInTime fit = other.GetComponent<FreezeInTime>();
            if (fit)
            {
                fit.UnFreeze();
            }
        }
    }
}
