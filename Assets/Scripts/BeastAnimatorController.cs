using UnityEngine;

public class BeastAnimatorController : MonoBehaviour
{

    ForestBeast forestBeast;
    AudioSource gruntSound;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        forestBeast = GetComponentInParent<ForestBeast>();
        gruntSound = transform.parent.transform.Find("GruntSound").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnStrikeAnimationEnd()
    {
        if (forestBeast != null)
        {
            forestBeast.IsHitting = false;
            forestBeast.RestartStrikeAnimation = true;
        }
    }

    public void OnStrikeAnimationStart()
    {
        if (forestBeast != null)
        {
            forestBeast.IsHitting = true;
        }
        if (gruntSound != null)
        {
            float randomPitch = Random.Range(0.4f, 0.6f);
            gruntSound.pitch = randomPitch;
            gruntSound.PlayOneShot(gruntSound.clip);
        }
    }
}
