using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{

    //TODO refactor this whole class
    public NavMeshAgent navAgent;
    public Character character = null;
    public float cooldownHandicap = 2.0f; 

    public Rigidbody rBody = null;

    public Animator animator = null;
    public string idleAnimName = "Idle";
    public string runAnimName = "Run";
    public string shootAnimName = "Shoot";
    public string jumpInAnimName = "Jump_In";
    public int animationState = 0;
    public enum animationStates
    {
        dead = -1,
        idle = 0,
        walkingForward = 1,
        walkingBackward = 2,
        running = 3,
        attacking = 4,
        jumping_in = 5,
        jumping_loop = 6
    }

    public int AIState = 0;
    public enum AIStates
    {
        idle = 0,
        moving = 1,
        attacking = 2
    }
    public float scanRadius = 5f;
    public float attackRadius = 2f;
    public List<GameObject> targetsInRange = new List<GameObject>();
    public GameObject currentTarget = null;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        animationLoop();
        scanForEnemies();
        AILoop();
    }

    void AILoop()
    {
        if(AIState == (int)AIStates.idle)
        {
            if(targetsInRange.Count > 0)
            {
                currentTarget = targetsInRange[0];
                attackTarget();
            }
        }
        else if(AIState == (int)AIStates.moving)
        {

        }
        else if(AIState == (int)AIStates.attacking)
        {
            if (targetsInRange.Count > 0)
            {
                attackTarget();
            }
            else
            {
                idle();
            }
        }
    }

    void attackTarget()
    {
        AIState = (int)AIStates.attacking;
        try
        {
            if(currentTarget.transform != null)
            {
                if (Vector3.Distance(transform.position, currentTarget.transform.position) > attackRadius || !hasClearSight())
                {
                    navAgent.SetDestination(currentTarget.transform.position);
                    setAnimationState((int)animationStates.running);
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
            }
            else
            {
                idle();
            }
        }
        catch (MissingReferenceException mre)
        {
            currentTarget = null;
        }
    }

    void scanForEnemies()
    {
        Collider[] inRange = Physics.OverlapSphere(transform.position, scanRadius);
        List<GameObject> possibleTargets = new List<GameObject>();
        foreach(Collider c in inRange)
        {
            if(c.gameObject.GetComponent<Character>() != null)
            {
                if(c.gameObject.GetComponent<Character>().faction != character.faction)
                {
                    possibleTargets.Add(c.gameObject);
                }
            }
        }
        targetsInRange = possibleTargets;
    }

    void animationLoop()
    {
        if (animationState == (int)animationStates.idle)
        {
            playAnimationWithoutInterruption(idleAnimName);
        }
        else if (animationState == (int)animationStates.running)
        {
            playAnimationWithoutInterruption(runAnimName);
        }
        else if (animationState == (int)animationStates.jumping_in)
        {
            playAnimationWithoutInterruption(jumpInAnimName);
        }
        else if (animationState == (int)animationStates.attacking && character.gun.cooldownRemaining <= 0)
        {
            playAnimationWithInterruption(shootAnimName);
        }
        
    }

    void setAnimationState(int state)
    {
        if (animationState != state)
        {
            animationState = state;
        }
    }

    void idle()
    {
        if (!animatorIsInTransition(shootAnimName))
        {
            setAnimationState((int)animationStates.idle);
        }
    }

    void fire()
    {
        if (character.gun.cooldownRemaining <= 0)
        { 
            setAnimationState((int)animationStates.attacking);
            character.gun.fire();
            character.gun.cooldownRemaining += cooldownHandicap;
        }
    }

    void fireOut()
    {

    }

    bool hasClearSight()
    {
        bool hasClearSight = false;
        RaycastHit raycastHit;
        bool hit = Physics.Raycast(transform.position,transform.forward, out raycastHit, 10, -1);
        Debug.DrawRay(transform.position, raycastHit.point * 10, Color.cyan);
        if (hit && raycastHit.collider.gameObject.GetInstanceID() == currentTarget.GetInstanceID())
        {
            hasClearSight = true;
        }
        else if(currentTarget != null && Vector3.Distance(currentTarget.transform.position, transform.position) < attackRadius / 2)
        {
            hasClearSight = true;
        }
        return hasClearSight;
    }

    void lookAtTarget()
    {
        if(currentTarget != null)
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
