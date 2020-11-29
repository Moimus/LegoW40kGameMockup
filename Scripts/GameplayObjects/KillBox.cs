using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillBox : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<IHittable>() != null)
        {
            other.GetComponent<IHittable>().onHit(1000);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<IHittable>() != null)
        {
            other.GetComponent<IHittable>().onHit(1000);
        }
    }
}
