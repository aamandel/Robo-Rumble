using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private Transform handlePoint;
    [SerializeField] private Transform muzzlePoint;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float launchSpeed;
    [SerializeField] private float recoveryTime; // time between shots
    [SerializeField] private float spreadAmount; // bullet spread
    private bool canShoot = true;
    [SerializeField] private int numShots;
    [SerializeField] private bool infiniteAmmo = false;
    private int currNumShots;
    private PlayerUIManager playerUI;
    private GameObject owner;
    [HideInInspector] public WeaponController myWeaponController;


    // get the number of shots currently left
    public int GetCurrShots()
    {
        return currNumShots;
    }

    // get the maximum number of shots
    public int GetShotCapacity()
    {
        return numShots;
    }

    // the handle point is the location for the weapon to be held from
    public Transform GetHandlePoint()
    {
        return handlePoint;
    }

    private void Awake()
    {
        currNumShots = numShots;
        playerUI = StaticData.playerUI;
    }

    private void Start()
    {
        currNumShots = numShots;
        playerUI = StaticData.playerUI;
    }

    public void SetOwner(GameObject _owner)
    {
        owner = _owner;
        
    }

    public void Shoot()
    {
        if (!canShoot) { return; }
        canShoot = false;
        Invoke("ResetShoot", recoveryTime);
        GameObject Projectile = Instantiate(projectilePrefab, muzzlePoint.position, muzzlePoint.rotation, null);
        Projectile.GetComponent<Rigidbody2D>().velocity = (muzzlePoint.right + (Vector3.up * (Random.Range(-spreadAmount / 100, spreadAmount/100)))) * launchSpeed;
        Bullet BulletScript = Projectile.GetComponent<Bullet>();
        if (BulletScript && owner)
        {
            BulletScript.SetOwner(owner);
        }

        if (infiniteAmmo)
        {
            return;
        }
        currNumShots--;
        if(currNumShots == 0)
        {
            if (myWeaponController)
            {
                myWeaponController.WeaponDestroyed();
            }
            Destroy(gameObject);
        }
        if (playerUI)
        {
            playerUI.SetUI();
        }
    }

    private void ResetShoot()
    {
        canShoot = true;
    }
}
