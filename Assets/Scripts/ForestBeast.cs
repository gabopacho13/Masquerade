using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class ForestBeast : Enemy
{

    private NavMeshAgent navMeshAgent;
    private Animator animator;
    private enum BeastState
    {
        movement,
        turning,
        hurt,
        strike,
        die
    }
    private BeastState currentState;
    public bool IsHitting { get; set; } = false;
    public bool RestartStrikeAnimation { get; set; } = false;
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private float stoppingDistance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
        navMeshAgent = GetComponent<NavMeshAgent>();
        currentState = BeastState.movement;
        animator = GetComponentInChildren<Animator>();
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        followingSpeed = navMeshAgent.speed;
        stoppingDistance = navMeshAgent.stoppingDistance;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        Vector3 direction = player.transform.position - transform.position;
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        float distanceToInitial = Vector3.Distance(transform.position, initialPosition);
        if (!isFollowing)
        {
            if (distanceToInitial < 1f)
            {
                transform.position = initialPosition;
                navMeshAgent.speed = followingSpeed;
                navMeshAgent.stoppingDistance = stoppingDistance;
                navMeshAgent.ResetPath();
                transform.rotation = Quaternion.Slerp(transform.rotation, initialRotation, Time.deltaTime * 2f);
                if (Quaternion.Angle(transform.rotation, initialRotation) < 0.1f)
                {
                    transform.rotation = initialRotation;
                }
            }
        }
        if (isFollowing && !isDead)
        {
            if (navMeshAgent != null && player != null)
            {
                if (distanceToPlayer > navMeshAgent.stoppingDistance && !IsHitting)
                {
                    if (navMeshAgent.isStopped)
                    {
                        Vector3 dirNorm = direction.normalized;
                        float animSpeed = animator.GetFloat("Speed");
                        navMeshAgent.velocity = dirNorm * animSpeed;
                        navMeshAgent.isStopped = false;
                    }
                    navMeshAgent.SetDestination(player.transform.position);
                    currentState = BeastState.movement;
                }
                else if (distanceToPlayer <= navMeshAgent.stoppingDistance)
                {
                    currentState = BeastState.strike;
                }
                if (IsHitting)
                {
                    navMeshAgent.isStopped = true;
                }
                if (navMeshAgent.isStopped)
                {
                    if (direction.sqrMagnitude > 0.001f)
                    {
                        Quaternion targetRotation = Quaternion.LookRotation(direction) * Quaternion.Euler(0, -5f, 0);
                        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
                    }
                    if (distanceToPlayer > 3.5f)
                    {
                        transform.position = Vector3.MoveTowards(transform.position, player.transform.position, navMeshAgent.speed/2 * Time.deltaTime);
                        currentState = BeastState.movement;
                    }
                    else
                    {
                        currentState = BeastState.strike;
                    }
                }
            }
        }
    }

    protected override void FixedUpdate()
    {
        
    }

    protected void LateUpdate()
    {
        if (RestartStrikeAnimation && currentState == BeastState.strike)
        {
            animator.Play("Strike", -1, 0f);
        }
        animator.SetInteger("State", (int)currentState);
        if (!navMeshAgent.isStopped)
            animator.SetFloat("Speed", navMeshAgent.velocity.magnitude);
        else
        {
            if (currentState == BeastState.strike)
                animator.SetFloat("Speed", 0);
            else
                animator.SetFloat("Speed", navMeshAgent.speed / 2);
        }
        RestartStrikeAnimation = false;
    }

    protected override void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canFollow = false;
            if (isFollowing)
            {
                navMeshAgent.speed = followingSpeed / 2;
                navMeshAgent.stoppingDistance = 0f;
                navMeshAgent.SetDestination(initialPosition);
            }
            isFollowing = false;
        }
    }
}
