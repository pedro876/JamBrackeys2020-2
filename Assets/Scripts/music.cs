using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class music : MonoBehaviour
{
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        //GetComponent<AudioSource>().Play();
    }
}
