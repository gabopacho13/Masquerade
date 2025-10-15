using System.Collections.Generic;
using UnityEngine;

public class StarManager : MonoBehaviour
{
    private Villager starVillager;
    private GameObject starMask;
    public List<GameObject> buttonsInOrder;
    public GameObject TeleportPoint;
    private static Queue<GameObject> buttonQueue = new();
    private static bool pressedInOrder = true;
    private GameObject player;
    private static StarManager instance;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Keep this instance across scenes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances
            return;
        }
        starVillager = transform.Find("StarVillager").GetComponent<Villager>();
        starMask = transform.Find("StarMask").gameObject;   
        if (starVillager == null)
        {
            Debug.LogError("StarVillager GameObject not found in ButtonManager.");
        }
        if (starMask == null)
        {
            Debug.LogError("StarMask GameObject not found in ButtonManager.");
        }
        else
        {
            starMask.SetActive(false); // Initially hide the star mask
        }
        if (buttonsInOrder == null || buttonsInOrder.Count == 0)
        {
            Debug.LogError("Buttons in order not set in StarManager.");
        }
        else
        {
            LoadQueue();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (buttonQueue.Count == 0 && starMask != null && !starMask.activeSelf)
        {
            player.transform.position = TeleportPoint.transform.position; // Teleport the player to the teleport point
            if (pressedInOrder == true)
            {
                starMask.SetActive(true);
            }
            else
            {
                LoadQueue();
            }
        }
        if (starVillager.CurrentDialogListIndex < 1 && (starMask == null || starMask.activeSelf))
        {
            starVillager.CurrentDialogListIndex = 2;
        }
    }

    public static void VerifyButton(GameObject button)
    {
        if (button != buttonQueue.Dequeue())
        {
            pressedInOrder = false;
        }
    }

    private void LoadQueue()
    {
        foreach (GameObject button in buttonsInOrder)
        {
            button.SetActive(true);
            buttonQueue.Enqueue(button);
        }
        pressedInOrder = true;
    }
}
