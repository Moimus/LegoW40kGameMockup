using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    public PlayerControllerAdvanced owner = null;
    public int marginHorizontal = 32;
    public int marginVertical = 32;
    /// <summary>
    /// 1 = left, -1 = right
    /// </summary>
    int orientation = -1;
    public GameObject heartPrefab = null;
    public GameObject heartBar = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void init()
    {
        if(owner.playerNumber == 1)
        {
            orientation = 1;
        }
        else if(owner.playerNumber == 2)
        {
            orientation = -1;
        }
        updateHearts();
    }

    public void updateHearts()
    {
        foreach(Transform t in heartBar.transform)
        {
            Destroy(t.gameObject);
        }

        int offsetX = 0;
        int offsetStep = 64;
        for(int n = 0; n < owner.character.hpCurrent; n++)
        {
            GameObject heart = Instantiate(heartPrefab, heartBar.transform);
            heart.GetComponent<RectTransform>().localPosition = new Vector2((offsetX + offsetStep + marginHorizontal) * orientation, 0);
            offsetX += offsetStep;
        }
    }


}
