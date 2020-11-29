using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagement : MonoBehaviour
{
    public static GameManagement instance = null; //singleton

    public PlayerUI player1UI = null;
    public PlayerUI player2UI = null;

    public CharacterRoster characterRoster = null;
    public string playerCharacterFolder = "Resources/Prefabs/Minis/Player/";
    public PlayerControllerAdvanced playerController1 = null;
    public PlayerControllerAdvanced playerController2 = null;
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
        initPlayerUI(playerController1);
    }

    public IEnumerator respawnCharacter(Character oldCharacter)
    {
        oldCharacter.GetComponent<PlayerControllerAdvanced>().actionsBlocked = true;
        GameObject newCharacter = Instantiate(characterRoster.roster[characterRoster.rosterPointer]);
        playerController1 = newCharacter.GetComponent<PlayerControllerAdvanced>();
        newCharacter.GetComponent<PlayerControllerAdvanced>().actionsBlocked = true;
        newCharacter.transform.position = oldCharacter.gameObject.GetComponent<PlayerControllerAdvanced>().lastCheckpoint.transform.position;
        newCharacter.transform.rotation = oldCharacter.gameObject.GetComponent<PlayerControllerAdvanced>().lastCheckpoint.transform.rotation;
        newCharacter.GetComponent<PlayerControllerAdvanced>().playerNumber = oldCharacter.GetComponent<PlayerControllerAdvanced>().playerNumber;
        newCharacter.GetComponent<Character>().playerUI = oldCharacter.GetComponent<Character>().playerUI;
        player1UI.owner = newCharacter.GetComponent<PlayerControllerAdvanced>();
        player1UI.init();
        Destroy(oldCharacter.gameObject);
        yield return new WaitForSeconds(respawnDelay);
        followCam.target = newCharacter.transform;
        newCharacter.GetComponent<PlayerControllerAdvanced>().actionsBlocked = false;
        initPlayerUI(newCharacter.GetComponent<PlayerControllerAdvanced>());
        yield return null;
    }

    void initPlayerUI(PlayerControllerAdvanced playerController)
    {
        if(playerController.playerNumber == 1 && playerController1 != null)
        {
            player1UI.owner = playerController;
            player1UI.init();
        }
        else if(playerController.playerNumber == 2 && playerController2 != null)
        {
            player2UI.owner = playerController;
            player2UI.init();
        }
    }

    public void selectNextCharacter()
    {
        //save old character params
        Vector3 playerPosition = playerController1.transform.position;
        Quaternion playerRotation = playerController1.transform.rotation;
        PlayerControllerAdvanced oldController = playerController1;
        Checkpoint lastCheckpoint = oldController.lastCheckpoint;
        int characterHP = oldController.character.hpCurrent;
        int oldplayerNumber = oldController.playerNumber;

        //load new characters and link to related objects
        GameObject newCharacter = characterRoster.getNextCharacter();
        GameObject newCharacterInstance = Instantiate(newCharacter, playerPosition, playerRotation);
        playerController1 = newCharacterInstance.GetComponent<PlayerControllerAdvanced>();
        playerController1.character.playerUI = oldController.character.playerUI;
        playerController1.lastCheckpoint = lastCheckpoint;
        playerController1.character.hpCurrent = characterHP;
        playerController1.playerNumber = oldController.playerNumber;
        player1UI.owner = playerController1;
        player1UI.init();
        followCam.target = newCharacterInstance.transform;

        //destroy old character and clean up
        Destroy(oldController.gameObject);
        if(spawnFX != null)
        {
            GameObject fxInstance = Instantiate(spawnFX, playerPosition, playerRotation);
        }
    }
}
