using System.Collections;
using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class BurnedVillageManager : MonoBehaviour
{

    [SerializeField]
    private Camera burnedVillageCamera;
    [SerializeField]
    private CanvasGroup canvas;
    [SerializeField]
    private CanvasGroup textCG;
    [SerializeField]
    private TextMeshProUGUI dialogText;
    [SerializeField]
    private DialogList burnedVillageDialogList;
    [SerializeField]
    private float fadeDuration = 2f;
    private float cameraCenterY;
    [SerializeField]
    private float cameraRotationSpeed = 10f;
    [SerializeField]
    private float cameraRotationAngle = 72f;
    [SerializeField]
    private CanvasGroup endingText;
    [SerializeField]
    private TextMeshProUGUI interactInstruction;
    [SerializeField]
    private List<AudioSource> fireSound;

    void Start()
    {
        textCG.alpha = 0;
        dialogText.text = "";
        cameraCenterY = burnedVillageCamera.transform.rotation.eulerAngles.y;
        burnedVillageCamera.transform.rotation = Quaternion.Euler(burnedVillageCamera.transform.rotation.eulerAngles.x, cameraCenterY - cameraRotationAngle / 2f, burnedVillageCamera.transform.rotation.eulerAngles.z);
        StartCoroutine(Cinematic());
    }

    void Update()
    {
        if (interactInstruction.gameObject.activeSelf && Input.anyKeyDown)
        {
            SceneDirector.LoadScene("MainMenu");
        }
    }

    private IEnumerator Cinematic()
    {
        foreach (string dialog in burnedVillageDialogList.dialogs)
        {
            dialogText.text = dialog;
            yield return StartCoroutine(FadeIn(textCG));
            if (dialog != burnedVillageDialogList.dialogs[burnedVillageDialogList.dialogs.Count - 1])
            {
                yield return new WaitForSeconds(2f);
                yield return StartCoroutine(FadeOut(textCG));
            }
            yield return new WaitForSeconds(0.5f);
        }
        StartCoroutine(FadeOut(canvas));
        StartCoroutine(FadeInSound());
        yield return new WaitForSeconds(1f);
        StartCoroutine(FadeOut(textCG));
        yield return StartCoroutine(PanCamera());
        StartCoroutine(FadeOutSound());
        yield return StartCoroutine(FadeIn(canvas));
        yield return StartCoroutine(FadeIn(endingText));
        yield return new WaitForSeconds(1.5f);
        interactInstruction.gameObject.SetActive(true);
    }

    private IEnumerator FadeIn(CanvasGroup canvasGroup)
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }

    private IEnumerator FadeOut(CanvasGroup canvasGroup)
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = 1f - Mathf.Clamp01(elapsedTime / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 0f;
    }

    private IEnumerator PanCamera()
    {
        float targetAngle = cameraCenterY + cameraRotationAngle / 2f;
        while (Mathf.Abs(Mathf.DeltaAngle(burnedVillageCamera.transform.rotation.eulerAngles.y, targetAngle)) > 0.1f)
        {
            float newY = Mathf.MoveTowardsAngle(burnedVillageCamera.transform.rotation.eulerAngles.y, targetAngle, cameraRotationSpeed * Time.deltaTime);
            burnedVillageCamera.transform.rotation = Quaternion.Euler(burnedVillageCamera.transform.rotation.eulerAngles.x, newY, burnedVillageCamera.transform.rotation.eulerAngles.z);
            yield return null;
        }
        burnedVillageCamera.transform.rotation = Quaternion.Euler(burnedVillageCamera.transform.rotation.eulerAngles.x, targetAngle, burnedVillageCamera.transform.rotation.eulerAngles.z);
    }

    private IEnumerator FadeInSound()
    {
        float elapsedTime = 0f;
        foreach (AudioSource fireSound in fireSound)
        {
            fireSound.volume = 0f;
            fireSound.Play();
        }
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            foreach (AudioSource fireSound in fireSound)
            {
                fireSound.volume = Mathf.Clamp01(elapsedTime / fadeDuration);
            }
            yield return null;
        }
        foreach (AudioSource fireSound in fireSound)
        {
            fireSound.volume = 1f;
        }
    }

    private IEnumerator FadeOutSound()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            foreach (AudioSource fireSound in fireSound)
            {
                fireSound.volume = 1f - Mathf.Clamp01(elapsedTime / fadeDuration);
            }
            yield return null;
        }
        foreach (AudioSource fireSound in fireSound)
        {
            fireSound.volume = 0f;
            fireSound.Stop();
        }
    }
}
