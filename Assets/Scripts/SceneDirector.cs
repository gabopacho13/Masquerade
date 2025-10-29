using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneDirector : MonoBehaviour
{
    private static SceneDirector Instance;
    private static CanvasGroup FadeCanvasGroup;
    private static readonly float FadeDuration = 1.5f;
    private static float ElapsedTime = 0.0f;
    public static bool IsTransitioning = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            var oldInstance = Instance;
            Instance = this;
            Destroy(oldInstance.gameObject);
        }
        else
        {
            Instance = this;
        }
        IsTransitioning = false;
        DontDestroyOnLoad(gameObject);

        FadeCanvasGroup = GetComponentInChildren<CanvasGroup>(true);

        if (FadeCanvasGroup != null)
        {
            FadeCanvasGroup.alpha = 0f;
            FadeCanvasGroup.blocksRaycasts = false;
            FadeCanvasGroup.interactable = false;
        }
        else
        {
            Debug.LogWarning("[SceneDirector] CanvasGroup no encontrado. Se omitirá el fade.");
        }
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 1;
        }
    }

    public static void LoadScene(string sceneName)
    {
        if (IsTransitioning)
        {
            Debug.LogWarning("[SceneDirector] Transición en curso. Ignorando solicitud de carga de escena.");
            return;
        }

        if (Instance == null)
        {
            Debug.LogError("[SceneDirector] No hay instancia inicializada. Cargando escena inmediatamente.");
            SceneManager.LoadScene(sceneName);
            return;
        }

        Instance.StartCoroutine(Instance.FadeAndLoad(sceneName));
    }

    private System.Collections.IEnumerator FadeAndLoad(string sceneName)
    {
        IsTransitioning = true;

        if (FadeCanvasGroup == null)
        {
            SceneManager.LoadScene(sceneName);
            IsTransitioning = false;
            yield break;
        }

        ElapsedTime = 0f;
        FadeCanvasGroup.blocksRaycasts = true;
        while (ElapsedTime < FadeDuration)
        {
            ElapsedTime += Time.unscaledDeltaTime;
            FadeCanvasGroup.alpha = Mathf.Clamp01(ElapsedTime / FadeDuration);
            yield return null;
        }
        FadeCanvasGroup.alpha = 1f;

        SceneManager.LoadScene(sceneName);
        yield return null;

        ElapsedTime = 0f;
        while (ElapsedTime < FadeDuration)
        {
            ElapsedTime += Time.unscaledDeltaTime;
            FadeCanvasGroup.alpha = 1f - Mathf.Clamp01(ElapsedTime / FadeDuration);
            yield return null;
        }
        FadeCanvasGroup.alpha = 0f;
        FadeCanvasGroup.blocksRaycasts = false;

        IsTransitioning = false;
    }

    public static void ExitGame()
    {
        if (IsTransitioning)
            return;
        Instance.StartCoroutine(Instance.FadeAndExit());
    }

    private System.Collections.IEnumerator FadeAndExit()
    {
        IsTransitioning = true;
        if (FadeCanvasGroup == null)
        {
            Application.Quit();
            yield break;
        }
        ElapsedTime = 0f;
        FadeCanvasGroup.blocksRaycasts = true;
        while (ElapsedTime < FadeDuration)
        {
            ElapsedTime += Time.unscaledDeltaTime;
            FadeCanvasGroup.alpha = Mathf.Clamp01(ElapsedTime / FadeDuration);
            yield return null;
        }
        FadeCanvasGroup.alpha = 1f;
        Application.Quit();
    }
}
