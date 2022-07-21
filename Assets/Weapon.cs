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
    public GameObject owner;

    
    //the handle point is the location for the weapon to be held from
    public Transform GetHandlePoint()
    {
        return handlePoint;
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
        GameObject Projectile = Instantiate(projectilePrefab, muzzlePoint.position, Quaternion.identity, null);
        Projectile.GetComponent<Rigidbody2D>().velocity = (muzzlePoint.right + (Vector3.up * (Random.Range(-spreadAmount / 100, spreadAmount/100)))) * launchSpeed;
        Bullet BulletScript = Projectile.GetComponent<Bullet>();
        if (BulletScript)
        {
            BulletScript.SetOwner(owner);
        }

        numShots--;
        if(numShots == 0)
        {
            Destroy(gameObject);
        }
    }

    private void ResetShoot()
    {
        canShoot = true;
    }
}
