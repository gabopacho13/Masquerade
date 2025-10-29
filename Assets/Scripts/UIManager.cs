using System.Collections;
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
    public static CanvasGroup GameOver { get; private set; }
    public static CanvasGroup Cg { get; private set; }
    public static Stack<GameObject> Hearts { get; private set; } = new();
    private static bool isFinalScene = false;
    public static bool IsGameOver { get; private set; } = false;
    private static float GameOverFadeDuration = 1.0f;
    private static float ElapsedGameOverFadeTime = 0.0f;
    private static GameObject GameOverOptions;

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
        GameOver = Canvas.Find("GameOver").GetComponent<CanvasGroup>();
        GameOverOptions = GameOver.transform.Find("GameOverOptions").gameObject;
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
        int loadedHearts = Buffer.health;
        for (int i = 0; i < Hearts.Count; i++)
        {
            if (i >= loadedHearts)
            {
                LoseHeart();
            }
        }
        InteractInstruction.SetActive(false);
        GameOver.alpha = 0;
        GameOverOptions.SetActive(false);
        IsGameOver = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            Destroy(gameObject);
        }
        if (SceneManager.GetActiveScene().name == "Masquerade" && !isFinalScene)
        {
            foreach(Transform child in Canvas)
            {
                child.gameObject.SetActive(false);
            }
            isFinalScene = true;
        }
        else
        {
            if (Buffer.HasKey && !KeySprite.activeSelf)
            {
                KeySprite.SetActive(true);
            }
            else if (!Buffer.HasKey && KeySprite.activeSelf)
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

    public static void GameOverRoutine()
    {
        MusicManager.ChangeMusic("GameOver");
        Instance.StartCoroutine(Instance.ShowGameOver());
    }

    private IEnumerator ShowGameOver()
    {
        ElapsedGameOverFadeTime = 0.0f;
        while (ElapsedGameOverFadeTime < GameOverFadeDuration)
        {
            ElapsedGameOverFadeTime += Time.deltaTime;
            GameOver.alpha = Mathf.Clamp01(ElapsedGameOverFadeTime / GameOverFadeDuration);
            yield return null;
        }
        GameOver.alpha = 1.0f;
        yield return new WaitForSeconds(2.0f);
        IsGameOver = true;
        GameOver.interactable = true;
        GameOver.blocksRaycasts = true;
        GameOverOptions.SetActive(true);
    }

    public static void ResetGameOver()
    {
        Instance.StartCoroutine(Instance.ResetStuff());
    }

    private IEnumerator ResetStuff()
    {
        yield return new WaitUntil(() => !SceneDirector.IsTransitioning);
        GameOver.interactable = false;
        GameOver.blocksRaycasts = false;
        GameOver.alpha = 0.0f;
        GameOverOptions.SetActive(false);
        foreach (Transform child in Canvas.Find("Hearts"))
        {
            child.gameObject.SetActive(true);
            Hearts.Push(child.gameObject);
        }
        IsGameOver = false;
    }
}
 