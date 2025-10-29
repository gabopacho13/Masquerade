using UnityEngine;
using UnityEngine.Audio;

public class SFXMasterController : MonoBehaviour
{

    public static SFXMasterController Instance { get; private set; }
    public AudioMixer sfx;
    private static AudioSource PingSound;

    void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        PingSound = transform.GetComponentInChildren<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetVolume(float volume)
    {
        sfx.SetFloat("SFXVolume", LinearToDb(volume));
    }

    // API estática para llamar desde cualquier lugar
    public static void SetVolumeStatic(float volume)
    {
        if (Instance == null || Instance.sfx == null)
        {
            Debug.LogError("SFXMasterController no inicializado o sin AudioMixer.");
            return;
        }
        Instance.SetVolume(volume);
    }

    private static float LinearToDb(float linear01)
    {
        if (linear01 <= 0.0001f) return -80f; // silencio
        return Mathf.Log10(linear01) * 20f;
    }

    public static void PlayPing()
    {
        if (PingSound != null)
        {
            if (!PingSound.isPlaying)
            {
                PingSound.Play();
            }
        }
        else
        {
            Debug.LogError("PingSound no está asignado.");
        }
    }
}
