using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class carController : MonoBehaviour
{
    [SerializeField] private float despawnTime = 15f;
    private DamageParams DP = new DamageParams(100f, "None");
    public float impactForce = 20f;
    public float timeBetweenDamage = 0.2f;
    private float currTimeBD;
    private bool canDoDamage = true;
    public Rigidbody2D rb;
    public float carSpeed = 30f;

    private void Awake()
    {
        Destroy(gameObject, despawnTime);
        currTimeBD = 0f;
    }

    public void setDamageParams(DamageParams _DP)
    {
        this.DP = _DP;
    }

    private IEnumerator coroutine;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Rigidbody2D>() && collision.gameObject.GetComponent<PlayerHealthHandler>() && canDoDamage)
        {
            float dmg = collision.relativeVelocity.x;
            if (dmg < 25) { return; }
            DP = new DamageParams(Mathf.Abs(collision.relativeVelocity.x), "None");
            Debug.Log(DP.GetDamage());
            //collision.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(-1, 5f) * DP.GetDamage(), ForceMode2D.Impulse);
            collision.gameObject.GetComponent<PlayerMovement>().enabled = false;
            if (collision.gameObject.GetComponent<PlayerHealthHandler>().GetHealth() - DP.GetDamage() > 0)
            {
                coroutine = resetMove(collision.gameObject);
                StartCoroutine(coroutine);
            }
            collision.collider.SendMessageUpwards("ApplyDamage", DP, SendMessageOptions.DontRequireReceiver);
            currTimeBD = timeBetweenDamage;
            canDoDamage = false;
        }
    }

    private IEnumerator resetMove(GameObject gObject)
    {
        yield return new WaitForSeconds(0.5f);
        gObject.GetComponent<PlayerMovement>().enabled = true;
    }

    private void Update()
    {
        if(currTimeBD <= 0)
        {
            canDoDamage = true;
        }
        else
        {
            currTimeBD -= Time.deltaTime;
        }

        
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(-carSpeed, rb.velocity.y);
    }
}
