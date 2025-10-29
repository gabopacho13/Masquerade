using UnityEngine;

public class MasqueradeVillagers : MonoBehaviour
{
    [SerializeField]
    private float spinSpeed = -30f;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime);
    }
}
