using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DianaSet : MonoBehaviour
{
    public bool allHit = false;
    Diana[] dianas;

    [SerializeField] UnityEvent onAllHit;

    private void Start()
    {
        dianas = GetComponentsInChildren<Diana>();
    }

    private void FixedUpdate()
    {
        if (!allHit)
        {
            allHit = true;
            foreach (Diana d in dianas) allHit = allHit && d.hit;
            if (allHit) onAllHit.Invoke();
        }
    }
}
