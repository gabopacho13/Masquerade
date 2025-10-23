using NUnit.Framework;
using TMPro;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.SearchService;
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
        if (PlayerPrefs.GetInt("ForestMask") == 1)
        {
            MaskCount++;
            if (SceneManager.GetActiveScene().name == "MainScene")
                Destroy(masks.FirstOrDefault(m => m.name == "ForestMask"));
        }
        if (PlayerPrefs.GetInt("FoxMask") == 1)
        {
            MaskCount++;
            if (SceneManager.GetActiveScene().name == "MainScene")
            {
                Destroy(masks.FirstOrDefault(m => m.name == "FoxMask"));
                GameObject.Find("Fox").GetComponent<FoxInteraction>().IsMoving = false;
            }
        }
        if (PlayerPrefs.GetInt("BeastMask") == 1)
        {
            MaskCount++;
            if (SceneManager.GetActiveScene().name == "MainScene")
            {
                GameObject.Find("CommonBeasts").SetActive(false);
                manholeLid.OpenUp = true;
            }
        }
        if (PlayerPrefs.GetInt("StarMask") == 1)
        {
            MaskCount++;
            if (SceneManager.GetActiveScene().name == "MainScene")
            {
                Destroy(masks.FirstOrDefault(m => m.name == "StarMask"));
                GameObject.Find("Star").transform.Find("Buttons").gameObject.SetActive(false);
            }
        }
        if (PlayerPrefs.GetInt("ObstacleMask") == 1)
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
        if (PlayerPrefs.GetInt("InvisiblePathMask") == 1)
        {
            MaskCount++;
            if (SceneManager.GetActiveScene().name == "MainScene")
                Destroy(masks.FirstOrDefault(m => m.name == "InvisiblePathMask"));
        }
        if (PlayerPrefs.GetInt("SewersMask") == 1)
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
    }
}
