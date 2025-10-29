using UnityEngine;

public class DemonAnimationEvents : MonoBehaviour
{

    [SerializeField]
    private GameObject blackScreen;
    [SerializeField]
    private AudioSource music;
    [SerializeField]
    private AudioSource demonRoar;
    [SerializeField]
    private AudioSource dramaticHit;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (blackScreen != null && blackScreen.activeSelf)
        {
            blackScreen.SetActive(false);
        }
    }

    public void ActivateBlackScreen()
    {
        if (blackScreen != null)
        {
            blackScreen.SetActive(true);
            dramaticHit.Play();
            music.Stop();
        }
    }

    public void PlayDemonRoar()
    {
        demonRoar.Play();
    }
}
