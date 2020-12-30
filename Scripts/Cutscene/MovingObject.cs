using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObject : MonoBehaviour
{
    public float speed = 1.0f;
    public bool forward = false; 
    public bool backward = false; 
    public bool left = false;
    public bool right = false;
    public bool up = false;
    public bool down = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        move();
    }

    void move()
    {
        if(forward)
        {
            transform.Translate(Vector3.forward * Time.deltaTime * speed);
        }

        if (backward)
        {
            transform.Translate(-Vector3.forward * Time.deltaTime * speed);
        }

        if (left)
        {
            transform.Translate(Vector3.left * Time.deltaTime * speed);
        }

        if (right)
        {
            transform.Translate(Vector3.right * Time.deltaTime * speed);
        }

        if (up)
        {
            transform.Translate(Vector3.up * Time.deltaTime * speed);
        }

        if (down)
        {
            transform.Translate(Vector3.down * Time.deltaTime * speed);
        }
    }
}
