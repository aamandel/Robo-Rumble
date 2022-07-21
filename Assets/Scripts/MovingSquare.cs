using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingSquare : MonoBehaviour
{
    public Vector2 startPoint;
    public Vector2 endPoint = new Vector2(0f, 0f);
    float timeElapsed = 0f;
    float lerpDuration = 3;

    private void Start()
    {
        startPoint = GetComponent<Transform>().position;
    }

    // Update is called once per frame
    void Update()
    {
        if (timeElapsed < lerpDuration)
        {
            //transform.position = new Vector3(); Mathf.Lerp(startValue, endValue, timeElapsed / lerpDuration);
            //timeElapsed += Time.deltaTime;
        }
    }
}
