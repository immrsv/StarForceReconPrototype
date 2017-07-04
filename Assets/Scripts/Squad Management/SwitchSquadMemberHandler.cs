using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This script should be placed on a Game Manager object in the scene.
 * Handles character switching. */
public class SwitchSquadMemberHandler : MonoBehaviour
{
    
	void Update ()
    {
        // Check if switch-button was pressed this frame
        if (Input.GetButtonDown("SwitchSquaddie"))
            stSquadManager.Switch(Input.GetAxisRaw("SwitchSquaddie") < 0);    // Switch is reversed if the negative button is used
    }
}
