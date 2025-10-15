using Unity.VisualScripting;
using UnityEngine;
using System.Collections;

public class Beast : Enemy, IMovable
{
    private bool isMoving = true;
    private bool isRotating = false;
    public float rotationSpeed = 10.0f;
    public float distance = 100.0f;
    public Material greyBearMaterial;
    public Material greyWeaponMaterial;
    private float currentDistance = 0.0f;
    private Quaternion initialRotation;
    private Vector3 initialPosition;
    private float waitTime = 0;
    private bool StartTalking { get; set; } = false;
    private bool hasTalked = false;
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
    public int health = 3;
    private bool wasFollowing = false;
    private bool startBeating = false;
    private bool isInvulnerable = false;
    private float invulnerableTime = 0f;
    private float flashTimer = 0f;
    private float flashInterval = 0.1f;
    private Renderer[] renderers;
    private float strikeExitTime = 0.75f;
    private float strikeExitTimer = 0f;
    private bool framePassed = false;
    public GameObject manholeLid;
    private AudioSource roarSound;
    private bool startedDying = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
        initialRotation = transform.rotation;
        initialPosition = transform.position;
        animator = GetComponentInChildren<Animator>();
        renderers = GetComponentsInChildren<Renderer>();
        roarSound = transform.Find("RoarSound").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        float distanceToPlayer = Vector3.Distance(this.transform.position, player.transform.position);
        Vector3 directionTowardsEnemy = (this.transform.position - player.transform.position).normalized;
        float anglePlayerEnemy = Vector3.Angle(player.transform.forward, directionTowardsEnemy);
        if (isDead && !hasTalked)
        {
            canFollow = false;
            isFollowing = false;
            CanHurt = false;
            if (BeastManager.VerifyActive(gameObject) == 0)
            {
                currentSpeed = 0f;
                if (!hasTalked)
                {
                    roarSound.pitch = 0.5f;
                    BeastManager.PlayDramaticSound();
                    roarSound.Play();
                }
                Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
                foreach (Renderer renderer in renderers)
                {
                    if (renderer.gameObject.CompareTag("Weapon"))
                    {
                        renderer.material = greyWeaponMaterial;
                    }
                    else
                    {
                        renderer.material = greyBearMaterial;
                    }
                }
                StartTalking = true;
                hasTalked = true;
            }
            else if (!startedDying)
            {
                startedDying = true;
                StartCoroutine(Die());
                return;
            }
        }
        if ((isMoving && !isRotating) || isFollowing)
        {
            currentState = BeastState.movement;
        }
        else if (isRotating)
        {
            currentState = BeastState.turning;
        }
        if (isFollowing)
        {
            currentDistance = 0f;
            isMoving = false;
        }
        if (distanceToPlayer <= 3f)
        {
            touchedPlayer = true;
            if (!startBeating && !isDead)
            {
                currentState = BeastState.strike;
                startBeating = true;
            }
        }
        if (Input.GetKeyDown(KeyCode.E) && anglePlayerEnemy <= 45f && distanceToPlayer <= 3f)
        {
            if (!isInvulnerable)
            {
                health--;
                if (health <= 0)
                {
                    isDead = true;
                    isFollowing = false;
                    isMoving = false;
                    StopAllCoroutines();
                }
                else
                {
                    currentState = BeastState.hurt;
                    touchedPlayer = true;
                    StopAllCoroutines();
                    isRotating = false;
                    isMoving = false;
                    isInvulnerable = true;
                    invulnerableTime = 0f;
                    flashTimer = 0f;
                }
            }
        }
        if (touchedPlayer)
        {   
            waitTime += Time.deltaTime;
            if (isFollowing)
            {
                isFollowing = false;
                wasFollowing = true;
            }
            if (waitTime > 1f)
            {
                waitTime = 0f;
                touchedPlayer = false;
                startBeating = false;
                if (wasFollowing)
                {
                    wasFollowing = false;
                    isFollowing = true;
                }
                else
                {
                    isMoving = true;
                }
            }
        }
        if (StartTalking)
        {
            StartTalking = false;
            IsTalking = true;
            CameraManager.ChangeToCamera(transform.Find(gameObject.name+"Camera").name);
            StartCoroutine(Talk());
            MusicManager.ChangePitch(0.3f);
        }
        if (hasTalked && !IsTalking)
        {
            hasTalked = false;
            BeastManager.instantiateMask(gameObject);
            CameraManager.ChangeToCamera("Main Camera");
            MusicManager.ChangePitch(1f);
            gameObject.SetActive(false);
        }
        if (isInvulnerable)
        {
            invulnerableTime += Time.deltaTime;
            flashTimer += Time.deltaTime;

            if (flashTimer >= flashInterval)
            {
                flashTimer = 0f;
                foreach (Renderer r in renderers)
                {
                    r.enabled = !r.enabled;
                }
            }

            if (invulnerableTime >= 2f)
            {
                isInvulnerable = false;
                foreach (Renderer r in renderers)
                {
                    r.enabled = true;
                }
            }
        }
        if (IsTalking)
        {
            if (currentDialogIndex == 4 && CameraManager.GetActiveCamera().name != "ManholeCamera")
            {
                CameraManager.ChangeToCamera("ManholeCamera");
                manholeLid.GetComponent<ManholeLid>().OpenUp = true;
            }
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (isMoving && !isRotating && !isDead)
        {
            Move();
        }
    }

