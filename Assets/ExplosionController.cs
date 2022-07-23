using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionController : MonoBehaviour
{
    public GameObject explosionPrefab;
    public float explosionDelay;
    public float damageRadius;
    public LayerMask damageLayers;
    public float damageAmount;
    public float explosionForce;
    private bool canExplode = false;

    private void Awake()
    {
        canExplode = false;
        Invoke("Explode", explosionDelay);
        Invoke("UnlockExplode", 0.1f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerHealthHandler PHH = collision.gameObject.GetComponent<PlayerHealthHandler>();
        if (PHH && canExplode)
        {
            Explode();
        }
    }

    public void Explode()
    {
        GameObject explosion = Instantiate(explosionPrefab, gameObject.transform.position, Quaternion.identity);
        Destroy(gameObject);
        Destroy(explosion, 5f);
        List<GameObject> explodedPlayers = new List<GameObject>();
        Collider2D[] colliders = Physics2D.OverlapCircleAll(gameObject.transform.position, damageRadius, damageLayers);
        for (int i = 0; i < colliders.Length; i++)
        {
            //apply explosion damage
            bool isPlayer = colliders[i].gameObject.tag == "Player";
            if (isPlayer && !explodedPlayers.Contains(colliders[i].gameObject))
            {
                explodedPlayers.Add(colliders[i].gameObject);
                PlayerHealthHandler PHH = colliders[i].gameObject.GetComponent<PlayerHealthHandler>();
                if (PHH)
                {
                    PHH.ApplyDamage(new DamageParams(damageAmount, "None"));
                }

                // Stun Player
                colliders[i].gameObject.GetComponent<PlayerMovement>().enabled = false;
                if (PHH.GetHealth() > 0)
                {
                    colliders[i].gameObject.GetComponent<CharacterController2D>().ResetPlayerMovement(0.5f);
                }
            }
            //apply explosion force
            Rigidbody2D rb = colliders[i].gameObject.GetComponent<Rigidbody2D>();
            if (rb)
            {
                Vector2 direction = new Vector2(
                    colliders[i].gameObject.transform.position.x - gameObject.transform.position.x,
                    colliders[i].gameObject.transform.position.y - gameObject.transform.position.y
                );
                rb.AddForce(direction.normalized * explosionForce, ForceMode2D.Impulse);
            }
        }
    }

    private void UnlockExplode()
    {
        canExplode = true;
    }
}
