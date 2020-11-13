using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Machine : MonoBehaviour, IMachine //just extend this class to use serializable machines (linkable in editor)
{
    public Animator animator = null;
    public bool blocked = false;
    public int currentState = 0;
    public enum activationStates
    {
        off = 0,
        on = 1
    }

    public virtual int getState()
    {
        throw new System.NotImplementedException();
    }

    public virtual void onTurnOff()
    {
        throw new System.NotImplementedException();
    }

    public virtual void onTurnOn()
    {
        throw new System.NotImplementedException();
    }

    public virtual void runLoop()
    {
        throw new System.NotImplementedException();
    }

    public void playAnimationWithInterruption(string name)
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
}
