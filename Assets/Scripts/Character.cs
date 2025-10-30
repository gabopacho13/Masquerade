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
    protected Animator animator;
    protected AudioSource voice;
    public List<AudioClip> voiceClips = new();
    protected bool canTalk = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        animator = GetComponent<Animator>();
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
        if (canTalk && Input.GetKeyDown(KeyCode.C))
        {
            StartTalking = true;
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
                UIManager.InteractInstruction.SetActive(true); // Muestra la instrucción de interacción
                canTalk = true;
            }
            else
            {
                UIManager.InteractInstruction.SetActive(false); // Oculta la instrucción de interacción
                canTalk = false;
            }
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            UIManager.InteractInstruction.SetActive(false); // Oculta la instrucción de interacción al salir del trigger
            canTalk = false;
        }
    }
}
