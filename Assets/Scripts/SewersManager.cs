using UnityEngine;

public class SewersManager : MonoBehaviour
{

    private Transform sewersMask;
    private Talker talkerComponent;
    private bool maskActivated = false;

    void Start()
    {
        talkerComponent = GetComponent<Talker>();
        sewersMask = transform.Find("SewersMask");
        sewersMask.gameObject.SetActive(false);
        if (PlayerPrefs.GetInt("SewersMask") == 1)
        {
            //GameObject.Find("InteractInstruction").SetActive(false);
            this.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (talkerComponent.CurrentDialogListIndex == 0 && talkerComponent.CurrentDialogIndex == 2 && !maskActivated)
        {
            sewersMask.gameObject.SetActive(true);
            maskActivated = true;
        }
    }
}
