using System.Collections;
using UnityEngine;

public class Car : MonoBehaviour
{

    private bool playerInRange = false;
    private AudioSource carAudioSource;
    [SerializeField]
    private Player player;

    private void Start()
    {
        carAudioSource = GetComponentInChildren<AudioSource>();
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.C))
        {
            StartCoroutine(StartCar());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (Buffer.BodySeen)
            {
                UIManager.InteractInstruction.SetActive(true);
                playerInRange = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (Buffer.BodySeen)
            {
                UIManager.InteractInstruction.SetActive(false);
                playerInRange = false;
            }
        }
    }

    private IEnumerator StartCar()
    {
        CameraManager.ChangeToCamera("CarCamera");
        carAudioSource.Play();
        player.gameObject.SetActive(false);
        UIManager.InteractInstruction.SetActive(false);
        yield return new WaitForSeconds(2f);
        MusicManager.FadeOut();
        StartCoroutine(FadeOutCarSound());
        SceneDirector.LoadScene("BurnedVillage");
    }

    private IEnumerator FadeOutCarSound()
    {
        float startVolume = carAudioSource.volume;
        float duration = 2f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            carAudioSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
            yield return null;
        }
        carAudioSource.Stop();
        carAudioSource.volume = startVolume;
    }
}
