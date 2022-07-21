using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserSight : MonoBehaviour
{
    public Transform LaserPoint;
    public LineRenderer lineRenderer;

    // Update is called once per frame
    void Update()
    {   
        lineRenderer.SetPosition(0, LaserPoint.position);
        lineRenderer.SetPosition(1, LaserPoint.position + LaserPoint.right * 2);
    }

    public void SetLaser()
    {
        RaycastHit2D hitInfo = Physics2D.Raycast(LaserPoint.position + LaserPoint.right * 0.3f, LaserPoint.right);

        lineRenderer.SetPosition(0, LaserPoint.position);
        lineRenderer.SetPosition(1, LaserPoint.position + LaserPoint.right * 100);
        if (hitInfo)
        {
            lineRenderer.SetPosition(1, hitInfo.point);
        }
    }
}
