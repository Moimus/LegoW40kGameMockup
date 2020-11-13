using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IActivator
{
    void activate(GameObject source); //triggered by player
    void activateLinkedMachines();
    void onActivationCompleted(); //triggered by animationEvent
    void onDeactivationComplete(); //triggered by animationEvent
    bool canBeActivatedByCharacter(List<int> characterTypes);
    Transform getPlayerMarker();
}
