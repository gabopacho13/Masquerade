using UnityEngine;
using UnityEngine.SceneManagement;

public class ManholeManager : MonoBehaviour
{
    private ManholeLid manholeLid;
    private bool canEnter = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        manholeLid = transform.parent.transform.GetComponentInChildren<ManholeLid>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C) && canEnter)
        {
            SceneDirector.LoadScene("Sewers");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && manholeLid.IsOpen)
        {
            UIManager.InteractInstruction.SetActive(true);
            canEnter = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && manholeLid.IsOpen)
        {
            UIManager.InteractInstruction.SetActive(false);
            canEnter = false;
        }
    }
}
