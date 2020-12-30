using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpJumpFX : MonoBehaviour
{
    public float scaleSpeed = 2.0f;
    public ParticleSystem[] particleSystems;
    public bool shrinking = false;
    public List<GameObject> enteringObjects = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(shrinking)
        {
            shrink();
        }
    }

    void shrink()
    {
        transform.localScale = new Vector3(transform.localScale.x - 0.1f * Time.deltaTime * scaleSpeed, transform.localScale.y - 0.1f * Time.deltaTime * scaleSpeed, transform.localScale.z - 0.1f * Time.deltaTime * scaleSpeed);
        if(transform.localScale.x <= 0f || transform.localScale.y <= 0f || transform.localScale.z <= 0f)
        {
            Destroy(gameObject);
        }
        else if(transform.localScale.x <= 0.4f || transform.localScale.y <= 0.4f || transform.localScale.z <= 0.4f)
        {
            foreach (GameObject obj in enteringObjects)
            {
                Destroy(obj);
            }
        }
        foreach (ParticleSystem p in particleSystems)
        {
            p.gameObject.transform.localPosition = Vector3.MoveTowards(p.gameObject.transform.localPosition, new Vector3(0,0,0), Time.deltaTime * scaleSpeed);
            p.gameObject.transform.localScale = new Vector3(p.gameObject.transform.localScale.x - 0.1f * Time.deltaTime * scaleSpeed, p.gameObject.transform.localScale.y - 0.1f * Time.deltaTime * scaleSpeed, p.gameObject.transform.localScale.z - 0.1f * Time.deltaTime * scaleSpeed);
            ParticleSystem.ShapeModule ps = p.shape;
            ps.scale = new Vector3(ps.scale.x - 0.1f * Time.deltaTime, ps.scale.y - 0.1f * Time.deltaTime, ps.scale.z - 0.1f * Time.deltaTime * scaleSpeed);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        shrinking = true;
        enteringObjects.Add(other.gameObject);
    }
}
