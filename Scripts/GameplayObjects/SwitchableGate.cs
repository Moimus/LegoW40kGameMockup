using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchableGate : Machine
{
    //Open = On, Closed = off
    public string openAnimName = "Open";
    public string closeAnimName = "Close";
    public Transform doorBlocker = null;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override int getState()
    {
        return currentState;
    }

    public override void onTurnOff()
    {
        toggleGate();
    }

    public override void onTurnOn()
    {
        toggleGate();

    }

    public void unblock()
    {
        blocked = false;
        if(currentState == (int)activationStates.on)
        {
            doorBlocker.gameObject.SetActive(false);
        }
        else
        {
            doorBlocker.gameObject.SetActive(true);
        }
    }

    void toggleGate()
    {
        if(currentState == (int)activationStates.off)
        {
            playAnimationWithInterruption(openAnimName);
            currentState = (int)activationStates.on;
        }
        else
        {
            playAnimationWithInterruption(closeAnimName);
            currentState = (int)activationStates.off;
        }
        blocked = true;
    }


}
