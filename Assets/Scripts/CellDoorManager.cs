using UnityEngine;

public class CellDoorManager : MonoBehaviour
{
    [SerializeField]
    private GameObject guard;
    private Player player;
    private bool playerInRange;
    private bool doorOpen = false;
    private Animator doorAnimator;
    private enum DoorState
    {
        Closed,
        Open
    }
    private DoorState currentState = DoorState.Closed;
    private AudioSource doorSound;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        doorAnimator = GetComponent<Animator>();
        if (PlayerPrefs.GetInt("CellDoorOpened", 0) == 1)
        {
            doorOpen = true;
        }
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        doorSound = GetComponentInChildren<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.C) && !player.IsTalking)
        {
            if (PlayerPrefs.GetInt("HasKey", 0) == 1)
            {
                doorOpen = true;
                PlayerPrefs.SetInt("CellDoorOpened", 1);
                PlayerPrefs.SetInt("HasKey", 0);
                doorSound.PlayOneShot(doorSound.clip);
                UIManager.InteractInstruction.SetActive(false);
                playerInRange = false;
            }
            else
            {
                player.CurrentDialogListIndex = 0;
                player.StartTalking = true;
            }
        }
    }

    private void LateUpdate()
    {
        if (doorOpen && currentState == DoorState.Closed)
        {
            doorAnimator.SetTrigger("OpenUp");
            currentState = DoorState.Open;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && currentState == DoorState.Closed && !guard.activeSelf)
        {
            UIManager.InteractInstruction.SetActive(true);
            playerInRange = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !guard.activeSelf)
        {
            UIManager.InteractInstruction.SetActive(false);
            playerInRange = false;
        }
    }
}
