using System.Collections;
using UnityEngine;

public class DemonDialog : MonoBehaviour
{

    public bool HasBeenShown { get; private set; } = false;
    private CanvasGroup canvasGroup;
    [SerializeField]
    private float fadeDuration = 0.5f;
    private float elapsedTime = 0.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public IEnumerator ShowDialog()
    {
        elapsedTime = 0.0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            canvasGroup.alpha = alpha;
            yield return null;
        }
        canvasGroup.alpha = 1.0f;
        yield return new WaitForSeconds(1.0f);
        yield return StartCoroutine(HideDialog());
        HasBeenShown = true;
    }

    public IEnumerator HideDialog()
    {
        elapsedTime = 0.0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = 1.0f - Mathf.Clamp01(elapsedTime / fadeDuration);
            canvasGroup.alpha = alpha;
            yield return null;
        }
        canvasGroup.alpha = 0.0f;
    }
}
