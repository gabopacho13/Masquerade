using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasqueradeCamera : MonoBehaviour
{
    [SerializeField]
    private Transform lookAt;
    [SerializeField]
    private Transform target;
    public bool SlerpToTargetFinished { get; private set; } = false;
    [SerializeField]
    private float moveSpeed = 3f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (lookAt != null)
        {
            transform.LookAt(lookAt);
        }
    }

    private void Update()
    {
        if (lookAt != null)
        {
            transform.LookAt(lookAt);
        }
    }

    public IEnumerator SlerpToTarget()
    {
        yield return new WaitForSeconds(2f);

        if (target == null) yield break;

        Vector3 startPos = transform.position;
        Vector3 endPos = target.position;

        float distance = Vector3.Distance(startPos, endPos);
        if (distance < 0.001f)
        {
            transform.position = endPos;
            yield break;
        }

        float duration = distance / Mathf.Max(0.0001f, moveSpeed);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            transform.position = Vector3.Slerp(startPos, endPos, t);
            yield return null;
        }

        transform.position = endPos;
        SlerpToTargetFinished = true;
    }
}
