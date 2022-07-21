using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformAssigner : MonoBehaviour
{
    [SerializeField] private GameObject point1, point2;
    private int width, height, dist;
    private bool vertical;

    public MovingPlatform movingPlatform;
    public void Assign(int _width, int _height, int _dist, float _speed, bool _vertical)
    {
        width = _width;
        height = _height;
        vertical = _vertical;
        dist = _dist;
        movingPlatform.updateSettings(_width, _height, _speed);
    }

    public void SetPoints(Vector3 _start)
    {
        Vector3 adjustedP1 = new Vector3(_start.x + ((float)width - 1) / 2, _start.y, 0);
        Vector3 adjustedP2;
        if (vertical)
        {
            adjustedP2 = new Vector3(_start.x + ((float)width - 1) / 2, _start.y + dist, 0);
        }
        else
        {
            adjustedP2 = new Vector3(_start.x + ((float)width - 1) / 2 + dist, _start.y, 0);
        }
        point1.transform.position = adjustedP1;
        point2.transform.position = adjustedP2;
    }

    public void SetPointsRaw(Vector3 p1, Vector3 p2)
    {
        Vector3 adjustedP1 = new Vector3(p1.x + ((float)width - 1) / 2, p1.y, 0);
        Vector3 adjustedP2 = new Vector3(p2.x + ((float)width - 1) / 2, p2.y, 0);
        point1.transform.position = adjustedP1;
        point2.transform.position = adjustedP2;
    }
}
