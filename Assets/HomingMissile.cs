using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class HomingMissile : MonoBehaviour
{
    public Transform target;
    public float speed = 5f;
    public float rotateSpeed = 200f;
    public BoxCollider2D boxCol;
    private GameObject owner = null;

    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCol = GetComponent<BoxCollider2D>();
        Invoke("EnableCollider", 0.1f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (target)
        {
            Vector2 direction = (Vector2)target.position - rb.position;
            direction.Normalize();
            float rotateAmount = Vector3.Cross(direction, transform.right).z;

            rb.angularVelocity = -rotateAmount * rotateSpeed;
        }
        rb.velocity = transform.right * speed;
    }

    public void SetOwner(GameObject _owner)
    {
        owner = _owner;
        GameObject[] potentialTargets = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject player in potentialTargets)
        {
            if(player != owner)
            {
                target = player.transform;
            }
        }
    }

    public void EnableCollider()
    {
        boxCol.isTrigger = false;
    }

    /*private void OnTriggerEnter2D(Collider2D collision)
    {
        Instantiate(explosionFX, transform.position, transform.rotation);
        Destroy(gameObject);

    }*/
}
