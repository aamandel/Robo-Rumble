using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController2D controller;
    public float runSpeed = 40f;
    float horizontalMove = 0f;
    bool jump = false;
    bool crouch = false;
    bool ability1 = false;
    public bool doneBoosting;
    public Animator animator;
    public ParticleSystem dashFX;
    [SerializeField] private LayerMask weaponLayers;
    [SerializeField] private float weaponPickupRadius = 2f;
    public WeaponController weaponController;
    public float interactTimer = 0.5f;
    private bool interactAllowed = true;

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
        animator.SetFloat("Speed", Mathf.Abs(horizontalMove));
    }

    void FixedUpdate()
    {
        //move character  - horizontalMove * Time.fixedDeltaTime
        controller.Move(horizontalMove * Time.fixedDeltaTime, crouch, jump);
        //reset jump
        jump = false;

        controller.useAbilities(ability1);
        ability1 = false;
    }

    public void onLanding()
    {
        animator.SetBool("IsJumping", false);
        animator.SetBool("IsDoubleJumping", false);
    }

    public void OnCrouching(bool state)
    {
        animator.SetBool("IsCrouching", state);
    }

    public void OnJump()
    {
        animator.SetBool("IsJumping", true);
    }

    public void OnDoubleJump()
    {
        animator.SetBool("IsDoubleJumping", true);
    }

    public void OnDashEnter()
    {
        animator.SetBool("IsDashing", true);
        dashFX.Play();
    }

    public void OnDashExit()
    {
        animator.SetBool("IsDashing", false);
        animator.SetBool("IsDoubleJumping", false);
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
                    weaponController.PickupWeapon(colliders[i].gameObject.GetComponent<Weapon>());
                    return;
                }
            }
        }

    }

    private void interactCooldown()
    {
        interactAllowed = true;
    }
}

