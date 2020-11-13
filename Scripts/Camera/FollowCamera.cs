using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform target = null;
    public bool xFollow = false;
    public bool yFollow = false;
    public bool zFollow = false;

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
        if(xFollow)
        {
            transform.position = new Vector3(target.position.x, transform.position.y, transform.position.z);
        }
    }
}
