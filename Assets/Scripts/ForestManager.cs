using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using System.Collections.Generic;

public class ForestManager : MonoBehaviour
{

    public GameObject darkBeasts;
    public GameObject forestMask;
    public Material skyBoxMaterial;
    public float initialExposure = 1f;
    public List<Light> directionalLights = new();
    private float initialTemperature;
    private List<float> initialIntensities = new();
    public float newExposure = 0.45f;
    public float newTemperature = 20000f;
    public float newIntensityRatio = 0.23f;
    private static AudioSource dramaticSound;
    private static AudioSource forestScream;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        skyBoxMaterial.SetFloat("_Exposure", initialExposure);
        initialTemperature = directionalLights[0].colorTemperature;
        for (int i = 0; i < directionalLights.Count; i++)
        {
            initialIntensities.Add(directionalLights[i].intensity);
        }
        forestScream = transform.Find("ForestScream").GetComponent<AudioSource>();
        dramaticSound = transform.Find("DramaticSound").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (forestMask == null)
        {
            darkBeasts.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            MusicManager.ChangeMusic("Forest");
            StopAllCoroutines();
            StartCoroutine(ChangeExposure(newExposure, 1f));
            StartCoroutine(ChangeTemperature(newTemperature, 1f));
            StartCoroutine(ChangeIntensity(newIntensityRatio, 1f));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            MusicManager.ChangeMusic("Overworld");
            StopAllCoroutines();
            StartCoroutine(ChangeExposure(initialExposure, 1f));
            StartCoroutine(ChangeTemperature(initialTemperature, 1f));
            StartCoroutine(ChangeIntensity(1f, 1f));
        }
    }

    private IEnumerator ChangeExposure(float target, float duration)
    {
        float time = 0f;
        float currentExposure = skyBoxMaterial.GetFloat("_Exposure");
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            float newExposure = Mathf.Lerp(currentExposure, target, t);
            skyBoxMaterial.SetFloat("_Exposure", newExposure);
            yield return null;
        }
        skyBoxMaterial.SetFloat("_Exposure", target);
    }

    private IEnumerator ChangeTemperature(float target, float duration)
    {
        float time = 0f;
        float currentTemperature = directionalLights[0].colorTemperature;
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            float newTemperature = Mathf.Lerp(currentTemperature, target, t);
            foreach (var light in directionalLights)
            {
                light.colorTemperature = newTemperature;
            }
            yield return null;
        }
        foreach (var light in directionalLights)
        {
            light.colorTemperature = target;
        }
    }

    private IEnumerator ChangeIntensity(float targetRatio, float duration)
    {
        float time = 0f;
        List<float> currentIntensities = new();
        for (int i = 0; i < directionalLights.Count; i++)
        {
            currentIntensities.Add(directionalLights[i].intensity);
        }
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            for (int i = 0; i < directionalLights.Count; i++)
            {
                float newIntensity = Mathf.Lerp(currentIntensities[i], initialIntensities[i] * targetRatio, t);
                directionalLights[i].intensity = newIntensity;
            }
            yield return null;
        }
        for (int i = 0; i < directionalLights.Count; i++)
        {
            directionalLights[i].intensity = initialIntensities[i] * targetRatio;
        }
    }

    public static void PlayScream()
    {
        dramaticSound.Play();
        forestScream.Play();
    }
}
