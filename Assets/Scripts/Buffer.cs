using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Unity.VisualScripting;


public class Buffer : MonoBehaviour
{

    private static Buffer Instance;
    public static List<Mask> Masks=new();
    public static bool HasKey=false;
    public static bool BodySeen=false;
    public static bool CellDoorOpened=false;
    public static int health = 5;
    public static bool SpawnedFromSave = false;
    public static float PlayerPosX = 0f;
    public static float PlayerPosY = 0f;
    public static float PlayerPosZ = 0f;

    public class Mask
    {
        public string Name;
        public bool Collected;
        public Mask(string name, bool collected)
        {
            Name = name;
            Collected = collected;
        }

        public void SetCollected(bool collected)
        {
            Collected = collected;
        }
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
        Masks.Add(new Mask("ForestMask", false));
        Masks.Add(new Mask("FoxMask", false));
        Masks.Add(new Mask("StarMask", false));
        Masks.Add(new Mask("BeastMask", false));
        Masks.Add(new Mask("InvisiblePathMask", false));
        Masks.Add(new Mask("ObstacleMask", false));
        Masks.Add(new Mask("SewersMask", false));
        Masks.Add(new Mask("SecretMask", false));
    }

    private void Update()
    {
        if (health <= 0)
        {
            health = 5;
            SpawnedFromSave = false;
        }
    }

    public static void SaveAndExit()
    {
        SaveGameSync();
        SceneDirector.LoadScene("MainMenu");
    }

    public static void LoadAndContinue()
    {
        LoadGameSync();
        SceneDirector.LoadScene(PlayerPrefs.GetString("CurrentScene", "MainScene"));
    }

    public static void ResetBuffer()
    {
        HasKey = false;
        BodySeen = false;
        CellDoorOpened = false;
        for (int i = 0; i < Masks.Count; i++)
        {
            Masks[i].SetCollected(false);
        }
    }

    public static void SaveGameSync()
    {
        foreach (var mask in Masks)
        {
            PlayerPrefs.SetInt(mask.Name, mask.Collected ? 1 : 0);
        }

        PlayerPrefs.SetInt("HasKey", HasKey ? 1 : 0);
        PlayerPrefs.SetInt("BodySeen", BodySeen ? 1 : 0);
        PlayerPrefs.SetInt("CellDoorOpened", CellDoorOpened ? 1 : 0);
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) {
            PlayerPrefs.SetFloat("PlayerPosX", player.transform.position.x);
            PlayerPrefs.SetFloat("PlayerPosY", player.transform.position.y);
            PlayerPrefs.SetFloat("PlayerPosZ", player.transform.position.z);
            PlayerPrefs.SetInt("PlayerHealth", health);
            PlayerPrefs.SetInt("PlayerInfoSaved", 1);
        }
        PlayerPrefs.SetString("CurrentScene", UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        PlayerPrefs.SetInt("DataSaved", 1);
        PlayerPrefs.Save();
    }

    public static void LoadGameSync()
    {
        foreach (var mask in Masks)
        {
            int collected = PlayerPrefs.GetInt(mask.Name, 0);
            mask.SetCollected(collected == 1);
        }
        HasKey = PlayerPrefs.GetInt("HasKey", 0) == 1;
        BodySeen = PlayerPrefs.GetInt("BodySeen", 0) == 1;
        CellDoorOpened = PlayerPrefs.GetInt("CellDoorOpened", 0) == 1;
        PlayerPosX = PlayerPrefs.GetFloat("PlayerPosX", 0f);
        PlayerPosY = PlayerPrefs.GetFloat("PlayerPosY", 0f);
        PlayerPosZ = PlayerPrefs.GetFloat("PlayerPosZ", 0f);
        health = PlayerPrefs.GetInt("PlayerHealth", 5);
        if (PlayerPrefs.GetInt("PlayerInfoSaved", 0) == 1)
        {
            SpawnedFromSave = true;
        }
        else
        {
            SpawnedFromSave = false;
        }
    }
}
