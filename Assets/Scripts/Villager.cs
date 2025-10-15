using NUnit.Framework;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Audio;

public class Villager : Character, IMovable
{
    protected enum VillagerState
    {
        Idle,
        Moving
    }
    protected VillagerState currentState;
    public bool moves = false;
    protected bool isMoving = false;
    protected bool isRotating = false;
    public float speed = 2.0f;
    public float rotationSpeed = 10.0f;
    public float distance = 100.0f;
    protected float currentDistance = 0.0f;
    protected AudioSource stepSound; 
    protected int frameCounter = 0;
    private float stepTimer = 0f;
    public float stepInterval = 0.33f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
        if (moves)
        {
            isMoving = true;
            currentState = VillagerState.Moving;
            stepSound = transform.Find("StepSound").GetComponent<AudioSource>();
        }
        else
        {
            isMoving = false;
            currentState = VillagerState.Idle;
        }
        voice = transform.Find("Voice").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (isMoving)
        {
            Move();
            currentState = VillagerState.Moving;
        }
        else if (isRotating && !IsTalking)
        {
            currentState = VillagerState.Moving;
        }
        else
        {
            currentState = VillagerState.Idle;
        }
        if (stepSound != null)
        {
            PlayStepSound();
        }
    }

    protected void LateUpdate()
    {
        animator.SetInteger("State", (int)currentState);
    }

    public void PlayStepSound()
    {
        float distToPlayer = Vector3.Distance(this.transform.position, player.transform.position);
        if (distToPlayer > stepSound.maxDistance && !stepSound.mute)
        {
            stepSound.mute = true;
        }
        else if (distToPlayer <= stepSound.maxDistance && stepSound.mute)
        {
            stepSound.mute = false;
        }
        if (currentState == VillagerState.Moving && player.GetComponent<Player>().GameOn)
        {
            stepTimer += Time.deltaTime;
            if (stepTimer >= stepInterval)
            {
                stepSound.PlayOneShot(stepSound.clip);
                stepTimer = 0f;
            }
        }
        else
        {
            stepTimer = 0f;
        }
    }

    public void Move()
    {
        if (currentDistance < distance)
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
            currentDistance += speed * Time.deltaTime;
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
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, step);
            rotatedAmount += step;
            yield return new WaitUntil(() => !_isTalking);
            yield return null; // Espera un frame
        }

        transform.rotation = targetRotation; // Asegura rotación exacta al final
        isRotating = false;
        isMoving = true; // Reinicia el movimiento
    }

    protected override IEnumerator Talk(List<AudioClip> audioClips = null, AudioSource audioSource = null)
    {
        if (moves)
        {
            isMoving = false; // Detiene el movimiento al iniciar la conversación
        }
        yield return StartCoroutine(base.Talk(voiceClips, voice));
        if (moves && !isRotating)
        {
            isMoving = true; // Reinicia el movimiento al finalizar la conversación
        }
    }
}
