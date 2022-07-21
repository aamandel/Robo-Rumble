using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spinner : MonoBehaviour
{
    float degrees = 0;
    [SerializeField] float spinSpeed;

    // Update is called once per frame
    void Update()
    {
        transform.eulerAngles = Vector3.forward * degrees;
        degrees += spinSpeed * Time.deltaTime;
        if(degrees > 360)
        {
            degrees = 0;
        }
    }
}
