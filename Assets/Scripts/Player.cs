using UnityEngine;
using JetBrains.Annotations;
using System;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using System.Collections;
using TMPro;
using System.Collections.Generic;

public class Player : Talker
{
    public Camera mainCamera;
    public float desiredSpeed = 5.0f, sensitivityH = 9.0f, sensitivityV = 9.0f;
    private float speed = 5.0f; // Velocidad de movimiento del jugador
    private float rotationV = 0.0f, rotationH = 0.0f;
    public int maxHealth, minHealth;
    public int Health { get { return _health; } private set { _health = value; } }
    private int _health;
    private bool wasDamaged = false;
    private bool isDead = false;
    public bool GameOn { get; private set; } = false;
    public GameObject target;
    //public float t;
    public float jumpForce = 5.0f;
    private bool jumping = false;
    private bool isGrounded = false;
    private bool isInvulnerable = false; // Indica si el jugador es invulnerable
    private Stack<GameObject> hearts = new();
    private GameObject gameOver;
    private Vector3 respawnPosition;
    private AudioSource stepSound;
    private AudioSource punchSound;
    private float stepTimer = 0f;
    public float stepIntervalWalking = 0.66f;
    public float stepIntervalRunning = 0.4f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Transform heartGroup = GameObject.Find("Hearts").transform;
        for (int i = 0; i < heartGroup.childCount; i++)
        {
            hearts.Push(heartGroup.GetChild(i).gameObject);
        }
        gameOver = GameObject.Find("GameOver");
        if (gameOver != null)
        {
            gameOver.SetActive(false); // Asegura que el GameOver esté oculto al inicio
        }
        Health = maxHealth;
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        // Posicionar la cámara en el target (modo primera persona)
        mainCamera.transform.position = target.transform.position;

        // Inicializar rotación horizontal en 180 grados
        rotationH = 180.0f;
        rotationV = 0.0f;

        // Aplicarla inmediatamente para evitar que el primer Update corrija mal
        mainCamera.transform.rotation = Quaternion.Euler(rotationV, rotationH, 0);
        speed = desiredSpeed;
        if (GameManager.MaskCount == 0 && SceneManager.GetActiveScene().name=="MainScene")
        {
            IsTalking = true;
            StartCoroutine(Talk());
        }
        stepSound = transform.Find("StepSound").GetComponent<AudioSource>();
        punchSound = transform.Find("PunchSound").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            UnityEngine.Cursor.visible = false;
            GameOn = true;
            Time.timeScale = 1;
        }
        if (GameOn)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                UnityEngine.Cursor.lockState = CursorLockMode.None;
                UnityEngine.Cursor.visible = true;
                GameOn = false;
                Time.timeScale = 0;
            }
            if (Time.timeScale != 0 && !_isTalking)
            {
                cameraControl();
                AlignWithCamera();
            }
            /*if (wasDamaged)
            {
                reduceHealth();
            }*/
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                speed = desiredSpeed*2;
            }
            else
            {
                speed = desiredSpeed;
            }
            if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            {
                jumping = true;
            }
        }
    }

    void reduceHealth()
    {
        punchSound.PlayOneShot(punchSound.clip);
        Health -= 1;
        if (hearts.Count > 0)
        {
            GameObject heart = hearts.Pop();
            heart.SetActive(false);
        }
        else
        {
            Debug.LogWarning("No more hearts to remove!");
        }
        if (Health <= minHealth)
        {
            Health = minHealth;
            gameOver.SetActive(true);
            this.gameObject.SetActive(false);
            isDead = true;
            Time.timeScale = 0;
        }
        else
        {
            Debug.Log("Invulnerable for 2 seconds");
            wasDamaged = false;
            if (!isInvulnerable)
            {
                StartCoroutine(InvulnerabilityCoroutine());
            }
        }
    }

    private IEnumerator InvulnerabilityCoroutine()
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(2.0f); // Duración de la invulnerabilidad
        isInvulnerable = false;
    }

    /*void recoverHealth()
    {
        Health += 10;
        if (Health > maxHealth)
        {
            Health = maxHealth;
        }
        Debug.Log("Health Increased to: " + Health);
    }*/

    private void FixedUpdate()
    {
        if (GameOn)
        {
            if (jumping)
            {
                jump();
                jumping = false;
            }
            if (Time.timeScale != 0 && !_isTalking)
            {
                movement();
            }
        }
    }

    void movement()
    {
        transform.position += Input.GetAxis("Vertical") * speed * Time.fixedDeltaTime * transform.forward;
        transform.position += Input.GetAxis("Horizontal") * speed * Time.fixedDeltaTime * transform.right;
        if (stepSound != null)
        {
            if ((Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0) && isGrounded && GameOn)
            {
                stepTimer += Time.fixedDeltaTime;
                float currentStepInterval = (speed > desiredSpeed) ? stepIntervalRunning : stepIntervalWalking;
                if (stepTimer >= currentStepInterval)
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
    }

    void cameraControl()
    {
        // Obtener la posición de destino según el modo de cámara
        Vector3 targetPosition = target.transform.position;
        rotationV -= Input.GetAxis("Mouse Y") * sensitivityV;
        rotationV = Mathf.Clamp(rotationV, -70f, 70f);
        rotationH += Input.GetAxis("Mouse X") * sensitivityH;
        // Modo primera persona: Mantener la cámara fija en el punto de vista del personaje
        mainCamera.transform.position = targetPosition;
        // Rotar la cámara según el input del mouse
        mainCamera.transform.rotation = Quaternion.Euler(rotationV, rotationH, 0);
    }

    void jump()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null && rb.useGravity)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "Mask":
                GameManager.MaskCount++;
                PlayerPrefs.SetInt(other.gameObject.name, 1);
                string maskName = other.gameObject.name;
                Destroy(other.gameObject);
                if (maskName == "ForestMask")
                {
                    ForestManager.PlayScream();
                    CurrentDialogListIndex = 1;
                    IsTalking = true;
                    StartCoroutine(Talk());
                }
                break;
            default:
                break;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("terrain") || collision.gameObject.CompareTag("Terrain2"))
        {
            isGrounded = true;
            if (collision.gameObject.CompareTag("terrain"))
                respawnPosition = transform.position;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("terrain") || collision.gameObject.CompareTag("Terrain2"))
        {
            isGrounded = true;
            stepSound.PlayOneShot(stepSound.clip);
        }
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Weapon"))
        {
            GameObject obj = collision.gameObject;
            Enemy enemy = obj.GetComponent<Enemy>();
            while (enemy == null)
            {
                obj = obj.transform.parent.gameObject;
                enemy = obj.GetComponent<Enemy>();
            }
            if (!isInvulnerable && enemy.CanHurt)
            {
                reduceHealth();
                Vector3 pushDir = (transform.position - collision.transform.position).normalized;
                float pushForce = 10f;
                Rigidbody rb = GetComponent<Rigidbody>();
                rb.AddForce(pushDir * pushForce, ForceMode.Impulse);
            }
        }
        if (collision.gameObject.layer == LayerMask.NameToLayer("Spikes"))
        {
            if (!isInvulnerable)
            {
                reduceHealth();
            }
        }
        if (collision.gameObject.CompareTag("Water"))
        {
            transform.position = respawnPosition;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("terrain"))
        {
            isGrounded = false;
        }
    }

    void AlignWithCamera()
    {
        Vector3 cameraForward = mainCamera.transform.forward;
        cameraForward.y = 0f;
        transform.forward = cameraForward.normalized;
    }
}
