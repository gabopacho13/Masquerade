using UnityEngine;

public class Enemy : Talker
{
    public float viewAngle = 45f;
    protected bool isDead = false;
    protected bool canFollow = false;
    protected bool isFollowing = false;
    protected bool canSeePlayer = false;
    public float speed = 2.0f;
    public float followingSpeed = 8.0f;
    protected float currentSpeed = 0.0f;
    public LayerMask obstacleLayerMask;
    protected bool touchedPlayer = false;
    public bool CanHurt { get; protected set; } = true;
    protected Rigidbody rb;

    protected Vector3 desiredDirection = Vector3.zero;

    private AudioSource stepSound;
    private float stepInterval;
    private float nextStepTime;

    protected virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        currentSpeed = speed;
        rb = GetComponent<Rigidbody>();
        stepSound = transform.Find("BeastStep").GetComponent<AudioSource>();
    }

    protected virtual void Update()
    {
        float distToPlayer = Vector3.Distance(transform.position, player.transform.position);
        if (canFollow)
        {
            Vector3 dirToPlayer = (player.transform.position - transform.position).normalized;
            float angleToPlayer = Vector3.Angle(transform.forward, dirToPlayer);

            bool inCone = angleToPlayer <= viewAngle * 0.5f;
            bool lineOfSight = !Physics.Raycast(transform.position, dirToPlayer, distToPlayer, obstacleLayerMask);

            canSeePlayer = inCone && lineOfSight && !touchedPlayer;

            if (canFollow && canSeePlayer && !isDead)
            {
                isFollowing = true;
            }
            if (isFollowing && !isDead)
            {
                if (currentSpeed < followingSpeed)
                    currentSpeed += Time.deltaTime * speed;
                else
                    currentSpeed = followingSpeed;

                desiredDirection = dirToPlayer;
            }
            else if (touchedPlayer)
            {
                currentSpeed = 0.0f;
                desiredDirection = Vector3.zero;
            }
        }
        else if (!isDead)
        {
            if (currentSpeed > speed)
                currentSpeed -= Time.deltaTime * speed;
            else
                currentSpeed = speed;

            desiredDirection = Vector3.zero;
        }
        float frequency = 0.1818f * currentSpeed + 0.545f;
        frequency = Mathf.Max(0f, frequency);

        stepInterval = frequency > 0 ? 1 / frequency : Mathf.Infinity;

        if (Time.time >= nextStepTime)
        {
            if (stepSound != null)
                PlayStep(distToPlayer);
            nextStepTime = Time.time + stepInterval;
        }
    }

    protected virtual void FixedUpdate()
    {
        if (isFollowing && !isDead)
        {
            Vector3 movement = desiredDirection * currentSpeed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + movement);

            SmoothLookAt(player.transform.position);
        }
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canFollow = true;
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canFollow = false;
            isFollowing = false;
        }
    }

    public void PlayStep(float distanceToPlayer)
    {
        if (distanceToPlayer > stepSound.maxDistance && !stepSound.mute)
        {
            stepSound.mute = true;
        }
        else if (distanceToPlayer <= stepSound.maxDistance && stepSound.mute)
        {
            stepSound.mute = false;
        }
        stepSound.Play();
    }

    protected void SmoothLookAt(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            Quaternion smoothRotation = Quaternion.Slerp(transform.rotation, targetRotation, 5f * Time.fixedDeltaTime);
            rb.MoveRotation(smoothRotation);
        }
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            touchedPlayer = true;
        }
    }
}
