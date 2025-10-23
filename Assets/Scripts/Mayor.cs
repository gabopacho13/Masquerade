using NUnit.Framework;
using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class Mayor : Character
{
    public List<Material> materials;
    public Renderer face;
    private bool finishedCutscene = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
        animator.SetInteger("State", 0);
        voice = transform.Find("Voice").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (CurrentDialogListIndex > 0 && CurrentDialogListIndex < 2 && Vector3.Distance(player.transform.position, this.transform.position) > 3.0f)
        {
            CurrentDialogListIndex = 2;
            StartTalking = true;
        }

        if (UIManager.Counter.GetComponent<TextMeshProUGUI>().text == "7")
        {
            CurrentDialogListIndex = 5;
        }

        if (IsTalking)
        {
            if (CurrentDialogListIndex == 0)
            {
                finishedCutscene = false;
                if (UIManager.DialogObject.text == dialogs[0].dialogs[2])
                {
                    if (CameraManager.GetActiveCamera().name != "IntroMaskCamera" && GameObject.Find("ObstacleMask") != null)
                    {
                        CameraManager.ChangeToCamera("IntroMaskCamera");
                    }
                }
                else if (UIManager.DialogObject.text == dialogs[0].dialogs[3])
                {
                    if (CameraManager.GetActiveCamera().name != "IntroBeastCamera")
                    {
                        CameraManager.ChangeToCamera("IntroBeastCamera");
                    }
                }
                else if (UIManager.DialogObject.text == dialogs[0].dialogs[6])
                {
                    if (CameraManager.GetActiveCamera().name != "IntroVillagerCamera")
                    {
                        CameraManager.ChangeToCamera("IntroVillagerCamera");
                    }
                }
                else
                {
                    if (CameraManager.GetActiveCamera().name != "Main Camera")
                    {
                        CameraManager.ChangeToCamera("Main Camera");
                    }
                }
            }
            if (UIManager.DialogObject.text == dialogs[0].dialogs[1] || UIManager.DialogObject.text == dialogs[0].dialogs[2])
            {
                face.material = materials[1];
            }
            else if (UIManager.DialogObject.text == dialogs[0].dialogs[3] || dialogs[3].dialogs.Contains(UIManager.DialogObject.text))
            {
                face.material = materials[2];
            }
            else if (UIManager.DialogObject.text == dialogs[0].dialogs[4])
            {
                face.material = materials[3];
            }
            else if (UIManager.DialogObject.text == dialogs[0].dialogs[5] || UIManager.DialogObject.text == dialogs[0].dialogs[6])
            {
                face.material = materials[4];
            }
            else
            {
                face.material = materials[0];
            }
        }
        else
        {
            face.material = materials[0];
            if (!finishedCutscene && CameraManager.GetActiveCamera().name != "Main Camera")
            {
                CameraManager.ChangeToCamera("Main Camera");
                finishedCutscene = true;
            }
        }
    }
}