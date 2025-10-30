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
    private CanvasGroup blackScreen;
    [SerializeField]
    private CanvasGroup finalText;
    [SerializeField]
    private TextMeshProUGUI interactInstruction;
    [SerializeField]
    private DialogList narration;
    [SerializeField]
    private CanvasGroup narrationCg;
    [SerializeField]
    private TextMeshProUGUI narrationText;
    [SerializeField]
    private GameObject playerMask;
    [SerializeField]
    private Animator playerAnimator;
    [SerializeField]
    private AudioSource dramaticHit;
    [SerializeField]
    private Transform player;
    private enum PlayerAnimationState
    {
        Idle,
        Walk
    }
    private PlayerAnimationState state;
    private bool finalTextShown = false;
    private float elapsedTime = 0.0f;
    private bool coroutinesStarted = false;
    private bool ending3Started = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!Buffer.Ending3Activated)
        {
            mainCamera.StartCoroutine(mainCamera.SlerpToTarget());
            blackScreen.gameObject.SetActive(true);
            blackScreen.alpha = 1.0f;
        }
        else
        {
            blackScreen.alpha = 0.0f;
            finalText.gameObject.GetComponent<TextMeshProUGUI>().text = "Final 3 de 3\nMascarada";
        }
        skybox.SetFloat("_Exposure", originalExposure);
        playerMask.SetActive(false);
        state = PlayerAnimationState.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        if (!Buffer.Ending3Activated)
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
            if (blackScreen.gameObject.activeSelf && !finalTextShown)
            {
                StartCoroutine(FinalTextFadeIn());
                finalTextShown = true;
            }
        }
        else
        {
            if (!ending3Started)
            {
                ending3Started = true;
                StartCoroutine(Ending3());
            }
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

    private void LateUpdate()
    {
        if (playerAnimator.GetInteger("State") != (int)state)
        {
            playerAnimator.SetInteger("State", (int)state);
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

    private IEnumerator Ending3()
    {
        blackScreen.gameObject.SetActive(true);
        yield return new WaitForSeconds(4f);
        CameraManager.ChangeToCamera("PlayerSideCamera");
        yield return new WaitForSeconds(2f);
        playerMask.SetActive(true);
        yield return new WaitForSeconds(1f);
        state = PlayerAnimationState.Walk;
        StartCoroutine(MovePlayerToFront());
        yield return StartCoroutine(FadeIn(blackScreen));
        state = PlayerAnimationState.Idle;
        CameraManager.ChangeToCamera("PlayerFrontCamera");
        yield return new WaitForSeconds(1f);
        int i = 0;
        foreach (var dialog in narration.dialogs)
        {
            narrationText.text = dialog;
            if (i == 2)
            {
                masqueradeMusic.StartCoroutine(masqueradeMusic.ChangePitch(0.8f));
                StartCoroutine(FadeOut(blackScreen));
            }
            yield return StartCoroutine(FadeIn(narrationCg));
            yield return new WaitForSeconds(2f);
            yield return StartCoroutine(FadeOut(narrationCg));
            yield return new WaitForSeconds(0.5f);
            i++;
        }
        yield return demonDialog.StartCoroutine(demonDialog.ShowDialog());
        yield return new WaitForSeconds(0.5f);
        blackScreen.alpha = 1f;
        dramaticHit.PlayOneShot(dramaticHit.clip);
        yield return StartCoroutine(FinalTextFadeIn());
        finalTextShown = true;
    }

    private IEnumerator FadeIn(CanvasGroup canvasGroup, float duration = 2f)
    {
        float elapsedTime = 0.0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, t);
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }

    private IEnumerator FadeOut(CanvasGroup canvasGroup, float duration = 2f)
    {
        float elapsedTime = 0.0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t);
            yield return null;
        }
        canvasGroup.alpha = 0f;
    }

    private IEnumerator MovePlayerToFront()
    {
        Vector3 startPos = player.position;
        Vector3 endPos = new(startPos.x, startPos.y, startPos.z + 3f);
        float distance = Vector3.Distance(startPos, endPos);
        float moveSpeed = 1.5f;
        float duration = distance / Mathf.Max(0.0001f, moveSpeed);
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            player.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }
        player.position = endPos;
    }
}
