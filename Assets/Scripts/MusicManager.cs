using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;
using System.Linq;

public class MusicManager : MonoBehaviour
{

    private static MusicManager instance;
    private List<AudioSource> audioSources;
    private List<float> maxVol;
    private static AudioMixer musicMixer;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        audioSources = transform.GetComponentsInChildren<AudioSource>().ToList();
        maxVol = new List<float>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < audioSources.Count; i++)
        {
            maxVol.Add(audioSources[i].volume);
            if (i == 0)
            {
                audioSources[i].volume = maxVol[i];
                audioSources[i].Play();
                musicMixer = audioSources[i].outputAudioMixerGroup.audioMixer;
            }
            else
            {
                audioSources[i].volume = 0f;
                audioSources[i].Stop();
            }
        }
    }

    public static void ChangeMusic(string musicName)
    {
        AudioSource currentMusic = null;
        AudioSource newMusic = null;
        float newMaxVol = 1f;
        instance.StopAllCoroutines();
        foreach (var source in instance.audioSources)
        {
            if (source.isPlaying && source.name != musicName)
            {
                currentMusic = source;
            }
            if (source.clip.name == musicName)
            {
                newMusic = source;
                newMaxVol = instance.maxVol[instance.audioSources.IndexOf(source)];
            }
            if (currentMusic != null && newMusic != null)
            {
                break;
            }
        }
        if (newMusic == null)
        {
            Debug.LogWarning($"Music with name {musicName} not found. Maintaining current music instead.");
            return;
        }
        else if (currentMusic == null)
        {
            Debug.LogWarning("Couldn't find current music. Playing new music directly");
            newMusic.Play();
        }
        else
        {
            instance.StartCoroutine(instance.CrossfadeMusic(currentMusic, newMusic, 2f, newMaxVol));
        }
    }

    private IEnumerator CrossfadeMusic(AudioSource from, AudioSource to, float duration, float newMaxVol)
    {
        float time = 0f;
        float fromVolume = from.volume;
        float toVolume = to.volume;
        to.Play();
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            from.volume = Mathf.Lerp(fromVolume, 0f, t);
            to.volume = Mathf.Lerp(toVolume, newMaxVol, t);
            yield return null;
        }
        from.Stop();
        to.volume = newMaxVol;
    }

    private static List<AudioSource> GetCurrentMusic()
    {
        List<AudioSource> playingSources = new();
        foreach (var source in instance.audioSources)
        {
            if (source.isPlaying)
            {
                playingSources.Add(source);
            }
        }
        return instance.audioSources;
    }

    public static void ChangePitch(float newPitch)
    {
        instance.StopAllCoroutines();
        instance.StartCoroutine(instance.ChangePitch(newPitch, 1f));
    }

    private IEnumerator ChangePitch(float newPitch, float duration)
    {
        float initialPitch = 1f;
        musicMixer.GetFloat("MasterPitch", out initialPitch);
        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            float currentpitch = Mathf.Lerp(initialPitch, newPitch, t);
            musicMixer.SetFloat("MasterPitch", currentpitch);
            yield return null;
        }
        musicMixer.SetFloat("MasterPitch", newPitch);
    }
}
