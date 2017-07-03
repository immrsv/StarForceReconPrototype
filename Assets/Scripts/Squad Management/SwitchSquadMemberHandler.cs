using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This script should be placed on a Game Manager object in the scene.
 * Handles character switching. */
public class SwitchSquadMemberHandler : MonoBehaviour
{
    
	void Update ()
    {
        // Check if button is pressed
        if (Input.GetButtonDown("SwitchSquaddie") && Input.GetAxisRaw("SwitchSquaddie") > 0)
            SwitchCharacter();

        // Check reverse button
        if (Input.GetButtonDown("SwitchSquaddie") && Input.GetAxisRaw("SwitchSquaddie") < 0)
            SwitchCharacter(true);
    }

    private void SwitchCharacter(bool reverse = false)
    {
        Debug.Log("Switching " + ((!reverse) ? "forward" : "backwards"));
        stSquadManager.Switch(reverse);
    }
}
