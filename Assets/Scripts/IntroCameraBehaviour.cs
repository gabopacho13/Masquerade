using UnityEngine;

public class IntroCameraBehaviour : MonoBehaviour
{

    public bool IsActive { get; set; } = false;
    public Vector3 finalPosition;
    public GameObject target;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
 
    }

    // Update is called once per frame
    void Update()
    {
        if (IsActive == true)
        {
            MoveDramatically();
        }
    }

    public void MoveDramatically()
    {
        if (Vector3.Distance(transform.localPosition, finalPosition) > 0.01f)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, finalPosition, Time.deltaTime * 0.5f);
        }
        else
        {
            transform.localPosition = finalPosition;
        }
        if (target != null)
        {
            transform.LookAt(target.transform);
        }
    }
}
