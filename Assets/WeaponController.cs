using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponController : MonoBehaviour
{
    public GameObject gunArm, swordArm;
    public float handleOffset = 0.75f;
    private Camera cam;
    private Vector2 aimInput = Vector2.zero;
    private GameObject currWeapon = null;
    private Weapon weaponScript = null;
    private bool isMouse;
    private bool isFiring = false;
    private Vector3 targetDirection = Vector3.zero;
    private Vector3 mouseWorldPosition = Vector3.zero;
    public Animator animator;
    public PlayerAnimationController playerAnimControl;
    public PlayerMovement playerMovement;
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

    public void PickupWeapon(GameObject weapon)
    {

        if (weapon == currWeapon)
        {
            return;
        }
        if (currWeapon)
        {
            DropWeapon();
        }
        currWeapon = weapon;
        
        weaponScript = currWeapon.GetComponent<Weapon>();
        weaponScript.SetOwner(gameObject);

        if (weaponScript.isMeleeWeapon)
        {
            currWeapon.transform.SetParent(swordArm.transform);
            Sword _sword = currWeapon.gameObject.GetComponent<Sword>();
            if (_sword)
            {
                _sword.SetOwner(gameObject);
            }
        }
        else
        {
            currWeapon.transform.SetParent(gunArm.transform);
        }
        
        currWeapon.transform.localPosition = (-weaponScript.GetHandlePoint().localPosition)*currWeapon.transform.localScale.x  + new Vector3(handleOffset, 0, 0);
        currWeapon.transform.localRotation = Quaternion.identity;
        currWeapon.transform.localScale = new Vector3(Mathf.Abs(currWeapon.transform.localScale.x), Mathf.Abs(currWeapon.transform.localScale.y), 1);
        weaponScript.myWeaponController = this;
        // update ui
        StaticData.playerUI.SetUI();
    }

    public void DropWeapon()
    {
        if (!currWeapon) { return; }
        if (weaponScript.isMeleeWeapon)
        {
            weaponScript.GetComponent<Sword>().SetOwner(null);
        }
        weaponScript.SetOwner(null);
        currWeapon.transform.SetParent(null);
        currWeapon.transform.rotation = Quaternion.identity;
        currWeapon.transform.localScale = new Vector3(currWeapon.transform.localScale.x, currWeapon.transform.localScale.x, 1);
        weaponScript = null;
        currWeapon = null;
    }

    public void OnAimInput(InputAction.CallbackContext context)
    {
        
        aimInput = context.ReadValue<Vector2>();
    }

    // LateUpdate is called once per frame right after Update
    void LateUpdate()
    {
        
        if(gunArm.transform.localScale.y < 0)
        {
            gunArm.transform.localScale += new Vector3(0, 2, 0);
        }
        if (isMouse)
        {
            Vector2 mouseScreenPosition = aimInput;
            mouseWorldPosition = cam.ScreenToWorldPoint(new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, 10));
            targetDirection = mouseWorldPosition - gunArm.transform.position;
        }
        else if(aimInput.magnitude > 0.05f)
        {
            targetDirection = aimInput;
            mouseWorldPosition = transform.position + (targetDirection.normalized * 5);
        }
        if (!currWeapon)
        {
            if (isFiring)
            {
                playerAnimControl.Melee(playerMovement.horizontalMove);
            }
            
            /*if (isFiring && !animator.GetBool("IsJumping") && !animator.GetBool("IsDoubleJumping"))
            {
                animator.SetBool("IsMeleeing", true);
            }*/
            return;
        }
        if (weaponScript.isMeleeWeapon)
        {
            if (isFiring)
            {
                playerAnimControl.SwingSword();
            }
            return;
        }
        float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
        gunArm.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle));

        if ((angle < -90f || angle > 90f) && gunArm.transform.localScale.y == 1)
        {
            gunArm.transform.localScale += new Vector3(0, -2, 0);
        }
        else if((angle > -90f || angle < 90f) && gunArm.transform.localScale.y == -1)
        {
            gunArm.transform.localScale += new Vector3(0, 2, 0);
        }

        LaserSight LS = currWeapon.gameObject.GetComponent<LaserSight>();
        if (LS)
        {
            LS.SetLaser();
        }

        if (isFiring)
        {
            weaponScript.Shoot();
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

    public Weapon GetWeaponScript()
    {
        return weaponScript;
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
    }

    public void SwordDamage()
    {
        Sword _sword = currWeapon.GetComponent<Sword>();
        _sword.DoDamage();
    }

    public void OnCrouching(bool state)
    {
        if (!weaponScript || !weaponScript.isMeleeWeapon) { return; }
        Sword _sword = currWeapon.gameObject.GetComponent<Sword>();
        if (state)
        {
            _sword.StartBlocking();
        }
        else
        {
            _sword.StopBlocking();
        }
    }
}
