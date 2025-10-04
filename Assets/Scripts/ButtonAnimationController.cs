using UnityEngine;

public class ButtonAnimationController : MonoBehaviour
{

    public Animator buttonAnimator;
    public Animator bubblesAnimator;

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
        buttonAnimator.SetTrigger("Play");
        if (bubblesAnimator)
        {
            bubblesAnimator.SetTrigger("Play");
        }
    }


}
