using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    public Animator animator;
    private string currentState;
    private bool animationLock;
    public float baseAnimationTransitionDuration = 0.01f;
    public PlayerMovement pMovement;
    public WeaponController weaponController;
    private enum playerAnimationStates
    {
        Player_Jump,
        Player_Idle,
        Player_DoubleJump,
        Player_TPose,
        Player_Crouch,
        Player_CrouchBlock,
        Player_CrouchBlockEvent,
        Player_Melee,
        Player_Run,
        Player_Dash,
        Player_RunMelee,
        Player_SwordSwing1,
        Player_RunSwordSwing1
    }

    private void Start()
    {
        animationLock = false;
    }

    public void Melee(float horizontalMove)
    {
        if (animationLock) { return; }
        float delay = 0f;
        if(Mathf.Abs(horizontalMove) > 0.01f)
        {
            ChangeAnimationState(playerAnimationStates.Player_RunMelee.ToString(), baseAnimationTransitionDuration);
            foreach(AnimationClip clip in animator.runtimeAnimatorController.animationClips)
            {
                if(clip.name == playerAnimationStates.Player_RunMelee.ToString())
                {
                    delay = clip.length;
                    break;
                }
            }
        }
        else
        {
            ChangeAnimationState(playerAnimationStates.Player_Melee.ToString(), baseAnimationTransitionDuration);
            foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
            {
                if (clip.name == playerAnimationStates.Player_Melee.ToString())
                {
                    delay = clip.length;
                    break;
                }
            }
        }
        animationLock = true;
        Invoke("ResetAnimationLock", delay);
    }

    public void SwingSword()
    {
        if (animationLock) { return; }
        //pMovement.FreezePlayer();
        string animationToPlay;
        if (Mathf.Abs(pMovement.horizontalMove) > 0.01f)
        {
            animationToPlay = playerAnimationStates.Player_RunSwordSwing1.ToString();
        }
        else
        {
            animationToPlay = playerAnimationStates.Player_SwordSwing1.ToString();
        }
        ChangeAnimationState(animationToPlay, baseAnimationTransitionDuration);
        animationLock = true;
        float delay = 0f;
        AnimationClip clip = Array.Find(animator.runtimeAnimatorController.animationClips, element => element.name == animationToPlay);
        if (clip)
        {
            delay = clip.length;
        }
        Invoke("ResetAnimationLock", delay);
        //pMovement.UnfreezePlayer(delay);
        /*foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == playerAnimationStates.Player_SwordSwing1.ToString())
            {
                delay = clip.length;
                break;
            }
        }*/
    }

    public void Run()
    {
        ChangeAnimationState(playerAnimationStates.Player_Run.ToString(), baseAnimationTransitionDuration);
    }

    public void Idle()
    {
        ChangeAnimationState(playerAnimationStates.Player_Idle.ToString(), baseAnimationTransitionDuration);
    }

    public void onLanding()
    {
        // ...
    }

    public void OnCrouching(bool state)
    {
        string animationToPlay;
        Weapon currWeapon = weaponController.GetWeaponScript();
        if (currWeapon && currWeapon.isMeleeWeapon)
        {
            animationToPlay = playerAnimationStates.Player_CrouchBlock.ToString();
        }
        else
        {
            animationToPlay = playerAnimationStates.Player_Crouch.ToString();
        }

        if (state)
        {
            ChangeAnimationState(animationToPlay, baseAnimationTransitionDuration);
            animationLock = true;
        }
        else
        {
            ResetAnimationLock();
        }
    }

    public void BlockEvent()
    {
        if(currentState == playerAnimationStates.Player_CrouchBlock.ToString() || currentState == playerAnimationStates.Player_CrouchBlockEvent.ToString())
        {
            string animationToPlay = playerAnimationStates.Player_CrouchBlockEvent.ToString();
            animator.CrossFade(playerAnimationStates.Player_CrouchBlock.ToString(), 2f);
            animator.Play(animationToPlay);
            currentState = animationToPlay;
        }
    }

    public void OnJump(float horizontalMove)
    {

        //animator.SetBool("IsJumping", true);
        if (Mathf.Abs(horizontalMove) > 0.01f)
        {
            ChangeAnimationState(playerAnimationStates.Player_Jump.ToString(), baseAnimationTransitionDuration * 10);
            return;
        }
        ChangeAnimationState(playerAnimationStates.Player_Jump.ToString(), baseAnimationTransitionDuration * 2);

    }

    public void OnDoubleJump()
    {
        ChangeAnimationState(playerAnimationStates.Player_DoubleJump.ToString(), baseAnimationTransitionDuration);
        // animation has exit time:
        animationLock = true;
        float delay = animator.GetCurrentAnimatorStateInfo(0).length / 10;
        Invoke("ResetAnimationLock", delay);
    }

    public void OnDashEnter()
    {
        ChangeAnimationState(playerAnimationStates.Player_Dash.ToString(), baseAnimationTransitionDuration);
        // animation has exit time:
        animationLock = true;
        //float delay = animator.GetCurrentAnimatorStateInfo(0).length / 10;
        //Invoke("ResetAnimationLock", delay);
    }

    public void OnDashExit(float horizontalMove)
    {
        ResetAnimationLock();
        if (Mathf.Abs(horizontalMove) > 0.01f)
        {
            ChangeAnimationState(playerAnimationStates.Player_Run.ToString(), baseAnimationTransitionDuration);
        }
        else
        {
            ChangeAnimationState(playerAnimationStates.Player_Idle.ToString(), baseAnimationTransitionDuration);
        }
    }

    void ChangeAnimationState(string newState, float transitionDuration)
    {
        if (currentState == newState || animationLock) return;
        animator.CrossFade(newState, transitionDuration);
        currentState = newState;
    }

    void ResetAnimationLock()
    {
        animationLock = false;
    }

}
