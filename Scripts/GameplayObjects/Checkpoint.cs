using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<PlayerControllerAdvanced>() != null)
        {
            other.gameObject.GetComponent<PlayerControllerAdvanced>().lastCheckpoint = this;
        }
    }
}
