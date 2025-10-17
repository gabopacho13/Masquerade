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
    private TextMeshProUGUI counter;
    public static int MaskCount { get; set; } = 0; // Contador de máscaras recogidas
    private GameObject[] masks;
    public GameObject map;
    public GameObject miniMap;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (map != null)
            map.SetActive(false);
        if (miniMap != null)
            miniMap.SetActive(true);
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.LogWarning("Multiple instances of GameManager detected. Destroying the new instance.");
            Destroy(gameObject);
        }
        counter = GameObject.Find("Counter").GetComponent<TextMeshProUGUI>();
        masks = GameObject.FindGameObjectsWithTag("Mask");
        if (PlayerPrefs.GetInt("ForestMask") == 1)
        {
            MaskCount++;
            Destroy(masks.FirstOrDefault(m => m.name == "ForestMask"));
        }
        if (PlayerPrefs.GetInt("FoxMask") == 1)
        {
            MaskCount++;
            Destroy(masks.FirstOrDefault(m => m.name == "FoxMask"));
            GameObject.Find("Fox").GetComponent<FoxInteraction>().IsMoving = false;
        }
        if (PlayerPrefs.GetInt("BeastMask") == 1)
        {
            MaskCount++;
            GameObject.Find("CommonBeasts").SetActive(false);
        }
        if (PlayerPrefs.GetInt("StarMask") == 1)
        {
            MaskCount++;
            Destroy(masks.FirstOrDefault(m => m.name == "StarMask"));
            GameObject.Find("Star").transform.Find("Buttons").gameObject.SetActive(false);
        }
        if (PlayerPrefs.GetInt("ObstacleMask") == 1)
        {
            MaskCount++;
            Destroy(masks.FirstOrDefault(m => m.name == "ObstacleMask"));
            GameObject obstacleBeast = GameObject.Find("ObstacleCourseSet").transform.Find("DarkBeast").gameObject;
            obstacleBeast.GetComponentInChildren<Camera>().transform.parent = obstacleBeast.transform.parent;
            Destroy(obstacleBeast);
        }
        if (PlayerPrefs.GetInt("InvisiblePathMask") == 1)
        {
            MaskCount++;
            Destroy(masks.FirstOrDefault(m => m.name == "InvisiblePathMask"));
        }
        if (PlayerPrefs.GetInt("SewersMask") == 1)
        {
            MaskCount++;
            Destroy(masks.FirstOrDefault(m => m.name == "SewersMask"));
        }
        if (counter != null)
        {
            counter.text = MaskCount.ToString();
        }
        else
        {
            Debug.LogError("Counter TextMeshProUGUI not found in GameManager.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (counter.text != MaskCount.ToString())
            counter.text = MaskCount.ToString();
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
        if ((Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl)) && miniMap != null)
        {
            if (miniMap.activeSelf)
            {
                miniMap.SetActive(false);
            }
            else
            {
                miniMap.SetActive(true);
            }
        }
    }
}
