using UnityEngine;
using UIButton = UnityEngine.UI.Button;

public class ContinueButtonManager : MonoBehaviour
{

    private UIButton button;

    void Start()
    {
        button = GetComponent<UIButton>();
        if (PlayerPrefs.GetInt("DataSaved", 0) == 1)
        {
            button.interactable = true;
        }
        else
        {
            button.interactable = false;
        }
    }
}
