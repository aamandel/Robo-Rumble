using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage;
    public GameObject particles;
    public GameObject OptionalImpactFX;
    public float maxExistTime = 5f;
    private bool hitSomething;

    private void Awake()
    {
        Destroy(gameObject, maxExistTime);
        hitSomething = false;
    }

    public void SetOwner(GameObject _owner)
    {
        Component[] collidersToIgnore = _owner.GetComponents<Collider2D>();
        foreach (Collider2D c in collidersToIgnore)
        {
            Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), c);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (hitSomething)
        {
            return;
        }
        PlayerHealthHandler PHH = collision.gameObject.GetComponent<PlayerHealthHandler>();
        if (PHH)
        {
            PHH.ApplyDamage(new DamageParams(damage, "None"));
        }

        if (OptionalImpactFX)
        {
            GameObject fx = Instantiate(OptionalImpactFX, transform.position, transform.rotation);
            Destroy(fx, 5f);
        }

        if(collision.gameObject.tag != "Weapon")
        {
            particles.transform.SetParent(null);
            Destroy(particles.gameObject, 5f);
            Destroy(gameObject);
            hitSomething = true;
        }
        
        
    }
}
