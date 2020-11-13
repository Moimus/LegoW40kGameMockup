using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public bool perfectAccuracy = false;
    public Transform bulletSpawner;
    public GameObject projectilePrefab;
    public float cooldown = 0.25f;
    public float cooldownRemaining = 0;
    public Vector3 maxDeviation = new Vector3(10, 10, 10); //maximum AimDeviation, used for AI
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        decreaseCooldown();
    }

    public void fire()
    {
        if(cooldownRemaining <= 0)
        {
            cooldownRemaining = cooldown;
            GameObject proj = Instantiate(projectilePrefab, bulletSpawner.position, bulletSpawner.rotation);
            if(!perfectAccuracy)
            {
                Quaternion deviation = Quaternion.Euler(Random.Range(-maxDeviation.x, maxDeviation.x) + bulletSpawner.transform.rotation.eulerAngles.x, Random.Range(-maxDeviation.y, maxDeviation.y) + bulletSpawner.transform.rotation.eulerAngles.y, Random.Range(-maxDeviation.z, maxDeviation.z) + bulletSpawner.transform.rotation.eulerAngles.z);
                proj.transform.rotation = deviation;
            }
        }
    }

    void decreaseCooldown()
    {
        if(cooldownRemaining > 0)
        {
            cooldownRemaining -= Time.deltaTime;
        }
    }
}
