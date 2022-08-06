using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreenAnimator : MonoBehaviour
{
    public Animator robot1AnimationController, robot2AnimationController;

    // Start is called before the first frame update
    void Start()
    {
        robot1AnimationController.Play("Player_Dance");
        robot2AnimationController.Play("Player_Hanging");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
