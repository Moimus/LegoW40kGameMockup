using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour, IActivator
{
    public Animator animator;
    public string activateAnimName = "Flip";
    public string deactivateAnimName = "FlipBack";
    public bool activated = false;
    public Transform statusLight = null;
    public Material[] onOffMaterials = new Material[2];
    public List<Machine> linkedMachines = new List<Machine>();
    public List<int> canBeActivatedBy = new List<int>(); // see Character.characterTypes for definitions
    public Transform playerMarker = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void activate(GameObject source)
    {
        if(!linkedMachineIsBlocked())
        {
            blockLinkedMachines();
            if (!activated)
            {
                playAnimationWithoutInterruption(activateAnimName);
                if(animator == null)
                {
                    onActivationCompleted();
                }
                activated = true;
            }
            else
            {
                playAnimationWithoutInterruption(deactivateAnimName);
                if (animator == null)
                {
                    onDeactivationComplete();
                }
                activated = false;
            }
        }
    }

    void blockLinkedMachines()
    {
        foreach (Machine m in linkedMachines)
        {
            m.blocked = true;
        }
    }

    public void activateLinkedMachines()
    {
        foreach(Machine m in linkedMachines)
        {
            m.blocked = true;
            m.onTurnOn();
        }
    }

    public void deactivateLinkedMachines()
    {
        foreach (Machine m in linkedMachines)
        {
            m.onTurnOff();
        }
    }

    public bool linkedMachineIsBlocked()
    {
        bool isBlocked = false;
        foreach(Machine m in linkedMachines)
        {
            if(m.blocked)
            {
                isBlocked = true;
                break;
            }
        }
        return isBlocked;
    }

    public void onActivationCompleted()
    {
        statusLight.GetComponent<Renderer>().material = onOffMaterials[0];
        activateLinkedMachines();
    }

    public void onDeactivationComplete()
    {
        statusLight.GetComponent<Renderer>().material = onOffMaterials[1];
        deactivateLinkedMachines();
    }

    public void playAnimationWithoutInterruption(string name)
    {
        if (animator != null && !animatorIsInTransition(name))
        {
            animator.Play(name, 0);
        }
    }

    public bool animatorIsInTransition(string animName)
    {
        bool val = false;
        if (animator.GetCurrentAnimatorClipInfo(0).Length > 0)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName(animName))
            {
                val = true;
            }
        }
        return val;
    }

    public bool canBeActivatedByCharacter(List<int> characterTypes)
    {
        bool activatable = false;
        foreach(int type in characterTypes)
        {
            if(canBeActivatedBy.Contains(type))
            {
                activatable = true;
            }
        }
        return activatable;
    }

    public Transform getPlayerMarker()
    {
        return playerMarker;
    }
}
