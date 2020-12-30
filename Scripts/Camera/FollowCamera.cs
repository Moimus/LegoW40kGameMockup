using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform target = null;
    public bool xFollow = false;
    public bool yFollow = false;
    public bool zFollow = false;

    public float xOffset = 0;
    public float yOffset = 0;
    public float zOffset = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        followPlayer();
        lookAtPlayer();
    }

    void lookAtPlayer()
    {
        if(target != null)
        {
            transform.LookAt(target.position);
        }
    }

    void followPlayer()
    {
        if(target != null)
        {
            if (xFollow)
            {
                transform.position = new Vector3(target.position.x + xOffset, transform.position.y + yOffset, transform.position.z + zOffset);
            }

            if (xFollow)
            {
                transform.position = new Vector3(transform.position.x + xOffset, target.position.y + yOffset, transform.position.z + zOffset );
            }
        }
    }
}
