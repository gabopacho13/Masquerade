using UnityEngine;
using UnityEngine.Audio;

public class MusicMasterController : MonoBehaviour
{

    public static MusicMasterController Instance { get; private set; }
    public AudioMixer music;

    void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetVolume(float volume)
    {
        music.SetFloat("MusicVolume", LinearToDb(volume));
    }

    // API estática para llamar desde cualquier lugar
    public static void SetVolumeStatic(float volume)
    {
        if (Instance == null || Instance.music == null)
        {
            Debug.LogError("MusicMasterController no inicializado o sin AudioMixer.");
            return;
        }
        Instance.SetVolume(volume);
    }

    private static float LinearToDb(float linear01)
    {
        if (linear01 <= 0.0001f) return -80f; // silencio
        return Mathf.Log10(linear01) * 20f;
    }
}
