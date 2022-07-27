using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponController : MonoBehaviour
{
    public GameObject weaponArm;
    public float handleOffset = 0.75f;
    private Camera cam;
    private Vector2 aimInput = Vector2.zero;
    private Weapon currWeapon = null;
    private bool isMouse;
    private bool isFiring = false;
    private Vector3 targetDirection = Vector3.zero;
    private Vector3 mouseWorldPosition = Vector3.zero;
    public Animator animator;
    public Transform punchPoint;
    public LayerMask meleeLayers;
    public float meleeDamage;
    public float meleeKnockback;

    public GameObject GetCurrentWeapon()
    {
        if (currWeapon)
        {
            return currWeapon.gameObject;
        }
        return null;
    }

    private void Awake()
    {
        isMouse = gameObject.GetComponent<PlayerInput>().currentControlScheme == "Keyboard";
        if (!cam) { cam = Camera.main; }
    }

    public void SetCam(Camera newCam)
    {
        cam = newCam;
    }

    public void PickupWeapon(Weapon weapon)
    {
        if (weapon == currWeapon)
        {
            return;
        }
        if (currWeapon)
        {
            currWeapon.transform.SetParent(null);
            currWeapon.transform.rotation = Quaternion.identity;
            currWeapon.transform.localScale = new Vector3(currWeapon.transform.localScale.x, currWeapon.transform.localScale.x, 1);
        }
        currWeapon = weapon;
        currWeapon.SetOwner(gameObject);

        currWeapon.transform.parent = weaponArm.transform;
        currWeapon.transform.localPosition = (-currWeapon.GetHandlePoint().localPosition)*currWeapon.transform.localScale.x  + new Vector3(handleOffset, 0, 0);
        currWeapon.transform.localRotation = Quaternion.identity;
        currWeapon.transform.localScale = new Vector3(Mathf.Abs(currWeapon.transform.localScale.x), Mathf.Abs(currWeapon.transform.localScale.y), 1);
        currWeapon.myWeaponController = this;
        // update ui
        StaticData.playerUI.SetUI();
    }

    public void DropWeapon()
    {
        if (currWeapon)
        {
            currWeapon.transform.SetParent(null);
            currWeapon.transform.rotation = Quaternion.identity;
            currWeapon.transform.localScale = new Vector3(currWeapon.transform.localScale.x, currWeapon.transform.localScale.x, 1);
        }
        currWeapon = null;
    }

    public void OnAimInput(InputAction.CallbackContext context)
    {
        
        aimInput = context.ReadValue<Vector2>();
    }

    // LateUpdate is called once per frame right after Update
    void LateUpdate()
    {
        if(weaponArm.transform.localScale.y < 0)
        {
            weaponArm.transform.localScale += new Vector3(0, 2, 0);
        }
        if (isMouse)
        {
            Vector2 mouseScreenPosition = aimInput;
            mouseWorldPosition = cam.ScreenToWorldPoint(new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, 10));
            targetDirection = mouseWorldPosition - weaponArm.transform.position;
        }
        else if(aimInput.magnitude > 0.05f)
        {
            targetDirection = aimInput;
            mouseWorldPosition = transform.position + (targetDirection.normalized * 5);
        }
        if (!currWeapon)
        {
            if (isFiring && !animator.GetBool("IsJumping") && !animator.GetBool("IsDoubleJumping"))
            {
                animator.SetBool("IsMeleeing", true);
            }
            return;
        }
        float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
        weaponArm.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle));

        if ((angle < -90f || angle > 90f) && weaponArm.transform.localScale.y == 1)
        {
            weaponArm.transform.localScale += new Vector3(0, -2, 0);
        }
        else if((angle > -90f || angle < 90f) && weaponArm.transform.localScale.y == -1)
        {
            weaponArm.transform.localScale += new Vector3(0, 2, 0);
        }

        LaserSight LS = currWeapon.gameObject.GetComponent<LaserSight>();
        if (LS)
        {
            LS.SetLaser();
        }

        if (isFiring)
        {
            currWeapon.Shoot();
        }
    }

    public void PrimaryFireInput(InputAction.CallbackContext context)
    {
        if (context.action.triggered)
        {
            isFiring = true;
        }
        else
        {
            isFiring = false;
        }
        
    }

    public Vector2 GetAimPoint()
    {
        return mouseWorldPosition;
    }

    public Weapon GetWeapon()
    {
        return currWeapon;
    }

    public void WeaponDestroyed()
    {
        currWeapon = null;
        StaticData.playerUI.SetUI();
    }

    public void Melee()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(punchPoint.position, 0.2f, meleeLayers);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject.tag == "Player" && colliders[i].gameObject != gameObject)
            {
                PlayerHealthHandler PHH = colliders[i].gameObject.GetComponent<PlayerHealthHandler>();
                PHH.ApplyDamage(new DamageParams(meleeDamage, null));
                float KB = meleeKnockback;
                KB *= 100;
                ForceMode2D mode = ForceMode2D.Force;
                if(PHH.GetHealth() > 0)
                {
                    colliders[i].gameObject.GetComponent<PlayerMovement>().enabled = false;
                    colliders[i].gameObject.GetComponent<CharacterController2D>().ResetPlayerMovement(0.1f);
                }
                colliders[i].attachedRigidbody.AddForce((colliders[i].gameObject.transform.position - gameObject.transform.position + Vector3.up/4).normalized * KB, mode);
                
                break;
            }
        }
        animator.SetBool("IsMeleeing", false);
    }

}
