using UnityEngine;

public class ButtonAnimationController : MonoBehaviour
{

    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    
    public void PlayAnim()
    {
        animator.SetTrigger("Play");
    }


}
