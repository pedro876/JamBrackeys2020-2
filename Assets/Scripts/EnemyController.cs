using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class EnemyController : MonoBehaviour
{
    #region variables

    public enum EnemyState { patrol, pursuit, shooting, stun }
    [SerializeField] public EnemyState state;

    FreezeInTime freezeInTime;
    Transform player;
    PlayerController playerController;
    NavMeshAgent agent;
    Vector3 destination;
    [SerializeField] Transform patrolRoot;
    List<Vector3> patrolRoute;
    int patrolNode = 0;

    [SerializeField] float patrolSpd = 3f;
    [SerializeField] float pursuitSpd = 6f;
    [SerializeField] float shootingLerp = 0.8f;
    [SerializeField] [Range(0f,1f)] [Tooltip("Vision Range for Dot product, 0 = 180 degrees, 1 = 0 degrees")]
    float viewRange = 0.5f;
    [SerializeField] float maxViewDistance = 5f;
    EnemyState preStunState = EnemyState.patrol;
    [SerializeField] float stunMaxTime = 5f;
    float lightIntensity;
    [SerializeField] [Range(0f,1f)] float stunPctToLightBlink = 0.8f;
    [SerializeField] float stunLightBlinkSpd = 3f;
    float stunTime = 0f;
    [SerializeField] float pursuitMinTime = 8f;
    [SerializeField] float toOriginalRotSpd = 0.5f;
    [SerializeField] float shootingSpeedMult = 0.1f;
    float distanceToPlayer;
    Quaternion originalRot;

    [SerializeField] Animator anim;
    [SerializeField] Material enemyMat;
    [SerializeField] Color pursuitColor;
    [SerializeField] Color patrolColor;
    [SerializeField] Color stunColor;
    [SerializeField] Light enemyLight;
    [SerializeField] [Range(0f,1f)] float colorLerp = 0.5f;

    int playerLayer;
    ParticleSystem laser;

    [SerializeField] UnityEvent OnStartPatrol;
    [SerializeField] UnityEvent OnStartPursuit;
    [SerializeField] UnityEvent OnStartShooting;
    [SerializeField] UnityEvent OnStartStun;

    #endregion

    void Start()
    {
        freezeInTime = GetComponent<FreezeInTime>();
        laser = GetComponentInChildren<ParticleSystem>();
        originalRot = transform.rotation;
        patrolRoute = new List<Vector3>();
        if (patrolRoot == null)
        {
            patrolRoot = transform;
            patrolRoute.Add(transform.position);
        }
        else
        {
            Transform[] children = patrolRoot.GetComponentsInChildren<Transform>();
            if (children != null && children.Length > 0)
                for (int i = 0; i < children.Length; i++) patrolRoute.Add(children[i].position);
        }
        transform.position = patrolRoot.position;
        if (GameManager.player)
        {
            player = GameManager.player;
            playerController = player.GetComponent<PlayerController>();
        }
        
        playerLayer = LayerMask.NameToLayer("Player");
        agent = GetComponent<NavMeshAgent>();
        agent.SetDestination(transform.position);
        destination = transform.position;
        distanceToPlayer = agent.stoppingDistance;
        ChangeState(state);
        enemyMat.SetColor("_EmissionColor", patrolColor*2.5f);
        enemyLight.color = patrolColor;
        lightIntensity = enemyLight.intensity;
    }

    private void FixedUpdate()
    {
        Color lightCol = Color.white;
        
        switch (state)
        {
            case EnemyState.patrol:    lightCol = patrolColor;  PatrolUpdate();     break;
            case EnemyState.pursuit:   lightCol = pursuitColor; PursuitUpdate();    break;
            case EnemyState.shooting:  lightCol = pursuitColor; ShootingUpdate();   break;
            case EnemyState.stun:      lightCol = stunColor; StunUpdate();       break;
        }

        if(state != EnemyState.stun)
        {
            enemyMat.SetColor("_EmissionColor",
            Color.Lerp(enemyMat.GetColor("_EmissionColor"),
            lightCol * 2.5f,
            colorLerp));

        }

        enemyLight.color = Color.Lerp(enemyLight.color, lightCol, colorLerp);
        
    }

    #region Patrol

    void PatrolNextDestiny()
    {
        //for (int i = 0; i < patrolRoute.Count; i++) Debug.Log(patrolRoute[i]);
        //Debug.Log("-----------------------");
        patrolNode = (patrolNode + 1) % patrolRoute.Count;
        agent.SetDestination(patrolRoute[patrolNode]);
    }

    void PatrolUpdate()
    {
        bool agentIsStopped = agent.remainingDistance <= 0.02f;
        anim.SetBool("walk", !agentIsStopped);
        anim.SetBool("idle", agentIsStopped);
        if (!CheckDetectors())
        {
            if (patrolRoute.Count > 1 && agentIsStopped) PatrolNextDestiny();
            else if (agentIsStopped)
                transform.rotation = Quaternion.Lerp(transform.rotation, originalRot, toOriginalRotSpd * Time.deltaTime);
        }
        if (PlayerInViewRange())  ChangeState(EnemyState.shooting);
    }

    bool CheckDetectors()
    {
        if (Detector.pointsOfInterest.Count > 0)
        {
            Vector3 closest = Vector3.zero;
            float minDistance = float.PositiveInfinity;
            foreach(Transform t in Detector.pointsOfInterest)
            {
                float distance = (t.position - transform.position).magnitude;
                if (distance < minDistance)
                {
                    closest = t.position;
                    minDistance = distance;
                }
            }
            agent.SetDestination(closest);
            return true;
        }
        else return false;
    }

    #endregion

    #region Pursuit

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

        bool agentIsStopped = agent.remainingDistance <= 0.02f;
        anim.SetBool("run", !agentIsStopped);
        anim.SetBool("idle", agentIsStopped);
    }
    IEnumerator PursuitTimer()
    {
        do yield return new WaitForSeconds(pursuitMinTime);
        while (PlayerInViewRange() || freezeInTime.frozen);
        ChangeState(EnemyState.patrol);
    }

    #endregion

    #region Shoot

    void ShootingUpdate()
    {
        destination = GameManager.player.position;
        agent.SetDestination(destination);
        Vector3 dirToPlayer = Vector3.ProjectOnPlane(player.position - transform.position, Vector3.up).normalized;
        Quaternion look = Quaternion.LookRotation(dirToPlayer, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, look, shootingLerp);
        if (!PlayerInViewRange()) ChangeState(EnemyState.pursuit);
        laser.transform.LookAt(player);
    }

    #endregion

    #region Stun
    public void Stun() { if (state != EnemyState.stun) ChangeState(EnemyState.stun); }

    void StunUpdate()
    {
        stunTime += Time.fixedDeltaTime;
        if (stunTime < stunMaxTime)
        {
            float pct = stunTime / stunMaxTime;
            if (pct > stunPctToLightBlink)
            {
                float lighten = (Mathf.Sin(stunTime * stunLightBlinkSpd) > 0f ? 1f : 0f);
                enemyMat.SetColor("_EmissionColor",
                Color.Lerp(enemyMat.GetColor("_EmissionColor"),
                stunColor * 2.5f * lighten,
                colorLerp));

                enemyLight.intensity = lightIntensity * lighten;
            }
            else
            {
                enemyMat.SetColor("_EmissionColor", stunColor);
                enemyLight.intensity = 0f;
            }
        }
        else
        {
            enemyLight.intensity = lightIntensity;
            ChangeState(preStunState);
        }
    }

    #endregion

    public void ChangeState(EnemyState newState)
    {
        StopCoroutine("PursuitTimer");
        switch (newState)
        {
            case EnemyState.patrol:
                OnStartPatrol.Invoke();
                agent.speed = patrolSpd;
                PatrolNextDestiny();
                agent.SetDestination(patrolRoute[patrolNode]);
                agent.stoppingDistance = 0f;
                anim.SetBool("run", false);
                anim.SetBool("shoot", false);
                break;
            case EnemyState.pursuit:
                OnStartPursuit.Invoke();
                agent.speed = pursuitSpd;
                StartCoroutine("PursuitTimer");
                agent.stoppingDistance = distanceToPlayer;
                //anim.SetBool("run", true);
                anim.SetBool("walk", false);
                //anim.SetBool("idle", false);
                anim.SetBool("shoot", false);
                break;
            case EnemyState.shooting:
                OnStartShooting.Invoke();                               
                agent.speed = pursuitSpd * shootingSpeedMult;
                anim.SetBool("run", false);
                anim.SetBool("walk", false);
                anim.SetBool("idle", false);
                anim.SetBool("shoot", true);
                break;
            case EnemyState.stun:
                OnStartStun.Invoke();
                preStunState = state;
                agent.speed = 0f;
                anim.SetBool("run", false);
                anim.SetBool("shoot", false);
                anim.SetBool("walk", false);
                anim.SetBool("idle", true);
                stunTime = 0f;
                break;
        }
        state = newState;
    }

    public void StartLaser()
    {
        laser.Play();
        if (state != EnemyState.shooting) laser.Stop();
    }

    private bool PlayerInViewRange()
    {
        if(player == null) return false;
        if (playerController.hidden) return false;
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
