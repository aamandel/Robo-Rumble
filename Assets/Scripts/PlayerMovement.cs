using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController2D controller;
    public float runSpeed = 40f;
    [HideInInspector]
    public float horizontalMove = 0f;
    bool jump = false;
    bool crouch = false;
    bool ability1 = false;
    [HideInInspector]
    public bool doneBoosting;
    public ParticleSystem dashFX;
    [SerializeField] private LayerMask weaponLayers;
    [SerializeField] private float weaponPickupRadius = 2f;
    public WeaponController weaponController;
    public float interactTimer = 0.5f;
    private bool interactAllowed = true;
    private bool freeze = false;

    public PlayerAnimationController playerAnimControl;

    //raw input
    private Vector2 movementInput = Vector2.zero;

    // Start is called before the first frame update
    void Start()
    {
        doneBoosting = false;
    }

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    public void OnJumpInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            jump = true;
        }
    }

    public void OnCrouchInput(InputAction.CallbackContext context)
    {
        crouch = context.action.triggered;
    }

    public void Ability1Input(InputAction.CallbackContext context)
    {
        if (!ability1)
            ability1 = context.action.triggered;
    }

    // Update is called once per frame
    void Update()
    {
        horizontalMove = movementInput.x * runSpeed;
        if (!controller.m_Grounded)
        {
            return;
        }
        if(Mathf.Abs(horizontalMove) > 0.01f)
        {
            playerAnimControl.Run();
        }
        else
        {
            playerAnimControl.Idle();
        }
    }

    void FixedUpdate()
    {
        //move character  - horizontalMove * Time.fixedDeltaTime
        if (freeze)
        {
            controller.Move(0f, false, false);
            return;
        }
        controller.Move(horizontalMove * Time.fixedDeltaTime, crouch, jump);
        //reset jump
        jump = false;

        controller.useAbilities(ability1);
        ability1 = false;
    }



    public void OnJump()
    {
        playerAnimControl.OnJump(horizontalMove);
    }

    public void OnDashEnter()
    {
        playerAnimControl.OnDashEnter();
        dashFX.Play();
    }

    public void OnDashExit()
    {
        playerAnimControl.OnDashExit(horizontalMove);
        dashFX.Stop();
    }

    public void OnInteract()
    {
        if (!interactAllowed)
        {
            return;
        }
        interactAllowed = false;
        Invoke("interactCooldown", interactTimer);
        Collider2D[] colliders = Physics2D.OverlapCircleAll(gameObject.transform.position, weaponPickupRadius, weaponLayers);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject.tag == "Weapon")
            {
                if (colliders[i].gameObject != weaponController.GetCurrentWeapon())
                {
                    weaponController.enabled = true;
                    weaponController.PickupWeapon(colliders[i].gameObject);
                    return;
                }
            }
        }

    }

    private void interactCooldown()
    {
        interactAllowed = true;
    }

    public void FreezePlayer()
    {
        freeze = true;
    }

    public void UnfreezePlayer(float delay)
    {
        Invoke("UnfreezePlayerNow", delay);
    }

    public void UnfreezePlayerNow()
    {
        freeze = false;
    }

}

