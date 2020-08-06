using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLaserCollision : MonoBehaviour
{
    int playerLayer;
    private void Start()
    {
        playerLayer = LayerMask.NameToLayer("Player");
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.layer == playerLayer)
        {
            //Debug.Log("Particle collision");
            other.GetComponent<PlayerHealth>().ReceiveDamage();
        }
    }
}
