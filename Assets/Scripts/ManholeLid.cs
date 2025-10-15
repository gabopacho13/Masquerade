using UnityEngine;

public class ManholeLid : MonoBehaviour
{

    public bool OpenUp { get; set; } = false;
    private Animator animator;
    private bool isOpen = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LateUpdate()
    {
        if (OpenUp && !isOpen)
        {
            animator.SetTrigger("openUp");
            isOpen = true;
        }
    }
}
