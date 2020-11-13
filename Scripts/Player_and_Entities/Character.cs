using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour, IHittable
{
    public bool isPlayer = false;
    public int hpMax = 4;
    int hpCurrent = 4;
    public float speed = 2f;
    public float rotationSpeed = 360f;
    public float jumpForce = 300;
    public bool doubleJump = false;
    public bool usedDoubleJump = false;
    public float rollSpeed = 3f;
    public float activationRange = 0.5f;
    public Gun gun = null;
    public bool closeCombat = false;
    public int closeCombatDamage = 1;
    public int faction = 0;
    public enum factions
    {
        friendly = 0,
        enemy = 1
    }
    public List<int> assignedCharacterTypes = new List<int>();
    public enum characterTypes
    {
        humanoid = 0,
        nonHumanoid = 1,
        human = 2,
        robot = 3
    }

    // Start is called before the first frame update
    void Start()
    {
        init();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void init()
    {
        hpCurrent = hpMax;
    }

    public void onHit(int damage)
    {
        hpCurrent -= damage;
        checkAlive();
    }

    public void checkAlive()
    {
        if(hpCurrent <= 0)
        {
            kill();
        }
    }

    public void kill()
    {
        if(isPlayer)
        {
            PlayerControllerAdvanced pc = transform.GetComponent<PlayerControllerAdvanced>();
            pc.respawn();
        }
        else
        {
            Destroy(gameObject); //TODO
        }
    }
}
