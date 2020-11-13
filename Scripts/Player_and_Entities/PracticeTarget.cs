using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PracticeTarget : MonoBehaviour, IHittable
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onHit(int damage)
    {
        Destroy(gameObject);
    }

    public void checkAlive()
    {
        throw new System.NotImplementedException();
    }
}
