using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using System;
using System.Net;

public class FoxInteraction : Character, IMovable
{
    protected enum FoxState
    {
        active,
        jumping,
        sitting,
        sittingBreak
    }
    protected FoxState currentState;
    public bool IsMoving { get; set; } = true;
    protected bool isRotating = false;
    public float speed = 2.0f;
    public float rotationSpeed = 10.0f;
    public float rotationAngle = 90.0f;
    public List<int> distances = new() { 50 };
    private int currentDistanceIndex = 0;
    protected float currentDistance = 0.0f;
    private float randomTime;
    private float timeCounter = 0.0f;
    private GameObject mask;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
        mask = transform.Find("FoxMask").gameObject;
        if (mask == null)
        {
            Debug.LogError("FoxMask GameObject not found in Fox.");
        }
        else
        {
            mask.SetActive(false); // Initially hide the mask
        }
        randomTime = UnityEngine.Random.Range(3.0f, 10.0f);
        voice = transform.parent.transform.Find("YipSound").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        timeCounter += Time.deltaTime;
        if (!IsMoving)
        {
            if (timeCounter >= randomTime)
            {
                timeCounter = 0.0f;
                randomTime = UnityEngine.Random.Range(3.0f, 10.0f);
                currentState = FoxState.sittingBreak;
            }
            else
            {
                currentState = FoxState.sitting;
            }
        }
    }

    protected void LateUpdate()
    {
        if (!IsMoving)
        {
            animator.SetInteger("State", (int)currentState);
        }
    }

    public void Move()
    {
 
        if (currentDistance < distances[currentDistanceIndex])
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
            currentDistance += speed * Time.deltaTime;
        }
        else
        {
            currentDistance = 0.0f;
            StartCoroutine(TurnAround(rotationAngle));
        }
    }

    public IEnumerator TurnAround(float directionY)
    {
        isRotating = true;
        float rotatedAmount = 0f;
        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = startRotation * Quaternion.Euler(0, directionY, 0);

        while (rotatedAmount < Math.Abs(rotationAngle))
        {
            float step = rotationSpeed * Time.deltaTime;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, step);
            rotatedAmount += step;
            yield return new WaitUntil(() => !_isTalking);
            yield return null; // Espera un frame
        }

        transform.rotation = targetRotation; // Asegura rotación exacta al final
        isRotating = false;
        currentDistanceIndex = (currentDistanceIndex + 1) % distances.Count; // Cambia al siguiente índice de distancia
        //isMoving = true; // Reinicia el movimiento
    }

    protected override IEnumerator Talk(List<AudioClip> audioClips = null, AudioSource audioSource = null)
    {
        IsMoving = false; // Detiene el movimiento al iniciar la conversación
        if (mask != null && !mask.activeSelf)
        {
            mask.SetActive(true); // Muestra el mask al iniciar la conversación
        }
        yield return StartCoroutine(base.Talk(audioClips, audioSource)); 
    }
}
