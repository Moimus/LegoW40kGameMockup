using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIControllerAdvanced : MonoBehaviour
{
    public NavMeshAgent navAgent;
    public Character character = null;
    public float cooldownHandicap = 2.0f;

    public Animator animator = null;
    public string idleAnimName = "Idle";
    public string runAnimName = "Run";
    public string shootAnimName = "Shoot";
    public string jumpInAnimName = "Jump_In";
    public bool actionsBlocked = false;

    public float scanRadius = 5f;
    public float attackRadius = 2f;
    public List<GameObject> targetsInRange = new List<GameObject>();
    public GameObject currentTarget = null;

    public int AIState = 0;
    public enum AIStates
    {
        idle = 0,
        route = 1,
        combat = 2
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        AILoop();
    }

    public void AILoop()
    {
        if(AIState == (int)AIStates.idle)
        {
            idle();
        }
        else if(AIState == (int)AIStates.combat)
        {
            combat();
        }
    }

    void idle()
    {
        if (currentTarget == null)
        {
            scanForEnemies();
            if(navAgent.hasPath)
            {
                navAgent.ResetPath();
            }
            if(targetsInRange.Count > 0)
            {
                currentTarget = targetsInRange[0];
            }
        }
        else
        {
            AIState = (int)AIStates.combat;
        }
    }

    void combat()
    {
        if(currentTarget != null)
        {
            if (Vector3.Distance(transform.position, currentTarget.transform.position) > attackRadius || !hasClearSight())
            {
                navAgent.SetDestination(currentTarget.transform.position);
                playAnimationWithInterruption(runAnimName);
            }
            else
            {
                if (!animatorIsInTransition(shootAnimName))
                {
                    playAnimationWithInterruption(idleAnimName);
                }
                navAgent.velocity = Vector3.zero;
                lookAtTarget();
                fire();
                navAgent.ResetPath();
            }

            if(Vector3.Distance(transform.position, currentTarget.transform.position) > attackRadius)
            {
                currentTarget = null;
            }
        }
        else
        {
            AIState = (int)AIStates.idle;
        }
    }

    void fire()
    {
        if (character.gun.cooldownRemaining <= 0)
        {
            character.gun.fire();
            character.gun.cooldownRemaining += cooldownHandicap;
            playAnimationWithoutInterruption(shootAnimName);
        }
    }

    void fireOut()
    {

    }

    void scanForEnemies()
    {
        Collider[] inRange = Physics.OverlapSphere(transform.position, scanRadius);
        List<GameObject> possibleTargets = new List<GameObject>();
        foreach (Collider c in inRange)
        {
            if (c.gameObject.GetComponent<Character>() != null)
            {
                if (c.gameObject.GetComponent<Character>().faction != character.faction)
                {
                    possibleTargets.Add(c.gameObject);
                }
            }
        }
        targetsInRange = possibleTargets;
    }

    bool hasClearSight()
    {
        bool hasClearSight = false;
        RaycastHit raycastHit;
        bool hit = Physics.Raycast(transform.position, transform.forward, out raycastHit, 10, -1);
        Debug.DrawRay(transform.position, raycastHit.point * 10, Color.cyan);
        if (hit && raycastHit.collider.gameObject.GetInstanceID() == currentTarget.GetInstanceID())
        {
            hasClearSight = true;
        }
        else if (currentTarget != null && Vector3.Distance(currentTarget.transform.position, transform.position) < attackRadius / 2)
        {
            hasClearSight = true;
        }
        return hasClearSight;
    }

    void lookAtTarget()
    {
        if (currentTarget != null)
        {
            Vector3 direction = (currentTarget.transform.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * character.rotationSpeed);
        }
    }

    //Doesn't interrupt current playing animation
    public void playAnimationWithoutInterruption(string name)
    {
        if (animator != null && !animatorIsInTransition(name))
        {
            animator.Play(name, 0);
        }
    }

    public void playAnimationWithInterruption(string name)
    {
        animator.Play(name, 0);
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
