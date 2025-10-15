using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

public class Talker : MonoBehaviour
{
    public List<DialogList> dialogs = new();
    public List<int> dialogStops;
    protected TextMeshProUGUI dialogObject;
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
    protected GameObject textBox;
    protected CanvasGroup cg;
    protected int currentDialogIndex = 0;

    protected virtual void Awake()
    {
        textBox = GameObject.Find("TextBox");
        if (textBox == null)
        {
            Debug.LogError("TextBox GameObject not found in the scene. Please ensure it exists.");
            return;
        }
        dialogObject = textBox.GetComponentInChildren<TextMeshProUGUI>();
        cg = textBox.GetComponent<CanvasGroup>();
        if (cg.alpha != 0)
        {
            cg.alpha = 0; // Asegura que el CanvasGroup esté oculto al inicio
            cg.interactable = false; // Desactiva la interacción con el CanvasGroup
            cg.blocksRaycasts = false; // Desactiva el bloqueo de raycasts
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected virtual IEnumerator Talk(List<AudioClip> audioClips = null, AudioSource audioSource = null)
    {
        if (this is not Player)
        {
            player.GetComponent<Player>().IsTalking = true; // Marca al jugador como hablando
        }
        cg.alpha = 1; // Muestra el CanvasGroup
        for (int i = 0; i < dialogs[_currentDialogListIndex].dialogs.Count; i++)
        {
            if (audioSource != null)
            {
                if (audioClips != null && audioClips.Count > 0)
                {
                    int randomIndex = Random.Range(0, audioClips.Count);
                    AudioClip selectedAudioClip = audioClips[randomIndex];
                    Debug.Log("Playing audio clip: " + selectedAudioClip.name);
                    audioSource.PlayOneShot(selectedAudioClip);
                }
            }
            dialogObject.text = dialogs[_currentDialogListIndex].dialogs[i];
            currentDialogIndex = i;
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
        cg.alpha = 0; // Hide the CanvasGroup
    }

}
