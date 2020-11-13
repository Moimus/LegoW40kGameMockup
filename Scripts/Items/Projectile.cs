using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Character owner = null;
    public float lifeTime = 2f;
    public float speed = 30f;
    public int damage = 1;

    // Start is called before the first frame update
    void Start()
    {
        init();
    }

    // Update is called once per frame
    void Update()
    {
        lifeCycle();
    }

    protected virtual void lifeCycle()
    {
        moveForward();
    }

    protected virtual void init()
    {
        Destroy(gameObject, lifeTime);
    }

    protected virtual void moveForward()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * speed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.GetComponent<IHittable>() != null)
        {
            IHittable hittable = other.transform.GetComponent<IHittable>();
            hittable.onHit(damage);
        }
        if(other.transform.GetComponent<Projectile>() == null)
        {
            Destroy(gameObject);
        }
    }
}
