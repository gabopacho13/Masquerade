using System.Collections;
using UnityEngine;

public class BrandonManager : MonoBehaviour
{
    private bool playerInRange = false;
    private Player player;
    private AudioSource sadMusic;
    private bool musicFading = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        sadMusic = GetComponentInChildren<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!player.IsTalking && Buffer.BodySeen && sadMusic.volume > 0f && !musicFading)
        {
            StopAllCoroutines();
            StartCoroutine(FadeSadMusic(false, 3f));
            musicFading = true;
        }
        if (playerInRange && Input.GetKeyDown(KeyCode.C) && !player.IsTalking)
        {
            if (!Buffer.BodySeen)
            {
                player.CurrentDialogListIndex = 1;
                StartCoroutine(FadeSadMusic(true, 3f, maxVol: 0.8f));
                Buffer.BodySeen = true;
                Buffer.SaveGameSync();
            }
            else
            {
                player.CurrentDialogListIndex = 2;
            }
            player.StartTalking = true;
            UIManager.InteractInstruction.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            UIManager.InteractInstruction.SetActive(true);
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            UIManager.InteractInstruction.SetActive(false);
            playerInRange = false;
        }
    }

    private IEnumerator FadeSadMusic(bool fadeIn, float fadeDuration, float maxVol = 1f, float minVol = 0f)
    {
        float time = 0f;
        float startVolume = sadMusic.volume;
        if (fadeIn)
        {
            sadMusic.Play();
            while (time < fadeDuration)
            {
                time += Time.deltaTime;
                float t = time / fadeDuration;
                sadMusic.volume = Mathf.Lerp(startVolume, maxVol, t);
                yield return null;
            }
            sadMusic.volume = maxVol;
        }
        else
        {
            while (time < fadeDuration)
            {
                time += Time.deltaTime;
                float t = time / fadeDuration;
                sadMusic.volume = Mathf.Lerp(startVolume, minVol, t);
                yield return null;
            }
            sadMusic.volume = minVol;
            sadMusic.Stop();
        }
    }
}
