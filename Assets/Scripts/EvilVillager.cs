using UnityEngine;
using UnityEngine.AI;

public class EvilVillager : MonoBehaviour
{

    private NavMeshAgent navMeshAgent;
    private Animator animator;
    public Vector3 InitialPosition { get; private set; }
    public Quaternion InitialRotation { get; private set; }
    private float stoppingDistance;
    private float followingSpeed;
    private GameObject player;
    public bool IsFollowing { get; set; } = false;
    private AudioSource stepSound;
    private float stepTimer = 0f;
    public float stepInterval = 0.33f;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        InitialPosition = transform.position;
        InitialRotation = transform.rotation;
        followingSpeed = navMeshAgent.speed;
        player = GameObject.FindGameObjectWithTag("Player");
        navMeshAgent.SetDestination(player.transform.position);
        stepSound = transform.Find("StepSound").GetComponent<AudioSource>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        navMeshAgent.SetDestination(player.transform.position);
        stepTimer += Time.deltaTime;
        if (stepTimer >= stepInterval && navMeshAgent.velocity.magnitude > 0.1f)
        {
            stepSound.PlayOneShot(stepSound.clip);
            stepTimer = 0f;
        }
    }

    private void LateUpdate()
    {
        if (animator.GetInteger("State") != 1)
        {
            animator.SetInteger("State", 1);
        }
    }
}
