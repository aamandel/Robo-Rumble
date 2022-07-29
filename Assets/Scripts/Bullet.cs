using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage;
    public GameObject particles;
    public bool particlesDieOnImpact = false;
    public bool particlesWorldPositionStays = true;
    public GameObject optionalImpactFX;
    public bool optionalFXHasPhysics = false;
    public float maxExistTime = 5f;
    public float optionalImpactExistTime = 5f;
    private bool hitSomething;
    public StunPlayer optionalStunner;


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
        if(collision.gameObject.tag == "Weapon")
        {
            Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), collision.collider);
        }
        if (hitSomething)
        {
            return;
        }
        PlayerHealthHandler PHH = collision.gameObject.GetComponent<PlayerHealthHandler>();
        if (PHH)
        {
            PHH.ApplyDamage(new DamageParams(damage, "None"));
            if (optionalStunner && PHH.GetHealth() > 0)
            {
                optionalStunner.Stun(collision.gameObject);
            }
        }

        if (optionalImpactFX)
        {
            GameObject fx = Instantiate(optionalImpactFX, transform.position, transform.rotation);
            if (optionalFXHasPhysics)
            {
                fx.GetComponent<Rigidbody2D>().velocity = gameObject.GetComponent<Rigidbody2D>().velocity;
            }
            Destroy(fx, optionalImpactExistTime);
        }

        if(collision.gameObject.tag != "Weapon")
        {
            particles.transform.SetParent(null, particlesWorldPositionStays);
            if (particlesDieOnImpact && particles.GetComponent<ParticleSystem>() != null)
            {
                particles.GetComponent<ParticleSystem>().Stop();
            }
            Destroy(particles.gameObject, 5f);
            Destroy(gameObject);
            hitSomething = true;
        }
        
        
    }
}
