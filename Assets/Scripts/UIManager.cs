using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{

    private static UIManager Instance;
    private static Transform Canvas;
    public static GameObject MiniMap { get; private set; }
    public static GameObject InteractInstruction { get; private set; }
    public static GameObject KeySprite { get; private set; }
    public static GameObject TextBox { get; private set; }
    public static TextMeshProUGUI DialogObject { get; private set; }
    public static TextMeshProUGUI Counter { get; private set; }
    public static GameObject GameOver { get; private set; }
    public static CanvasGroup Cg { get; private set; }
    public static Stack<GameObject> Hearts { get; private set; } = new();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.LogWarning("Multiple instances of UIManager detected. Destroying new instance.");
            Destroy(gameObject);
            return;
        }
        Canvas = transform.Find("UI").transform;
        MiniMap = Canvas.Find("MiniMapContainer").gameObject;
        InteractInstruction = Canvas.Find("InteractInstruction").gameObject;
        KeySprite = Canvas.Find("KeySprite").gameObject;
        TextBox = Canvas.Find("TextBox").gameObject;
        Counter = Canvas.Find("Counter").GetComponent<TextMeshProUGUI>();
        GameOver = Canvas.Find("GameOver").gameObject;
        DialogObject = TextBox.transform.Find("Dialog").GetComponent<TextMeshProUGUI>();
        Cg = TextBox.GetComponent<CanvasGroup>();
        if (Cg.alpha != 0)
        {
            Cg.alpha = 0; // Asegura que el CanvasGroup esté oculto al inicio
            Cg.interactable = false; // Desactiva la interacción con el CanvasGroup
            Cg.blocksRaycasts = false; // Desactiva el bloqueo de raycasts
        }
        foreach (Transform child in Canvas.Find("Hearts"))
        {
            Hearts.Push(child.gameObject);
        }
        InteractInstruction.SetActive(false);
        GameOver.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerPrefs.GetInt("HasKey") == 1)
        {
            KeySprite.SetActive(true);
        }
        else
        {
            KeySprite.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
        {
            if (MiniMap.activeSelf)
            {
                MiniMap.SetActive(false);
            }
            else
            {
                MiniMap.SetActive(true);
            }
        }
    }

    public static void LoseHeart()
    {
        if (Hearts == null || Hearts.Count == 0)
        {
            Debug.LogWarning("No hearts available to lose.");
            return;
        }
        GameObject heartToLose = Hearts.Pop();
        heartToLose.SetActive(false);
    }
}
 