    protected void LateUpdate()
    {
        animator.SetFloat("Speed", currentSpeed);
        animator.SetInteger("State", (int)currentState);
        if (currentState == BeastState.hurt)
        {
            if (!framePassed)
            {
                framePassed = true;
            }
            else
            {
                currentState = BeastState.movement;
                framePassed = false;
            }
            strikeExitTimer = 0f;
        }
        if (currentState == BeastState.strike)
        {
            strikeExitTimer += Time.deltaTime;
            if (strikeExitTimer >= strikeExitTime)
            {
                strikeExitTimer = 0f;
                startBeating = false;
                isMoving = true;
                isRotating = false;
                currentState = BeastState.movement;
            }
        }
    }

    public void Move()
    {
        if (currentDistance < distance)
        {
            Vector3 movement = transform.forward * currentSpeed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + movement);
            currentDistance += movement.magnitude;
        }
        else
        {
            isMoving = false;
            currentDistance = 0.0f;
            StartCoroutine(TurnAround(180));
        }
    }

    public IEnumerator TurnAround(float directionY)
    {
        isRotating = true;
        float rotatedAmount = 0f;
        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = startRotation * Quaternion.Euler(0, directionY, 0);

        while (rotatedAmount < 180f)
        {
            float step = rotationSpeed * Time.deltaTime;
            rb.MoveRotation(Quaternion.RotateTowards(rb.rotation, targetRotation, rotationSpeed * Time.deltaTime));
            rotatedAmount += step;
            //yield return new WaitUntil(() => !_isTalking);
            yield return null; // Espera un frame
        }

        transform.rotation = targetRotation; // Asegura rotación exacta al final
        isRotating = false;
        isMoving = true; // Reinicia el movimiento
    }

    protected override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);
        StartCoroutine(ReturnToOrigin());
    }

    private IEnumerator ReturnToOrigin()
    {
        while (Vector3.Distance(transform.position, initialPosition) > 0.01f)
        {
            Vector3 direction = (initialPosition - transform.position).normalized;
            Vector3 movement = direction * currentSpeed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + movement);
            SmoothLookAt(initialPosition);
            yield return new WaitForFixedUpdate();
        }

        rb.MovePosition(initialPosition); // asegura precisión
        StartCoroutine(RotateTowardsOriginalPath());
    }

    private IEnumerator RotateTowardsOriginalPath()
    {
        isRotating = true;
        Quaternion targetRotation = initialRotation;
        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.01f)
        {
            rb.MoveRotation(Quaternion.RotateTowards(rb.rotation, targetRotation, rotationSpeed * Time.deltaTime));
            yield return null; // Espera un frame
        }
        transform.rotation = targetRotation; // Asegura que la rotación final sea exacta
        isRotating = false;
        isMoving = true; // Reinicia el movimiento
    }

    protected IEnumerator Die()
    {
        currentState = BeastState.die;
        float timer = 0f;
        flashTimer = 0f;
        roarSound.PlayOneShot(roarSound.clip);
        while (timer < 2f)
        {
            timer += Time.deltaTime;
            flashTimer += Time.deltaTime;

            if (flashTimer >= flashInterval)
            {
                flashTimer = 0f;
                foreach (Renderer r in renderers)
                {
                    r.enabled = !r.enabled;
                }
            }

            yield return null; // Espera un frame
        }

        // Asegura visibilidad final antes de desactivar
        foreach (Renderer r in renderers)
        {
            r.enabled = true;
        }

        gameObject.SetActive(false);
    }
}
