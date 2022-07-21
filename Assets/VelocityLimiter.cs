using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityLimiter : MonoBehaviour
{
    public Rigidbody2D rb;
    private float maxVelocity;
    private void Awake()
    {
        maxVelocity = 50f;
    }

    void FixedUpdate()
    {
        if (!rb.isKinematic)
        {
            if(rb.velocity.magnitude > maxVelocity)
            {
                Vector3 newVelocity = rb.velocity.normalized;
                newVelocity *= maxVelocity;
                rb.velocity = newVelocity;
            }
        }
        
    }
}
