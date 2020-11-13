using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerControllerAdvanced : MonoBehaviour
{
    public bool keyboardControlled = true;
    public Character character = null;
    public Checkpoint lastCheckpoint = null;

    public Rigidbody rBody = null;
    public bool grounded = true;

    public Animator animator = null;
    public string idleAnimName = "Idle";
    public string runAnimName = "Run";
    public string shootAnimName = "Shoot";
    public string closeCombatAnimName = "Attack";
    public string jumpInAnimName = "Jump";
    public string rollingAnimName = "Roll";
    public string activateAnimName = "Activate";
    public bool actionsBlocked = false;
    public Transform autoAimTarget = null;
    enum states
    {
        idle = 0,
        walking = 1,
        jumping = 2,
        attacking = 3,
        activating = 4
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        rollLoop();
        jumpLoop();
        checkInput();
    }

    void checkInput()
    {
        if(!actionsBlocked)
        {
            //Keyboard Mode
            if (Input.GetMouseButtonDown(0))
            {
                if (character.closeCombat)
                {
                    closeCombatIn();
                }
                else
                {
                    fireIn();
                }
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                rollIn();
            }
            else if (Input.GetKeyDown(KeyCode.F))
            {
                activateIn();
            }
            else if (Input.GetKeyDown(KeyCode.Q))
            {
                GameManagement.instance.selectNextCharacter();
            }
            else if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A))
            {
                lookLeft();
                moveForward();
            }
            else if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D))
            {
                lookRight();
                moveForward();
            }
            else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A))
            {
                lookLeft();
                moveBackward();
            }
            else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.D))
            {
                lookRight();
                moveBackward();
            }
            else if (Input.GetKey(KeyCode.W))
            {
                moveForward();
            }
            else if (Input.GetKey(KeyCode.A))
            {
                lookLeft();
            }
            else if (Input.GetKey(KeyCode.D))
            {
                lookRight();
            }
            else if (Input.GetKey(KeyCode.S))
            {
                moveBackward();

            }
            else
            {
                idle();
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                jump();
            }
        }
    }

    void idle()
    {
        if(grounded)
        {
            playAnimationWithoutInterruption(idleAnimName);
        }
        else if (!grounded)
        {
            playAnimationWithoutInterruption(jumpInAnimName);
        }
    }

    void fireIn()
    {
        if(character.gun != null && grounded)
        {
            if(character.gun.cooldownRemaining <= 0)
            {
                autoAim(2,2);
                character.gun.fire();
                playAnimationWithoutInterruption(shootAnimName);
                actionsBlocked = true;
            }
        }
    }

    void fireMid()
    {

    }

    void fireOut()
    {
        actionsBlocked = false;
    }

    void closeCombatIn()
    {
        autoAim(0.5f, 0.5f);
        playAnimationWithoutInterruption(closeCombatAnimName);
        actionsBlocked = true;
    }

    void closeCombatMid()
    {
        if(autoAimTarget != null)
        {
            autoAimTarget.GetComponent<IHittable>().onHit(character.closeCombatDamage);
        }
    }

    void closeCombatOut()
    {
        actionsBlocked = false;
    }

    void rollLoop()
    {
        if (animatorIsInTransition(rollingAnimName))
        {
            transform.Translate(Vector3.forward * Time.deltaTime * character.rollSpeed);
        }
    }

    void rollIn()
    {
        if(rollingAnimName != "0" && grounded)
        {
            playAnimationWithoutInterruption(rollingAnimName);
            actionsBlocked = true;
        }
    }

    void rollOut()
    {
        actionsBlocked = false;
    }

    void activateIn()
    {
        IActivator activator = getActivatorInRange();
        if (activator != null)
        {
            if (activator.canBeActivatedByCharacter(character.assignedCharacterTypes))
            {
                if(activator.getPlayerMarker() != null)
                {
                    transform.position = activator.getPlayerMarker().position;
                    transform.rotation = activator.getPlayerMarker().rotation;
                }

                if(activateAnimName != "0")
                {
                    playAnimationWithoutInterruption(activateAnimName);
                    actionsBlocked = true;
                }
                else
                {
                    activateMid();
                }
            }
        }
    }

    void activateMid()
    {
        IActivator activator = getActivatorInRange();
        if (activator != null)
        {
            activator.activate(gameObject);
        }

    }

    void activateOut()
    {
        actionsBlocked = false;
    }

    void moveForward()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * character.speed);
        if(grounded)
        {
            playAnimationWithoutInterruption(runAnimName);
        }
    }

    void moveBackward()
    {
        transform.Translate(-Vector3.forward * Time.deltaTime * character.speed);
        if (grounded)
        {
            playAnimationWithoutInterruption(runAnimName);
        }
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
        if (!animatorIsInTransition(rollingAnimName))
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * character.rotationSpeed);
        }
    }

    void jump()
    {
        if (!character.doubleJump)
        {
            if (grounded)
            {
                rBody.AddForce(transform.up * character.jumpForce);
                playAnimationWithoutInterruption(jumpInAnimName);
            }
        }
        else if (character.doubleJump)
        {
            if (grounded)
            {
                rBody.AddForce(transform.up * character.jumpForce);
            }
            else if (!grounded && !character.usedDoubleJump)
            {
                rBody.AddForce(transform.up * character.jumpForce);
                character.usedDoubleJump = true;
            }
        }
    }

    void jumpLoop()
    {
        RaycastHit raycastHit;
        bool hit = Physics.Raycast(transform.position, -transform.up, out raycastHit, 10, -1);
        Debug.DrawRay(transform.position, raycastHit.point * 10, Color.cyan);
        if (hit)
        {
            if (Vector3.Distance(transform.position, raycastHit.point) < 0.1f)
            {
                grounded = true;
                if (character.doubleJump)
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

    void autoAim(float range, float radius)
    {
        List<Collider> inRange = new List<Collider>(Physics.OverlapSphere(transform.position + transform.forward * range, radius)).OrderBy(p => Vector3.Distance(p.transform.position, transform.position)).ToList<Collider>();
        if (inRange.Count > 0)
        {
            foreach (Collider col in inRange)
            {
                if (col.gameObject.GetInstanceID() != gameObject.GetInstanceID() && col.transform.GetComponent<IHittable>() != null)
                {
                    GameObject target = col.gameObject;
                    autoAimTarget = target.transform;
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

    IActivator getActivatorInRange()
    {
        IActivator result = null;
        RaycastHit raycastHit;
        bool hit = Physics.Raycast(transform.position, transform.forward, out raycastHit, character.activationRange, -1);
        if (hit)
        {
            if (raycastHit.collider.transform.GetComponent<IActivator>() != null)
            {
                result = raycastHit.collider.transform.GetComponent<IActivator>();
            }
        }

        return result;
    }

    //Doesn't interrupt current playing animation
    public void playAnimationWithoutInterruption(string name)
    {
        if (animator != null && !animatorIsInTransition(name) && name != "0")
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

    public void respawn()
    {
        transform.position = lastCheckpoint.transform.position;
        transform.rotation = lastCheckpoint.transform.rotation;
    }
}

