using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Obsolete]
public class PlayerController : MonoBehaviour
{
    public bool keyboardControlled = true;
    public Character character = null;

    public Rigidbody rBody = null;
    public bool grounded = true;

    public Animator animator = null;
    public string idleAnimName = "Idle";
    public string runAnimName = "Run";
    public string shootAnimName = "Shoot"; //also used for close combat
    public string jumpInAnimName = "Jump_In";
    public string rollingAnimName = "Roll";
    public string activateAnimName = "Activate"; //TODO
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
        jumping_loop = 6,
        rolling = 7,
        flippingSwitch = 8
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        checkInput();
        jumpLoop();
        rollLoop();
        animationLoop();
    }

    void checkInput()
    {
        if(animatorIsInTransition(rollingAnimName))
        {
            return;
        }
        //Keyboard Mode
        if(Input.GetMouseButtonDown(0))
        {
            fire();
        }
        else if(Input.GetKeyDown(KeyCode.E))
        {
            if (!animatorIsInTransition(rollingAnimName))
            {
                roll();
            }
        }
        else if (Input.GetKeyDown(KeyCode.F))
        {
            if (!animatorIsInTransition(activateAnimName))
            {
                activate();
            }
        }
        else if(Input.GetKeyDown(KeyCode.Q))
        {
            GameManagement.instance.selectNextCharacter();
        }
        else if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A))
        {
            if(!animatorIsInTransition(rollingAnimName))
            {
                lookLeft();
                moveForward();
            }
        }
        else if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D))
        {
            if (!animatorIsInTransition(rollingAnimName))
            {
                lookRight();
                moveForward();
            }

        }
        else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A))
        {
            if (!animatorIsInTransition(rollingAnimName))
            {
                lookLeft();
                moveBackward();
            }

        }
        else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.D))
        {
            if (!animatorIsInTransition(rollingAnimName))
            {
                lookRight();
                moveBackward();
            }

        }
        else if(Input.GetKey(KeyCode.W))
        {
            if (!animatorIsInTransition(rollingAnimName))
            {
                lookForward();
                moveForward();
            }

        }
        else if (Input.GetKey(KeyCode.A))
        {
            lookLeft();
        }
        else if(Input.GetKey(KeyCode.D))
        {
            lookRight();
        }
        else if(Input.GetKey(KeyCode.S))
        {
            if (!animatorIsInTransition(rollingAnimName))
            {
                moveBackward();
            }

        }
        else
        {
            idle();
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            jump();
        }
    }

    void rollLoop()
    {
        if (animationState == (int) animationStates.rolling)
        {
            transform.Translate(Vector3.forward * Time.deltaTime * character.rollSpeed);
        }
    }

    void animationLoop()
    {
        if(grounded)
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
            else if (animationState == (int)animationStates.attacking)
            {
                playAnimationWithoutInterruption(shootAnimName);
            }
            else if(animationState == (int)animationStates.rolling)
            {
                playAnimationWithoutInterruption(rollingAnimName);
            }
        }
        else
        {
            playAnimationWithoutInterruption(jumpInAnimName);
            if (animationState == (int)animationStates.jumping_in)
            {
                playAnimationWithoutInterruption(jumpInAnimName);
            }
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
        if(!animatorIsInTransition(shootAnimName) && !animatorIsInTransition(rollingAnimName))
        {
            setAnimationState((int)animationStates.idle);
        }
    }

    void fire()
    {
        if(character.gun != null && !character.closeCombat)
        {
            if (!animatorIsInTransition(shootAnimName) && !animatorIsInTransition(rollingAnimName) && character.gun.cooldownRemaining <= 0)
            {
                if (animationState != (int)animationStates.running)
                {
                    setAnimationState((int)animationStates.attacking);
                    autoAim();
                }
                character.gun.fire();
            }
        }
        else if(character.closeCombat && !animatorIsInTransition(rollingAnimName))
        {
            if(!animatorIsInTransition(shootAnimName))
            {
                setAnimationState((int)animationStates.attacking);
                autoAim();
            }
        }
    }

    void activate()
    {
        IActivator activator = getActivatorInRange();
        if(activator != null)
        {
            if(activator.canBeActivatedByCharacter(character.assignedCharacterTypes))
            {
                activator.activate(gameObject);
            }
        }
    }

    IActivator getActivatorInRange()
    {
        IActivator result = null;
        RaycastHit raycastHit;
        bool hit = Physics.Raycast(transform.position, transform.forward, out raycastHit, character.activationRange, -1);
        if(hit)
        {
            if(raycastHit.collider.transform.GetComponent<IActivator>() != null)
            {
                result = raycastHit.collider.transform.GetComponent<IActivator>();
            }
        }

        return result;
    }

    void autoAim()
    {
        List<Collider> inRange = new List<Collider>(Physics.OverlapSphere(transform.position + transform.forward * 2, 2)).OrderBy(p=>Vector3.Distance(p.transform.position, transform.position)).ToList<Collider>();
        if(inRange.Count > 0)
        {
            foreach(Collider col in inRange)
            {
                if(col.gameObject.GetInstanceID() != gameObject.GetInstanceID() && col.transform.GetComponent<IHittable>() != null)
                {
                    GameObject target = col.gameObject;
                    if (target != null)
                    {
                        Debug.Log(target.name);
                        Vector3 direction = (target.transform.position - transform.position).normalized;
                        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
                        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * character.rotationSpeed * 360);
                    }
                    break;
                }
            }

        }
    }

    void moveForward()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * character.speed);
        setAnimationState((int)animationStates.running);
    }

    void moveBackward()
    {
        transform.Translate(-Vector3.forward * Time.deltaTime * character.speed);
        setAnimationState((int)animationStates.running);
    }

    void lookForward()
    {
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(transform.forward.x, 0, transform.forward.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * character.rotationSpeed);
    }

    void lookLeft()
    {
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(-transform.right.x, 0, -transform.right.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * character.rotationSpeed);
    }

    void lookRight()
    {
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(transform.right.x, 0, transform.right.z));
        if(!animatorIsInTransition(rollingAnimName))
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * character.rotationSpeed);
        }
    }

    void lookBack()
    {
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(-transform.forward.x, 0, -transform.forward.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * character.rotationSpeed);
    }

    void jump()
    {
        if(!character.doubleJump)
        {
            if (grounded)
            {
                rBody.AddForce(transform.up * character.jumpForce);
                setAnimationState((int)animationStates.jumping_in);
            }
        }
        else if(character.doubleJump)
        {
            if (grounded)
            {
                rBody.AddForce(transform.up * character.jumpForce);
                setAnimationState((int)animationStates.jumping_in);
            }
            else if(!grounded && !character.usedDoubleJump)
            {
                rBody.AddForce(transform.up * character.jumpForce);
                setAnimationState((int)animationStates.jumping_in);
                character.usedDoubleJump = true;
            }
        }
    }

    void roll()
    {
        if(grounded)
        {
            setAnimationState((int)animationStates.rolling);
        }
    }

    void rollOut()
    {
        idle();
    }

    void jumpLoop()
    {
        RaycastHit raycastHit;
        bool hit = Physics.Raycast(transform.position, -transform.up, out raycastHit, 10, -1);
        Debug.DrawRay(transform.position, raycastHit.point * 10, Color.cyan);
        if (hit)
        {
            if(Vector3.Distance(transform.position, raycastHit.point) < 0.1f)
            {
                grounded = true;
                if(character.doubleJump)
                {
                    character.usedDoubleJump = false;
                }
            }
            else
            {
                grounded = false;
            }
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
