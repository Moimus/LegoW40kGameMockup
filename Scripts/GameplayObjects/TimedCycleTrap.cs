using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedCycleTrap : Machine
{
    public GameObject FX = null;
    public float interval = 2f;
    public float offSet = 0f;
    float intervalCountDown = 2f;
    public int damage = 4;

    // Start is called before the first frame update
    void Start()
    {
        intervalCountDown = interval + offSet;
    }

    // Update is called once per frame
    void Update()
    {
        runLoop();
    }

    public override void runLoop()
    {
        if(currentState == (int)activationStates.on)
        {
            trapCycle();
        }
        else
        {

        }
    }

    public override void onTurnOff()
    {
        toggleTrap();

    }

    public override void onTurnOn()
    {
        toggleTrap();
    }

    void toggleTrap()
    {
        if(currentState == (int)activationStates.on)
        {
            currentState = (int)activationStates.off;
            FX.SetActive(false);
            intervalCountDown = interval;
            blocked = false;
        }
        else
        {
            currentState = (int)activationStates.on;
            FX.SetActive(true);
            intervalCountDown = interval;
            blocked = false;
        }
    }

    void trapCycle()
    {
        if(intervalCountDown > 0)
        {
            intervalCountDown -= Time.deltaTime;
        }
        else
        {
            if(FX.activeInHierarchy)
            {
                FX.SetActive(false);
            }
            else
            {
                FX.SetActive(true);
            }
            intervalCountDown = interval;
        }
    }

    void trapHit(IHittable other)
    {
        other.onHit(damage);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(currentState == (int)activationStates.on && other.GetComponent<IHittable>() != null && FX.activeInHierarchy)
        {
            trapHit(other.GetComponent<IHittable>());
        }
    }


}
