using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ReactToBulletHit : MonoBehaviour
{
    [SerializeField] UnityEvent OnBulletHit;
    public void React() { OnBulletHit.Invoke(); }
}