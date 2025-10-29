using UnityEngine;

public class NewGameManager : MonoBehaviour
{

    [SerializeField]
    private GameObject warningCanvas;

    public void StartNewGame()
    {
        if (PlayerPrefs.HasKey("DataSaved"))
        {
            warningCanvas.SetActive(true);
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene");
        }
    }


    public void ConfirmNewGame()
    {
        PlayerPrefs.DeleteAll();
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene");
    }

}
