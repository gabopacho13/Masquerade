using UnityEngine;

public class ObstacleManager : MonoBehaviour
{

    public GameObject obstacleBeast;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && obstacleBeast != null && !obstacleBeast.activeSelf)
        {
            CameraManager.ChangeToCamera("ObstacleCourseCamera");
            obstacleBeast.SetActive(true);
            obstacleBeast.GetComponent<ObstacleBeast>().StartTalking = true;
            MusicManager.ChangeMusic("ObstacleCourse");
        }
    }
}
