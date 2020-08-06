using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class EnemySound : MonoBehaviour
{
    [SerializeField] AudioSource walkAudio;
    [SerializeField] AudioSource runAudio;
    [SerializeField] float volumeLerp = 0.5f;
    FreezeInTime freezeInTime;
    NavMeshAgent agent;
    EnemyController enemyController;

    private void Start()
    {
        freezeInTime = GetComponent<FreezeInTime>();
        agent = GetComponent<NavMeshAgent>();
        enemyController = GetComponent<EnemyController>();
    }

    private void FixedUpdate()
    {
        bool isStoppedOrFrozen = freezeInTime.frozen || agent.remainingDistance <= 0.02f;
        walkAudio.volume = Mathf.Lerp(walkAudio.volume,
            enemyController.state == EnemyController.EnemyState.patrol && !isStoppedOrFrozen ? 1f : 0f,
            volumeLerp);
        runAudio.volume = Mathf.Lerp(runAudio.volume,
            enemyController.state == EnemyController.EnemyState.pursuit && !isStoppedOrFrozen ? 1f : 0f,
            volumeLerp);
    }
}
