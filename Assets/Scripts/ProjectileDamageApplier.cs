using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileDamageApplier : MonoBehaviour
{
    //public GameObject hitFX;
    private DamageParams DP = new DamageParams(100f, "None");

    /*
     * DamageParams is a simple class to store damage and origin of projectile
     * See Assets/Extra Classes
    */
    public void setDamageParams(DamageParams _DP)
    {
        this.DP = _DP;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //GameObject effect = Instantiate(hitFX, collision.contacts[0].point, transform.rotation);
        //Destroy(effect, 5f);
        //Destroy(gameObject.GetComponent<ParticleSystem>());
        Destroy(gameObject.GetComponent<CircleCollider2D>(), 2f);
        Destroy(gameObject, 5f);

        collision.collider.SendMessageUpwards("ApplyDamage", DP, SendMessageOptions.DontRequireReceiver);
    }
}
