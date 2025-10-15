using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class BeastManager : MonoBehaviour
{

    private static BeastManager instance;
    public GameObject maskPrefab;
    private static List<GameObject> beasts = new();
    private static AudioSource dramaticSound;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("Multiple instances of BeastManager detected. Destroying the new instance.");
            Destroy(gameObject);
        }
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).gameObject.activeSelf)
                beasts.Add(transform.GetChild(i).gameObject);
        }
        dramaticSound = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static int VerifyActive(GameObject defeatedBeast)
    {
        beasts.Remove(defeatedBeast);
        return beasts.Count;
        /*if (beasts.Count == 0)
        {
            GameObject newMask = Instantiate(instance.maskPrefab, defeatedBeast.transform.position + Vector3.up*1.5f, Quaternion.identity);
            newMask.name = "BeastMask";
            newMask.tag = "Mask";
            newMask.layer = LayerMask.NameToLayer("Masks");
        }*/
    }

    public static void instantiateMask(GameObject defeatedBeast)
    {
        if (instance != null && instance.maskPrefab != null)
        {
            GameObject newMask = Instantiate(instance.maskPrefab, defeatedBeast.transform.position + Vector3.up * 1.5f, Quaternion.identity);
            newMask.name = "BeastMask";
            newMask.tag = "Mask";
            newMask.layer = LayerMask.NameToLayer("Masks");
        }
        else
        {
            Debug.LogError("BeastManager or maskPrefab is not initialized.");
        }
    }

    public static void PlayDramaticSound()
    {
        if (dramaticSound != null && !dramaticSound.isPlaying)
        {
            dramaticSound.PlayOneShot(dramaticSound.clip);
        }
    }
}
