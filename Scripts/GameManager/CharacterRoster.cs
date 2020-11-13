using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterRoster
{
    public int rosterPointer = 0; //currently selected character
    List<GameObject> roster = new List<GameObject>();
    public string playerCharacterFolder = "Prefabs/Minis/Player/";

    public CharacterRoster()
    {
        GameObject[] characters = Resources.LoadAll<GameObject>(playerCharacterFolder);
        roster = new List<GameObject>(characters);
        Debug.Log("Loaded Roster");
    }

    public GameObject getNextCharacter()
    {
        GameObject character = null;
        if(rosterPointer < roster.Count -1)
        {
            rosterPointer++;
            character = roster[rosterPointer];
        }
        else
        {
            rosterPointer = 0;
            character = roster[rosterPointer];
        }

        return character;
    }
}
