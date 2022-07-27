using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spinner : MonoBehaviour
{
    float degrees = 0;
    [SerializeField] float spinSpeed;
    [SerializeField] bool switchesDirections = false;
    [SerializeField] float maxTimeBetweenSwitches = 30f;
    private float switchCountdown;

    private void Start()
    {
        switchCountdown = Random.Range(0.5f, maxTimeBetweenSwitches);
    }

    // Update is called once per frame
    void Update()
    {
        transform.eulerAngles = Vector3.forward * degrees;
        degrees += spinSpeed * Time.deltaTime;
        if(degrees > 360 || degrees < -360)
        {
            degrees = 0;
        }

        if (!switchesDirections)
        {
            return;
        }

        if (switchCountdown < 0)
        {
            switchCountdown = Random.Range(0.5f, maxTimeBetweenSwitches);
            spinSpeed = -spinSpeed;
        }
        switchCountdown -= Time.deltaTime;
    }
}
