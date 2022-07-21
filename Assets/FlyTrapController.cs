using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyTrapController : MonoBehaviour
{
    public Transform target, grabPoint;
    public Transform[] idlePoints;
    private int index;
    private GameObject player;
    public float attackSpeed = 10f;
    public float normalSpeed = 1f;
    public Animator jawsAnimator;
    private bool grabbedPlayer = false;
    public float maxStunTime = 3f;
    private float amountMoved;
    public float damageAmount = 50f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            jawsAnimator.SetBool("Chomping", true);
            player = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealthHandler PHH = collision.gameObject.GetComponent<PlayerHealthHandler>();
            if(PHH && PHH.GetHealth() <= 0)
            {
                return;
            }
            jawsAnimator.SetBool("Chomping", false);
            grabbedPlayer = false;
            player = null;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        target.position = idlePoints[0].position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (player)
        {
            Attack();
            return;
        }
        Shake(normalSpeed);
    }

    private void Attack()
    {
        float distToPlayer = Vector2.Distance(grabPoint.position, player.transform.position);
        if(distToPlayer > 0.4f)
        {
            target.position = Vector3.MoveTowards(target.position, player.transform.position, attackSpeed * Time.fixedDeltaTime);
            return;
        }
        if (!grabbedPlayer)
        {
            // Damage
            DamageParams dp = new DamageParams(damageAmount, null);
            PlayerHealthHandler PHH = player.gameObject.GetComponent<PlayerHealthHandler>();
            // Stun Player
            player.gameObject.GetComponent<PlayerMovement>().enabled = false;
            if (PHH)
            {
                // apply damage
                PHH.ApplyDamage(dp);
                if (PHH.GetHealth() > 0)
                {
                    player.gameObject.GetComponent<CharacterController2D>().ResetPlayerMovement(3f);
                }
            }
            StartCoroutine(Grab());
            grabbedPlayer = true;
        }
        
    }

    IEnumerator Grab()
    {
        float timePassed = 0;
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        while (timePassed < 2)
        {
            rb.MovePosition(grabPoint.position);
            //rb.AddForce((grabPoint.position - player.transform.position) * 100f);
            //player.transform.position = grabPoint.position;
            Shake(attackSpeed);
            timePassed += Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }
        //player.GetComponent<Rigidbody2D>().velocity = new Vector2(3, 2) * 10;
        float yToss = 2;
        float xToss;
        xToss = 3;
        
        if (amountMoved < 0)
        {
            xToss = -3;
        }
        //player.GetComponent<Rigidbody2D>().velocity = new Vector2(xToss, yToss) * 7;
        rb.AddForce(new Vector2(xToss, yToss).normalized * 200f, ForceMode2D.Impulse);
        player = null;
        grabbedPlayer = false;
        jawsAnimator.SetBool("Chomping", false);
    }
    private void Shake(float shakeSpeed)
    {
        float distToPoint = Vector2.Distance(target.position, idlePoints[index].position);
        if (distToPoint < 0.1f)
        {
            index++;
            if (index == idlePoints.Length)
            {
                index = 0;
            }
        }
        //move the target for idle anim
        Vector3 oldPos = target.position;
        target.position = Vector2.MoveTowards(target.position, idlePoints[index].position, shakeSpeed * Time.fixedDeltaTime * Mathf.Min(distToPoint, 1f));
        amountMoved = (target.position.x - oldPos.x);
    }
}
