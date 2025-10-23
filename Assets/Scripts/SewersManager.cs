using UnityEngine;

public class SewersManager : MonoBehaviour
{

    private Transform sewersMask;
    private Talker talkerComponent;
    private bool maskActivated = false;
    [SerializeField]
    private Transform guard;

    void Start()
    {
        talkerComponent = guard.GetComponent<Talker>();
        sewersMask = guard.transform.Find("SewersMask");
        sewersMask.gameObject.SetActive(false);
        if (PlayerPrefs.GetInt("SewersMask") == 1)
        {
            guard.gameObject.SetActive(false);
            if (GameObject.Find("InteractInstruction") != null)
                GameObject.Find("InteractInstruction").SetActive(false);
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
