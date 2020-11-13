using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchableLight : Machine
{
    public Material[] onOffMaterials = new Material[2]; //on = 0, off = 1
    public List<Transform> lightBodies = new List<Transform>();
    public Light lightSource = null;
    public string animationName = "Loop";
    public string idleAnimName = "Idle";
    public float autoStopTime = -1;

    public override int getState()
    {
        return currentState;
    }

    public override void onTurnOff()
    {
        toggleLights();
        animate();
    }

    public override void onTurnOn()
    {
        toggleLights();
        animate();
        
    }

    void toggleLights()
    {
        if(currentState == (int)activationStates.off)
        {
            foreach (Transform t in lightBodies)
            {
                Material[] materials = t.GetComponent<Renderer>().materials;
                for (int n = 0; n < materials.Length; n++)
                {
                    materials[n] = onOffMaterials[0];
                }
                t.GetComponent<Renderer>().materials = materials;
            }

            lightSource.gameObject.SetActive(true);
            currentState = (int)activationStates.on;
            blocked = false;
        }
        else
        {
            foreach (Transform t in lightBodies)
            {
                Material[] materials = t.gameObject.GetComponent<Renderer>().materials;
                for (int n = 0; n < materials.Length; n++)
                {
                    materials[n] = onOffMaterials[1];
                }
                t.GetComponent<Renderer>().materials = materials;
            }
            lightSource.gameObject.SetActive(false);
            currentState = (int)activationStates.off;
            blocked = false;
        }
    }

    void animate()
    {
        if(animator != null)
        {
            playAnimationWithInterruption(animationName);
            if (autoStopTime != -1)
            {
                StartCoroutine(autoTurnOff());
            }
        }
    }

    IEnumerator autoTurnOff()
    {
        yield return new WaitForSeconds(autoStopTime);
        playAnimationWithInterruption(idleAnimName);
        toggleLights();
        yield return null;
    }


    public override void runLoop()
    {
        //not needed for this
    }

}
