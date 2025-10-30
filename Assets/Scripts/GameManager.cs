using NUnit.Framework;
using TMPro;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    private static GameManager instance;
    public static int MaskCount { get; set; } = 0; // Contador de máscaras recogidas
    private GameObject[] masks;
    public GameObject map;
    [SerializeField]
    private ManholeLid manholeLid;
    private bool wasInSewers = false;
    private GameObject player;
    [SerializeField]
    private GameObject spawnFromSewers;
    public static bool GameOn { get; private set; } = false;
    [SerializeField]
    private GameObject PauseMenu;
    public static bool WaitingForInput = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (map != null)
            map.SetActive(false);
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.LogWarning("Multiple instances of GameManager detected. Destroying old instance.");
            wasInSewers = instance.wasInSewers;
            Debug.Log("wasInSewers value transferred: " + wasInSewers);
            Destroy(instance.gameObject);
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        if (SceneManager.GetActiveScene().name == "Sewers")
        {
            wasInSewers = true;
        }
        MaskCount = 0;
        masks = GameObject.FindGameObjectsWithTag("Mask");
        player = GameObject.FindGameObjectWithTag("Player");
        if (wasInSewers && SceneManager.GetActiveScene().name == "MainScene")
        {
            if (player != null && spawnFromSewers != null)
            {
                player.transform.position = spawnFromSewers.transform.position;
                wasInSewers = false;
            }
            else
            {
                Debug.LogError("Player or spawnFromSewers GameObject not found.");
            }
        }
        if (Buffer.Masks != null && Buffer.Masks.Find(m => m.Name == "ForestMask").Collected)
        {
            MaskCount++;
            if (SceneManager.GetActiveScene().name == "MainScene")
                Destroy(masks.FirstOrDefault(m => m.name == "ForestMask"));
        }
        if (Buffer.Masks != null && Buffer.Masks.Find(m => m.Name == "FoxMask").Collected)
        {
            MaskCount++;
            if (SceneManager.GetActiveScene().name == "MainScene")
            {
                Destroy(masks.FirstOrDefault(m => m.name == "FoxMask"));
                GameObject.Find("Fox").GetComponent<FoxInteraction>().IsMoving = false;
            }
        }
        if (Buffer.Masks != null && Buffer.Masks.Find(m => m.Name == "BeastMask").Collected)
        {
            MaskCount++;
            if (SceneManager.GetActiveScene().name == "MainScene")
            {
                GameObject.Find("CommonBeasts").SetActive(false);
                manholeLid.OpenUp = true;
            }
        }
        if (Buffer.Masks != null && Buffer.Masks.Find(m => m.Name == "StarMask").Collected)
        {
            MaskCount++;
            if (SceneManager.GetActiveScene().name == "MainScene")
            {
                Destroy(masks.FirstOrDefault(m => m.name == "StarMask"));
                GameObject.Find("Star").transform.Find("Buttons").gameObject.SetActive(false);
            }
        }
        if (Buffer.Masks != null && Buffer.Masks.Find(m => m.Name == "ObstacleMask").Collected)
        {
            MaskCount++;
            if (SceneManager.GetActiveScene().name == "MainScene")
            {
                Destroy(masks.FirstOrDefault(m => m.name == "ObstacleMask"));
            GameObject obstacleBeast = GameObject.Find("ObstacleCourseSet").transform.Find("DarkBeast").gameObject;
            obstacleBeast.GetComponentInChildren<Camera>().transform.parent = obstacleBeast.transform.parent;
            Destroy(obstacleBeast);
            }
        }
        if (Buffer.Masks != null && Buffer.Masks.Find(m => m.Name == "InvisiblePathMask").Collected)
        {
            MaskCount++;
            if (SceneManager.GetActiveScene().name == "MainScene")
                Destroy(masks.FirstOrDefault(m => m.name == "InvisiblePathMask"));
        }
        if (Buffer.Masks != null && Buffer.Masks.Find(m => m.Name == "SecretMask").Collected)
        {
            MaskCount++;
            if (SceneManager.GetActiveScene().name == "MainScene")
                Destroy(masks.FirstOrDefault(m => m.name == "SecretMask"));
        }
        if (Buffer.Masks != null && Buffer.Masks.Find(m => m.Name == "SewersMask").Collected)
        {
            MaskCount++;
            if (SceneManager.GetActiveScene().name == "Sewers")
                Destroy(masks.FirstOrDefault(m => m.name == "SewersMask"));
        }

        UIManager.Counter.text = MaskCount.ToString();
        if (UIManager.InteractInstruction.activeSelf)
            UIManager.InteractInstruction.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name != "MainScene" && SceneManager.GetActiveScene().name != "Sewers")
        {
            Destroy(gameObject);
        }
        if (UIManager.Counter.text != MaskCount.ToString())
            UIManager.Counter.text = MaskCount.ToString();
        if (Input.GetKeyDown(KeyCode.M) && map != null)
        {
            if (map.activeSelf)
            {
                map.SetActive(false);
                Time.timeScale = 1;
            }
            else
            {
                map.SetActive(true);
                Time.timeScale = 0;
            }
        }
        if (UIManager.IsGameOver)
        {
            Time.timeScale = 0;
        }
        if (UIManager.IsGameOver && !Cursor.visible)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            GameOn = false;
        }
        if (Input.GetMouseButtonDown(0) && !UIManager.IsGameOver && !PauseMenu.activeSelf && !WaitingForInput)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            GameOn = true;
            Time.timeScale = 1;
        }
        if (GameOn)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                GameOn = false;
                Time.timeScale = 0;
                PauseMenu.SetActive(true);
            }
        }
    }

    public void ContinueGame()
    {
        PauseMenu.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        GameOn = true;
        Time.timeScale = 1;
    }

    public static void SetWaitingForInput(bool newState)
    {
        WaitingForInput = newState;
        if (Cursor.visible != newState)
        {
            Cursor.visible = newState;
            if (newState)
            {
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }

    public static void LoadMasqueradeWithParticipation(bool participate)
    {
        Buffer.Ending3Activated = participate;
        SceneDirector.LoadScene("Masquerade");
    }
}
