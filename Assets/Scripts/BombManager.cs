using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class BombManager : MonoBehaviour
{
    public GameObject BombBody { get; private set; }
    private List<ParticleSystem> explosion = new();
    private float timeToExplode;
    private Rigidbody rb;
    private float currentTime = 0f;
    private bool exploded = false;
    private AudioSource fuseSound;
    private AudioSource explosionSound;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        BombBody = transform.Find("Bomb").gameObject;
        foreach (Transform child in transform)
        {
            if (child.CompareTag("ExplosionSmoke"))
            {
                explosion.Add(child.GetComponent<ParticleSystem>());
            }
        }
        rb = this.GetComponent<Rigidbody>();
        timeToExplode = Mathf.Sqrt(-(2*transform.position.y)/Physics.gravity.y);
        fuseSound = transform.Find("FuseSound").GetComponent<AudioSource>();
        explosionSound = transform.Find("ExplosionSound").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void FixedUpdate()
    {
        currentTime += Time.fixedDeltaTime;
        if (currentTime >= timeToExplode && !exploded)
        {
            foreach (ParticleSystem ps in explosion)
            {
                ps.Play();
            }
            BombBody.SetActive(false);
            fuseSound.Stop();
            explosionSound.PlayOneShot(explosionSound.clip);
            rb.isKinematic = true;
            exploded = true;
            Destroy(this.gameObject, explosion[0].main.duration+1f);
        }
    }
}
