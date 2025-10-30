using UnityEngine;

public class Button : MonoBehaviour
{

    [SerializeField]
    private AudioSource pingSound;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StarManager.VerifyButton(this.gameObject);
            if (pingSound != null)
            {
                pingSound.PlayOneShot(pingSound.clip);
            }
            gameObject.SetActive(false);
        }
    }
}
