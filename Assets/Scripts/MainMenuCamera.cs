using UnityEngine;

public class MainMenuCamera : MonoBehaviour
{
    [SerializeField]
    private float rotationSpeed = 1.0f;
    [SerializeField]
    private Transform lookAtTarget;

    void Start()
    {
        if (lookAtTarget == null)
        {
            Debug.LogWarning($"{nameof(MainMenuCamera)}: 'lookAtTarget' no asignado.");
            return;
        }

        transform.LookAt(lookAtTarget);
    }

    void Update()
    {
        if (lookAtTarget == null) return;

        transform.RotateAround(lookAtTarget.position, Vector3.up, rotationSpeed * Time.deltaTime);
        transform.LookAt(lookAtTarget);
    }
}
