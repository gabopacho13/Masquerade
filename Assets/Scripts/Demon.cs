using System.Collections;
using UnityEngine;

public class Demon : MonoBehaviour
{
    [SerializeField]
    private float rescaleDuration = 5.0f;
    [SerializeField]
    private float targetScale = 20f;
    [SerializeField]
    private Light headLight;
    public bool RescaleFinished { get; private set; } = false;
    public bool Attack { get; set; } = false;
    private Animator animator;
    private bool hasAttacked = false;


    private void Start()
    {
        if (headLight != null)
        {
            headLight.enabled = false;
        }
        animator = transform.GetComponentInChildren<Animator>();
    }

    private void LateUpdate()
    {
        if (Attack && !hasAttacked)
        {
            animator.SetTrigger("Attack");
            hasAttacked = true;
        }
    }

    public IEnumerator ReScale()
    {
        float elapsedTime = 0.0f;
        float t = 0;
        Vector3 startingScale = transform.localScale;
        Vector3 finalScale = new Vector3(targetScale, targetScale, targetScale);
        headLight.enabled = true;
        while (elapsedTime < rescaleDuration)
        {
            elapsedTime += Time.deltaTime;
            t = elapsedTime / rescaleDuration;
            transform.localScale = Vector3.Lerp(startingScale, finalScale, t);
            yield return null;
        }
        transform.localScale = finalScale;
        RescaleFinished = true;
    }
}
