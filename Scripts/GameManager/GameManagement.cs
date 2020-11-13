﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagement : MonoBehaviour
{
    public static GameManagement instance = null; //singleton

    public CharacterRoster characterRoster = null;
    public string playerCharacterFolder = "Resources/Prefabs/Minis/Player/";
    public PlayerControllerAdvanced playerController = null;
    public GameObject spawnFX = null;

    public FollowCamera followCam = null;

    // Start is called before the first frame update
    void Start()
    {
        init();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void init()
    {
        instance = this;
        characterRoster = new CharacterRoster();
    }

    public void selectNextCharacter()
    {
        //save old character params
        Vector3 playerPosition = playerController.transform.position;
        Quaternion playerRotation = playerController.transform.rotation;
        PlayerControllerAdvanced oldController = playerController;

        //load new characters and link to related objects
        GameObject newCharacter = characterRoster.getNextCharacter();
        GameObject newCharacterInstance = Instantiate(newCharacter, playerPosition, playerRotation);
        playerController = newCharacterInstance.GetComponent<PlayerControllerAdvanced>();
        followCam.target = newCharacterInstance.transform;

        //destroy old character and clean up
        Destroy(oldController.gameObject);
        if(spawnFX != null)
        {
            GameObject fxInstance = Instantiate(spawnFX, playerPosition, playerRotation);
        }
    }
}