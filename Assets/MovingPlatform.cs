using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private float speed;     // Speed for platform to move
    [SerializeField] private int width, height;
    public int startingPoint;                 // starting index for the points array
    public Transform[] points;                // the array of points for the platform to move between
    private int index = 0;
    public MovingPlatformAssigner myParent;

    private void Start()
    {
        transform.position = points[startingPoint].position;
    }

    private void Update()
    {
        // switch points once point is reached
        if (Vector2.Distance(transform.position, points[index].position) < 0.02f)
        {
            index++;
            if(index == points.Length)
            {
                index = 0;
            }
        }

        // move the platform
        transform.position = Vector2.MoveTowards(transform.position, points[index].position, speed * Time.deltaTime);
    }

    //private Vector3 startPos,endPos;
    //private Vector3 nextPos;
    //public GameObject myParent;
    /*private Rigidbody2D parentRB;


    // Start is called before the first frame update
    void Start()
    {
        //updateSettings(width, height, distance, speed, vertical);
        parentRB = gameObject.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if(transform.position == endPos)
        {
            nextPos = startPos;
        }
        if(transform.position == startPos)
        {
            nextPos = endPos;
        }
        Vector3 copy = transform.position;
        //parentRB.MovePosition(Vector2.MoveTowards(transform.position, nextPos, speed * Time.deltaTime));
        transform.position = Vector2.MoveTowards(copy, nextPos, speed * Time.deltaTime);
    }*/

    public void updateSettings(int _width, int _height, float _speed)
    {
        width = _width;
        height = _height;
        speed = _speed;
        // scale platform
        transform.localScale = new Vector3(width, height, 1);
        //set points positions to account for scale
        foreach(Transform p in points)
        {
            p.position = new Vector3(p.position.x + ((float)width - 1) / 2, p.position.y + ((float)height - 1) / 2, 0);
        }
        //myParent.transform.position = new Vector3(transform.position.x + ((float)width - 1) / 2, transform.position.y + ((float)height - 1) / 2, 0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.transform.SetParent(this.transform);
            collision.gameObject.GetComponent<Rigidbody2D>().interpolation = RigidbodyInterpolation2D.None;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
            collision.gameObject.GetComponent<Rigidbody2D>().interpolation = RigidbodyInterpolation2D.Interpolate;
        }
    }
}
