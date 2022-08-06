using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    private GameObject owner;
    public float damageAmount = 30f;
    public float knockBack = 50f;
    public Transform bladePoint;
    public LayerMask damageLayers;
    public LayerMask bulletLayers;
    public bool deflectsBullets = true;
    private bool isBlocking = false;
    // whether or not this sword has a cooldown for blocking bullets
    public bool BlockingHasCooldown = true;
    public float BlockCD = 1f;
    private bool canBlock = true;

    public void StartBlocking()
    {
        isBlocking = true;
    }

    public void StopBlocking()
    {
        isBlocking = false;
    }

    private void Start()
    {
        isBlocking = false;
    }

    private void Awake()
    {
        isBlocking = false;
    }

    // block every physics update
    void FixedUpdate()
    {
        if (isBlocking)
        {
            Block();
        }
    }

    public void SetOwner(GameObject _owner)
    {
        owner = _owner;
        if(owner == null)
        {
            isBlocking = false;
        }
    }

    public void DoDamage()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(bladePoint.position, 0.5f, damageLayers);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject.tag == "Player" && colliders[i].gameObject != owner)
            {
                PlayerHealthHandler PHH = colliders[i].gameObject.GetComponent<PlayerHealthHandler>();
                PHH.ApplyDamage(new DamageParams(damageAmount, null));
                float KB = knockBack;
                KB *= 100;
                ForceMode2D mode = ForceMode2D.Force;
                if (PHH.GetHealth() > 0)
                {
                    colliders[i].gameObject.GetComponent<PlayerMovement>().enabled = false;
                    colliders[i].gameObject.GetComponent<CharacterController2D>().ResetPlayerMovement(0.1f);
                }
                colliders[i].attachedRigidbody.AddForce((colliders[i].gameObject.transform.position - gameObject.transform.position + Vector3.up / 4).normalized * KB, mode);

                break;
            }
        }
    }

    public void Block()
    {
        if (!deflectsBullets) { return; }
        Collider2D[]  colliders = Physics2D.OverlapCircleAll(bladePoint.position, 1f, bulletLayers);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (!canBlock) { return; }
            Rigidbody2D rb = colliders[i].GetComponent<Rigidbody2D>();
            if (rb)
            {
                rb.velocity = rb.velocity * -1f;
                if (BlockingHasCooldown)
                {
                    canBlock = false;
                    Invoke("ResetBlock", BlockCD);
                    InvokeBlockAnim();
                }
            }
            Bullet bullet = colliders[i].GetComponent<Bullet>();
            if (bullet)
            {
                bullet.SetOwner(owner);
            }
        }
    }

    void ResetBlock()
    {
        canBlock = true;
    }

    void InvokeBlockAnim()
    {
        if (owner)
        {
            PlayerAnimationController pAnim = owner.GetComponent<PlayerAnimationController>();
            pAnim.BlockEvent();
        }
    }
}
