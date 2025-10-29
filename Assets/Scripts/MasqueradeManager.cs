using System.Collections;
using TMPro;
using UnityEngine;

public class MasqueradeManager : MonoBehaviour
{
    [SerializeField]
    private MasqueradeCamera mainCamera;
    [SerializeField]
    private Demon demon;
    [SerializeField]
    private MasqueradeMusic masqueradeMusic;
    [SerializeField]
    private float waitDuration = 2.0f;
    [SerializeField]
    private DemonDialog demonDialog;
    [SerializeField]
    private FireManager fireManager;
    [SerializeField]
    private Material skybox;
    [SerializeField]
    private float originalExposure = 0.35f;
    [SerializeField]
    private GameObject blackScreen;
    [SerializeField]
    private CanvasGroup finalText;
    [SerializeField]
    private TextMeshProUGUI interactInstruction;
    private bool finalTextShown = false;
    private float elapsedTime = 0.0f;
    private bool coroutinesStarted = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainCamera.StartCoroutine(mainCamera.SlerpToTarget());
        skybox.SetFloat("_Exposure", originalExposure);
    }

    // Update is called once per frame
    void Update()
    {
        if (mainCamera.SlerpToTargetFinished && !coroutinesStarted)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= waitDuration)
            {
                StartCoroutine(StartMultipleCoroutines());
                coroutinesStarted = true;
            }
        }
        if (blackScreen.activeSelf && !finalTextShown)
        {
            StartCoroutine(FinalTextFadeIn());
            finalTextShown = true;
        }
        if (finalText.alpha == 1f && !interactInstruction.gameObject.activeSelf)
        {
            interactInstruction.gameObject.SetActive(true);
        }
        if (Input.anyKeyDown && interactInstruction.gameObject.activeSelf)
        {
            SceneDirector.LoadScene("MainMenu");
        }
    }

    private IEnumerator StartMultipleCoroutines()
    {
        StartCoroutine(DecreaseExposure());
        demon.StartCoroutine(demon.ReScale());
        masqueradeMusic.StartCoroutine(masqueradeMusic.ChangePitch());
        fireManager.StartSwapToFinal();
        yield return new WaitUntil(() => demon.RescaleFinished);
        yield return demonDialog.StartCoroutine(demonDialog.ShowDialog());
        demon.Attack = true;
    }

    private IEnumerator DecreaseExposure()
    {
        float duration = 4.0f;
        float elapsedTime = 0.0f;
        float startingExposure = originalExposure;
        float targetExposure = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            skybox.SetFloat("_Exposure", Mathf.Lerp(startingExposure, targetExposure, t));
            yield return null;
        }
        skybox.SetFloat("_Exposure", targetExposure);
    }

    private IEnumerator FinalTextFadeIn()
    {
        float duration = 5.0f;
        float elapsedTime = 0.0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            finalText.alpha = Mathf.Lerp(0f, 1f, t);
            yield return null;
        }
        finalText.alpha = 1f;
    }
}
