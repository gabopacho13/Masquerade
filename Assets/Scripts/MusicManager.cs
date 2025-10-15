using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
{

    private static MusicManager instance;
    private AudioSource[] audioSources;
    private static AudioMixer musicMixer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        audioSources = transform.GetComponentsInChildren<AudioSource>();
        for (int i = 0; i < audioSources.Length; i++)
        {
            if (i == 0)
            {
                audioSources[i].volume = 1f;
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

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void ChangeMusic(string musicName)
    {
        AudioSource currentMusic = null;
        AudioSource newMusic = null;
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
            instance.StartCoroutine(instance.CrossfadeMusic(currentMusic, newMusic, 2f));
        }
    }

    private IEnumerator CrossfadeMusic(AudioSource from, AudioSource to, float duration)
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
            to.volume = Mathf.Lerp(toVolume, 1f, t);
            yield return null;
        }
        from.Stop();
        to.volume = 1f;
    }

    private static AudioSource[] GetCurrentMusic()
    {
        AudioSource[] playingSources = System.Array.FindAll(instance.audioSources, source => source.isPlaying);
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
