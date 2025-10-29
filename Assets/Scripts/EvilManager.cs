using UnityEngine;

public class EvilManager : MonoBehaviour
{

    private static GameObject evilVillagers;
    public static bool AreEvilVillagersActive => evilVillagers.activeSelf;
    private GameObject key;
    private AudioSource evilScream;

    void Awake()
    {
        key = transform.Find("Key").gameObject;
        evilScream = transform.Find("EvilScream").GetComponent<AudioSource>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        evilVillagers = transform.Find("EvilVillagers").gameObject;
        evilVillagers.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Buffer.HasKey)
        {
            key.SetActive(false);
        }
        else
        {
            key.SetActive(true);
        }
    }

    public static void RestartVillagers()
    {
        foreach (Transform villager in evilVillagers.transform)
        {
            EvilVillager ev = villager.GetComponent<EvilVillager>();
            villager.position = ev.InitialPosition;
            villager.rotation = ev.InitialRotation;
        }
        evilVillagers.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!evilVillagers.activeSelf)
                evilScream.PlayOneShot(evilScream.clip);
            evilVillagers.SetActive(true);
        }
    }
}
