using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class positionLocker : MonoBehaviour
{
    private Vector3 myPos;
    public Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        myPos = transform.localPosition;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.localPosition = myPos;
    }
}
