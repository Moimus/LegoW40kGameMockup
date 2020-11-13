using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMachine
{
    void onTurnOn(); //what happens when the machine is turned on
    void onTurnOff(); //what happens when the machine is turned off
    void runLoop(); //what happens each frame while the machine is turned on
    int getState(); //returns the current state of the machine i.e. off = 0, on = 1, etc.
}
