using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEnd : MonoBehaviour
{
    SlowMoSkill slowMoSkill;

    private void Start()
    {
        slowMoSkill = GameManager.player.GetComponent<SlowMoSkill>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!slowMoSkill.fieldActive)
            GameManager.NextLevel();
    }
}
