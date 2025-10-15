using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Character : Talker
{
    
    public bool StartTalking { get; set; } = false;
    protected bool isOnTheGround = false; // Variable para verificar si el villager está en el suelo
    protected GameObject interactInstruction;
    protected Animator animator;
    protected AudioSource voice;
    public List<AudioClip> voiceClips = new();

    protected override void Awake()
    {
        base.Awake();
        interactInstruction = GameObject.FindGameObjectWithTag("InteractInstruction");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        animator = GetComponent<Animator>();
        if (interactInstruction == null)
        {
            Debug.LogError("InteractInstruction GameObject not found in the scene. Please ensure it exists.");
            return;
        }
        if (interactInstruction.activeSelf)
        {
            interactInstruction.SetActive(false); // Asegura que la instrucción de interacción esté oculta al inicio
        }
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("terrain"))
        {
            this.GetComponent<Rigidbody>().isKinematic = true; // Desactiva la física al colisionar con el terreno
            isOnTheGround = true; // Marca que el villager está en el suelo
        }
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (!isOnTheGround)
        {
            this.GetComponent<Rigidbody>().isKinematic = false;
        }
        else
        {
            if (StartTalking)
            {
                StartTalking = false;
                IsTalking = true;
                StartCoroutine(Talk(voiceClips, voice));
            }
        }
    }

    protected virtual void OnTriggerStay(Collider other)
    {
        float distancia = Vector3.Distance(this.transform.position, player.transform.position);
        Vector3 direccionHaciaNPC = (this.transform.position - player.transform.position).normalized;
        float angulo = Vector3.Angle(player.transform.forward, direccionHaciaNPC);

        if (other.CompareTag("Player"))
        {
            if (distancia <= 2.5f && angulo <= 45f && !IsTalking && !dialogs.Count.Equals(0))
            {
                interactInstruction.SetActive(true); // Muestra la instrucción de interacción
                if (Input.GetKeyDown(KeyCode.C))
                {
                    StartTalking = true;
                }
            }
            else
            {
                interactInstruction.SetActive(false); // Oculta la instrucción de interacción
            }
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            interactInstruction.SetActive(false); // Oculta la instrucción de interacción al salir del trigger
        }
    }
}
