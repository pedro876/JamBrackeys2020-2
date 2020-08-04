using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class EnemyController : MonoBehaviour
{
    public enum EnemyState { patrol, pursuit, shooting }
    [SerializeField] EnemyState state;

    Transform player;
    NavMeshAgent agent;
    Vector3 destination;
    [SerializeField] Transform patrolRoot;
    List<Transform> patrolRoute;
    int patrolNode = 0;

    //[Header("")]
    [SerializeField] float shootingLerp = 0.8f;
    [SerializeField] [Range(0f,1f)] [Tooltip("Vision Range for Dot product, 0 = 180 degrees, 1 = 0 degrees")]
    float viewRange = 0.5f;
    [SerializeField] float maxViewDistance = 5f;

    int playerLayer;

    ParticleSystem laser;

    [SerializeField] UnityEvent OnStartPatrol;
    [SerializeField] UnityEvent OnStartPursuit;
    [SerializeField] UnityEvent OnStartShooting;

    // Start is called before the first frame update
    void Start()
    {
        laser = GetComponentInChildren<ParticleSystem>();
        if (patrolRoot == null)
        {
            patrolRoot = transform;
            patrolRoute = new List<Transform>();
        }
        else
        {
            transform.position = patrolRoot.position;
            patrolRoute = new List<Transform>(patrolRoot.GetComponentsInChildren<Transform>());
        }
        player = GameManager.player;
        playerLayer = LayerMask.NameToLayer("Player");
        agent = GetComponent<NavMeshAgent>();
        agent.SetDestination(transform.position);
    }

    private void FixedUpdate()
    {
        Debug.Log("State: " + state + " | inRange: " + PlayerInViewRange());
        switch (state)
        {
            case EnemyState.patrol:     PatrolUpdate();     break;
            case EnemyState.pursuit:    PursuitUpdate();    break;
            case EnemyState.shooting:   ShootingUpdate();   break;
        }
    }

    void PatrolUpdate()
    {
        if(patrolRoute.Count > 1 && agent.isStopped)
        {
            patrolNode = (patrolNode + 1) % patrolRoute.Count;
            agent.SetDestination(patrolRoute[patrolNode].position);
        }
        if (PlayerInViewRange())
        {
            ChangeState(EnemyState.shooting);
        }
    }
    void PursuitUpdate()
    {
        if (PlayerInViewRange())
        {
            ChangeState(EnemyState.shooting);
        }
        else
        {
            destination = GameManager.player.position;
            agent.SetDestination(destination);
        }
    }
    void ShootingUpdate()
    {
        Vector3 dirToPlayer = Vector3.ProjectOnPlane(player.position - transform.position, Vector3.up).normalized;
        Quaternion look = Quaternion.LookRotation(dirToPlayer, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, look, shootingLerp);
        if (!PlayerInViewRange()) ChangeState(EnemyState.pursuit);
        laser.transform.LookAt(player);
    }

    public void ChangeState(EnemyState newState)
    {
        state = newState;
        switch (newState)
        {
            case EnemyState.patrol:     OnStartPatrol.Invoke();     StartStopAgent(true);   break;
            case EnemyState.pursuit:    OnStartPursuit.Invoke();    StartStopAgent(true);   break;
            case EnemyState.shooting:   OnStartShooting.Invoke();   StartStopAgent(false);  break;
        }
    }

    public void StartLaser()
    {
        laser.Play();
        if (state != EnemyState.shooting) laser.Stop();
    }

    private bool PlayerInViewRange()
    {
        RaycastHit hit;
        Vector3 dirToPlayer = (player.position - transform.position);
        if(Vector3.Dot(transform.forward, dirToPlayer) > viewRange &&
            dirToPlayer.magnitude < maxViewDistance &&
            Physics.Raycast(transform.position, dirToPlayer, out hit, Mathf.Infinity))
        {
            return hit.collider.gameObject.layer == playerLayer;
        }
        return false;
    }

    public void StartStopAgent(bool start)
    {
        if(!start) destination = agent.destination;
        agent.enabled = start;
        if (start) agent.SetDestination(destination);
    }
}
