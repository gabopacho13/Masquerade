using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

public class Talker : MonoBehaviour
{
    public List<DialogList> dialogs = new();
    public List<int> dialogStops;
    protected bool _isTalking = false;
    public bool IsTalking { get { return _isTalking; } set { _isTalking = value; } }
    protected GameObject player;
    protected int _currentDialogListIndex = 0;
    public int CurrentDialogListIndex
    {
        get { return _currentDialogListIndex; }
        set
        {
            _currentDialogListIndex = value;
        }
    }
    public int CurrentDialogIndex { get; protected set; } = 0;

    protected virtual IEnumerator Talk(List<AudioClip> audioClips = null, AudioSource audioSource = null)
    {
        if (this is not Player)
        {
            player.GetComponent<Player>().IsTalking = true; // Marca al jugador como hablando
        }
        UIManager.Cg.alpha = 1; // Muestra el CanvasGroup
        for (int i = 0; i < dialogs[_currentDialogListIndex].dialogs.Count; i++)
        {
            if (audioSource != null)
            {
                if (audioClips != null && audioClips.Count > 0)
                {
                    int randomIndex = Random.Range(0, audioClips.Count);
                    AudioClip selectedAudioClip = audioClips[randomIndex];
                    audioSource.PlayOneShot(selectedAudioClip);
                }
            }
            UIManager.DialogObject.text = dialogs[_currentDialogListIndex].dialogs[i];
            CurrentDialogIndex = i;
            yield return new WaitUntil(() => !(Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)));
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0));
        }
        if (!dialogStops.Contains(_currentDialogListIndex) && _currentDialogListIndex + 1 < dialogs.Count)
        {
            _currentDialogListIndex++; // Move to the next dialog list if available
        }
        IsTalking = false; // End talking state
        if (this is not Player)
        {
            player.GetComponent<Player>().IsTalking = false; // Marca al jugador como no hablando
        }
        UIManager.Cg.alpha = 0; // Hide the CanvasGroup
    }

}
