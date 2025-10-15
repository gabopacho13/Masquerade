using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FoxMovement : MonoBehaviour
{
    private List<PositionNode> positionNodes = new List<PositionNode>();
    private FoxInteraction foxInteraction;
    private PositionNode currentNode;
    private AudioSource stepSound;
    private AudioSource yipSound;
    private float stepTimer = 0f;
    public float stepInterval = 0.3f;

    protected enum FoxState
    {
        active,
        jumping,
        sitting,
        sittingBreak
    }
    protected FoxState currentState;

    protected Animator animator;
    private bool isActive = true;
    private bool isMoving = false;

    public float switchDistance = 0f;
    public float moveSpeed = 18f;
    private float currentSpeed = 0f;

    private float randomTime;
    private float timeCounter = 0.0f;

    private Vector3 targetPosition; // nuevo destino manual

    void Start()
    {
        GameObject positionGraph = GameObject.FindGameObjectWithTag("PositionGraph");
        foxInteraction = GetComponentInChildren<FoxInteraction>();

        if (positionGraph != null)
        {
            foreach (Transform child in positionGraph.transform)
            {
                PositionNode node = child.GetComponent<PositionNode>();
                if (node != null)
                {
                    positionNodes.Add(node);
                    if (currentNode == null || Vector3.Distance(this.transform.position, node.transform.position) < Vector3.Distance(this.transform.position, currentNode.transform.position))
                    {
                        currentNode = node;
                    }
                }
            }
            this.transform.position = currentNode.transform.position;
        }
        else
        {
            Debug.LogError("PositionGraph GameObject not found in the scene. Please ensure it exists and is tagged correctly.");
        }

        currentState = FoxState.active;
        animator = GetComponentInChildren<Animator>();
        targetPosition = currentNode.transform.position;
        stepSound = transform.Find("StepSound").GetComponent<AudioSource>();
        yipSound = transform.Find("YipSound").GetComponent<AudioSource>();
    }

    void Update()
    {
        isActive = foxInteraction.IsMoving;

        if (isActive && isMoving)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, moveSpeed, Time.deltaTime * 2f);
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, currentSpeed * Time.deltaTime);
            float distance = Vector3.Distance(transform.position, targetPosition);
            Vector3 direction = targetPosition - transform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            }
            if (!currentNode.isFinalNode && distance <= switchDistance)
            {
                currentNode = currentNode.BestNode.GetComponent<PositionNode>();
                targetPosition = currentNode.transform.position;
            }
            else if (currentNode.isFinalNode && distance <= switchDistance)
            {
                isMoving = false;
                currentSpeed = 0f;
            }
            if (timeCounter >= randomTime)
            {
                timeCounter = 0.0f;
                randomTime = Random.Range(3.0f, 10.0f);
                currentState = FoxState.jumping;
            }
            else
            {
                currentState = FoxState.active;
                timeCounter += Time.deltaTime;
            }
            if (stepSound != null && currentState == FoxState.active)
            {
                float distToPlayer = Vector3.Distance(this.transform.position, Camera.main.transform.position);
                if (distToPlayer > stepSound.maxDistance && !stepSound.mute)
                {
                    stepSound.mute = true;
                }
                else if (distToPlayer <= stepSound.maxDistance && stepSound.mute)
                {
                    stepSound.mute = false;
                }
                stepTimer += Time.deltaTime;
                if (stepTimer >= stepInterval)
                {
                    stepSound.PlayOneShot(stepSound.clip);
                    stepTimer = 0f;
                }
            }
            if (yipSound != null && currentState == FoxState.jumping)
            {
                yipSound.PlayOneShot(yipSound.clip);
            }
        }

        if (!isActive)
        {
            isMoving = false;
        }
    }

    private void LateUpdate()
    {
        if (isActive)
        {
            animator.SetInteger("State", (int)currentState);
            float speed = (isMoving) ? moveSpeed : 0f;
            animator.SetFloat("Speed", speed);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && isActive && !isMoving)
        {
            currentNode = currentNode.BestNode.GetComponent<PositionNode>();
            targetPosition = currentNode.transform.position;
            isMoving = true;
        }
    }
}
