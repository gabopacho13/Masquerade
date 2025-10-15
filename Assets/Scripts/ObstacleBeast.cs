using System.Collections;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class ObstacleBeast : Enemy
{

    private bool alreadyTalked = false;
    private bool followingPlayer = false;
    private float waitTime = 0;
    public GameObject obstacleMask;
    public bool StartTalking { get; set; } = false;
    private Animator animator;
    private bool isJumping = false;
    private GameObject bomb;
    private bool startedDying = false;

    private enum BeastState
    {
        movement,
        turning,
        hurt,
        strike,
        die
    }
    private NavMeshAgent navMeshAgent;

    protected override void Start()
    {
        base.Start();
        gameObject.SetActive(false); // Desactiva el GameObject al inicio
        animator = GetComponentInChildren<Animator>();
        animator.SetInteger("State", (int)BeastState.movement);
        navMeshAgent = GetComponent<NavMeshAgent>();
        foreach(Transform child in transform)
        {
            if (child.CompareTag("Bomb"))
            {
                bomb = child.gameObject;
            }
        }
        bomb.SetActive(false);
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (StartTalking)
        {
            StartTalking = false;
            IsTalking = true;
            StartCoroutine(Talk());
            alreadyTalked = true;
        }

        if (alreadyTalked && !IsTalking)
        {
            if (CurrentDialogListIndex == 0)
            {
            CameraManager.ChangeToCamera("Main Camera");
            followingPlayer = true;
            alreadyTalked = false;
            }
            else if (CurrentDialogListIndex == 1 && !startedDying)
            {
                StartCoroutine(Die());
                startedDying = true;
            }
        }
        if (followingPlayer && !isJumping)
        {
            FollowPlayer();
        }
        if (touchedPlayer)
        {
            followingPlayer = false;
            navMeshAgent.isStopped = true;
            navMeshAgent.velocity = Vector3.zero;
            waitTime += Time.deltaTime;
            if (waitTime > 2.0f)
            {
                followingPlayer = true;
                waitTime = 0f;
                touchedPlayer = false;
                navMeshAgent.isStopped = false;
            }
        }
        if (obstacleMask == null && !alreadyTalked)
        {
            CurrentDialogListIndex = 1;
            followingPlayer = false;
            navMeshAgent.isStopped = true;
            CameraManager.ChangeToCamera("ObstacleCourseCamera");
            StartTalking = true;
        }
        if (navMeshAgent.isOnOffMeshLink && !isJumping)
        {
            StartCoroutine(JumpOverObstacle(navMeshAgent.currentOffMeshLinkData));
        }
    }

    private void LateUpdate()
    {
        animator.SetFloat("Speed", navMeshAgent.velocity.magnitude);
    }

    private void FollowPlayer()
    {
        if (player != null)
        {
            Vector3 targetPosition = player.transform.position;
            targetPosition.y = transform.position.y;
            navMeshAgent.SetDestination(targetPosition);
        }
    }

    protected IEnumerator JumpOverObstacle(OffMeshLinkData data)
    {
        isJumping = true;
        //navMeshAgent.isStopped = true;
        Vector3 startPos = navMeshAgent.transform.position;
        Vector3 endPos = data.endPos + (navMeshAgent.baseOffset * Vector3.up);

        float jumpHeight = 2f;
        float jumpDuration = 0.8f;

        GameObject offLink = data.owner.GetComponent<NavMeshLink>()?.gameObject;
        if (offLink != null)
        {
            NavMeshCustomJump settings = offLink.GetComponent<NavMeshCustomJump>();
            if (settings != null)
            {
                jumpHeight = settings.jumpHeight;
                jumpDuration = settings.jumpDuration;
            }
        }

        float time = 0f;

        while (time < jumpDuration)
        {
            float t = time / jumpDuration;
            float height = Mathf.Sin(Mathf.PI * t) * jumpHeight;
            Vector3 interpolate = Vector3.Lerp(startPos, endPos, t) + Vector3.up * height;
            navMeshAgent.transform.position = interpolate;

            time += Time.deltaTime;
            yield return null;
        }

        navMeshAgent.Warp(endPos);

        navMeshAgent.CompleteOffMeshLink();
        isJumping = false;
    }

    private IEnumerator Die()
    {
        bomb.SetActive(true);
        yield return new WaitUntil(() => !bomb.GetComponent<BombManager>().BombBody.activeSelf);
        animator.gameObject.SetActive(false);
        yield return new WaitUntil(() => bomb.IsDestroyed());
        MusicManager.ChangeMusic("Overworld");
        CameraManager.ChangeToCamera("Main Camera");
        transform.GetComponentInChildren<Camera>().transform.parent = transform.parent;
        Destroy(this.gameObject);
    }
}
