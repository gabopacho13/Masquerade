using UnityEngine;
using System.Collections;

public class MasqueradeMusic : MonoBehaviour
{

    [SerializeField]
    private float targetPitch = -0.12f;
    [SerializeField]
    private float pitchChangeDuration = 2.0f;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = 0f;
        StartCoroutine(FadeInMusic());
    }

    public IEnumerator ChangePitch()
    {
        float elapsedTime = 0.0f;
        float t = 0;
        float startingPitch = audioSource.pitch;
        while (elapsedTime < pitchChangeDuration)
        {
            elapsedTime += Time.deltaTime;
            t = elapsedTime / pitchChangeDuration;
            audioSource.pitch = Mathf.Lerp(startingPitch, targetPitch, t);
            yield return null;
        }
        audioSource.pitch = targetPitch;
    }

    private IEnumerator FadeInMusic()
    {
        float fadeDuration = 2.0f;
        float elapsedTime = 0.0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            audioSource.volume = Mathf.Clamp01(elapsedTime / fadeDuration);
            yield return null;
        }
        audioSource.volume = 1.0f;
    }
}
