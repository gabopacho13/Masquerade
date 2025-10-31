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
            SceneDirector.LoadScene("MainScene");
        }
    }


    public void ConfirmNewGame()
    {
        PlayerPrefs.DeleteAll();
        Buffer.ResetBuffer();
        SceneDirector.LoadScene("MainScene");
    }

}
