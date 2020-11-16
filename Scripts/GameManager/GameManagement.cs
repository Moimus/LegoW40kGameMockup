using System.Collections;
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

    public float respawnDelay = 2.0f;

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

    public IEnumerator respawnCharacter(Character oldCharacter)
    {
        oldCharacter.GetComponent<PlayerControllerAdvanced>().actionsBlocked = true;
        GameObject newCharacter = Instantiate(characterRoster.roster[characterRoster.rosterPointer]);
        playerController = newCharacter.GetComponent<PlayerControllerAdvanced>();
        newCharacter.GetComponent<PlayerControllerAdvanced>().actionsBlocked = true;
        newCharacter.transform.position = oldCharacter.gameObject.GetComponent<PlayerControllerAdvanced>().lastCheckpoint.transform.position;
        newCharacter.transform.rotation = oldCharacter.gameObject.GetComponent<PlayerControllerAdvanced>().lastCheckpoint.transform.rotation;
        Destroy(oldCharacter.gameObject);
        yield return new WaitForSeconds(respawnDelay);
        followCam.target = newCharacter.transform;
        newCharacter.GetComponent<PlayerControllerAdvanced>().actionsBlocked = false;
        yield return null;
    }

    public void selectNextCharacter()
    {
        //save old character params
        Vector3 playerPosition = playerController.transform.position;
        Quaternion playerRotation = playerController.transform.rotation;
        PlayerControllerAdvanced oldController = playerController;
        Checkpoint lastCheckpoint = oldController.lastCheckpoint;
        int characterHP = oldController.character.hpCurrent;

        //load new characters and link to related objects
        GameObject newCharacter = characterRoster.getNextCharacter();
        GameObject newCharacterInstance = Instantiate(newCharacter, playerPosition, playerRotation);
        playerController = newCharacterInstance.GetComponent<PlayerControllerAdvanced>();
        playerController.lastCheckpoint = lastCheckpoint;
        playerController.character.hpCurrent = characterHP;
        followCam.target = newCharacterInstance.transform;

        //destroy old character and clean up
        Destroy(oldController.gameObject);
        if(spawnFX != null)
        {
            GameObject fxInstance = Instantiate(spawnFX, playerPosition, playerRotation);
        }
    }
}
