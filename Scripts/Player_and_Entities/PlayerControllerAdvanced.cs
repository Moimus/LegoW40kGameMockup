﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerControllerAdvanced : MonoBehaviour
{
    public bool keyboardControlled = true;
    /// <summary>
    /// 1 or 2
    /// </summary>
    public int playerNumber = 1;
    public Character character = null;
    public Checkpoint lastCheckpoint = null;

    public Rigidbody rBody = null;
    public bool grounded = true;
    public bool grabbing = false;

    public Animator animator = null;
    public string idleAnimName = "Idle";
    public string runAnimName = "Run";
    public string shootAnimName = "Shoot";
    public string closeCombatAnimName = "Attack";
    public string jumpInAnimName = "Jump";
    public string jumpMidAnimName = "Jump";
    public string jumpLandAnimName = "Jump";
    public string rollingAnimName = "Roll";
    public string activateAnimName = "Activate";
    public string grabInAnimName = "Grab_In";
    public string grabJumpAnimName = "Grab_Jump";
    public bool actionsBlocked = false;
    public bool moveBlocked = false;
    public bool rotationBlocked = false;
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
                if(grounded)
                {
                    rollIn();
                }
                else
                {
                    if(character.canGrab)
                    {
                        grab();
                    }
                }
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
            else if(Input.GetKeyDown(KeyCode.Backspace))
            {
                character.onHit(20);
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
            if(!animatorIsInTransition(jumpInAnimName))
            {
                playAnimationWithoutInterruption(idleAnimName);
            }
        }
        else if (!grounded)
        {

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
        if(grounded)
        {
            autoAim(0.5f, 0.5f);
            playAnimationWithoutInterruption(closeCombatAnimName);
            actionsBlocked = true;
        }
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
        idle();
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
        if(rollingAnimName != "0")
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
        if(!moveBlocked)
        {
            transform.Translate(Vector3.forward * Time.deltaTime * character.speed);
            if (grounded)
            {
                playAnimationWithoutInterruption(runAnimName);
            }
            else if (!grounded && !animatorIsInTransition(jumpInAnimName))
            {
                playAnimationWithoutInterruption(jumpMidAnimName);
            }
        }

    }

    void moveBackward()
    {
        if (!moveBlocked)
        {
            transform.Translate(-Vector3.forward * Time.deltaTime * character.speed);
            if (grounded)
            {
                playAnimationWithoutInterruption(runAnimName);
            }
        }

    }

    void lookForward()
    {
        if(!rotationBlocked)
        {
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(transform.forward.x, 0, transform.forward.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * character.rotationSpeed);
        }

    }

    void lookLeft()
    {
        if (!rotationBlocked)
        {
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(-transform.right.x, 0, -transform.right.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * character.rotationSpeed);
        }

    }

    void lookRight()
    {
        if (!rotationBlocked)
        {
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(transform.right.x, 0, transform.right.z));
            if (!animatorIsInTransition(rollingAnimName))
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * character.rotationSpeed);
            }
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
            else if(grabbing)
            {
                grabJump();
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
            if (Vector3.Distance(transform.position, raycastHit.point) < 0.1f && raycastHit.collider.gameObject.tag != "noProjectileTrigger")
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

    void grab()
    {
        GameObject grabPoint = getClosestGrabPointInRange();
        if(grabPoint != null)
        {
            moveBlocked = true;
            rotationBlocked = true;
            transform.position = grabPoint.transform.position;
            transform.rotation = grabPoint.transform.rotation;
            playAnimationWithoutInterruption(grabInAnimName);
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            grabbing = true;
        }
    }

    void grabJump()
    {
        moveBlocked = false;
        rotationBlocked = false;
        rBody.AddForce(transform.up * character.jumpForce);
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        playAnimationWithoutInterruption(grabJumpAnimName);
        grabbing = false;
    }

    //Animation event, on JumpIn out event
    public void jumpInOut()
    {
        playAnimationWithoutInterruption(jumpMidAnimName);
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

    GameObject getClosestGrabPointInRange()
    {
        List<Collider> inRange = new List<Collider>(Physics.OverlapSphere(transform.position, character.grabRange)).OrderBy(p => Vector3.Distance(p.transform.position, transform.position)).ToList<Collider>();
        foreach(Collider collider in inRange)
        {
            if(collider.gameObject.GetComponent<IGrabbable>() != null)
            {
                return collider.gameObject;
            }
        }
        return null;
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

    public IEnumerator respawn(float delay)
    {
        yield return new WaitForSeconds(delay);
        transform.position = lastCheckpoint.transform.position;
        transform.rotation = lastCheckpoint.transform.rotation;
        character.resetCharacter();
        yield return null;
    }
}

