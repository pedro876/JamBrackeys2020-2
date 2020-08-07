using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lamp : MonoBehaviour
{
    [SerializeField] Material offMat;
    [SerializeField] Material onMat;
    MeshRenderer renderer;

    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<MeshRenderer>();
        renderer.material = offMat;
    }

    public void TurnOn()
    {
        renderer.material = onMat;
    }

    public void TurnOff()
    {
        renderer.material = offMat;
    }
}
