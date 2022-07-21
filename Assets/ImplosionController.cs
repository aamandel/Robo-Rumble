using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImplosionController : MonoBehaviour
{
    public float implodeRadius;
    public float damageRadius;
    public LayerMask effectedLayers;
    public float damageAmount;
    public float implosionForce;
    public float implosionDuration;
    private bool canImplode = false;
    private bool imploding = false;
    private Rigidbody2D rb;
    public Collider2D myCollider;
    public ParticleSystem particleFX;
    private List<PlayerHealthHandler> players;
    private float accumulatedDmg = 0f;

    private void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        players = new List<PlayerHealthHandler>();
        canImplode = false;
        imploding = false;
        Invoke("UnlockImplode", 0.1f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // don't implode on immediate contact with player (so you cant hit yourself)
        if (!canImplode && effectedLayers == (effectedLayers | (1 << collision.gameObject.layer)))
        {
            return;
        }
        particleFX.Play();
        imploding = true;
        Destroy(gameObject, implosionDuration);
        myCollider.enabled = false;
        Rigidbody2D hitRB = collision.gameObject.GetComponent<Rigidbody2D>();
        InvokeRepeating("DoDamage", 0f, .9f);
        // don't parent if the hit GameObject has a dynamic rigid body
        if (hitRB && hitRB.isKinematic == false)
        {
            return;
        }
        transform.SetParent(collision.gameObject.transform);
        rb.isKinematic = true;
    }

    void Implode()
    {
        if (!rb)
        {
            Debug.LogError("GameObject with Implode script has no rigidbody");
            return;
        }
        rb.velocity = Vector2.zero;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(gameObject.transform.position, implodeRadius, effectedLayers);
        for (int i = 0; i < colliders.Length; i++)
        {
            Rigidbody2D rbToAttract = colliders[i].GetComponent<Rigidbody2D>();
            if (rbToAttract)
            {
                Vector3 direction = rb.position - rbToAttract.position;
                Vector3 force = direction.normalized * implosionForce;
                rbToAttract.AddForce(force);
            }
            PlayerHealthHandler PHH = colliders[i].GetComponent<PlayerHealthHandler>();
            if (PHH && !players.Contains(PHH))
            {
                players.Add(PHH);
            }
        }
    }

    private void FixedUpdate()
    {
        if (imploding)
        {
            Implode();
        }
    }

    private void UnlockImplode()
    {
        canImplode = true;
    }

    void DoDamage()
    {
        foreach (PlayerHealthHandler PHH in players)
        {
            if(accumulatedDmg >= damageAmount)
            {
                return;
            }
            DamageParams dp = new DamageParams(damageAmount / implosionDuration, null);
            if(Vector3.Distance(PHH.gameObject.transform.position, transform.position) < damageRadius)
            {
                PHH.ApplyDamage(dp);
            }
            accumulatedDmg += damageAmount / implosionDuration;
        }
    }
}
