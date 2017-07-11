using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Static class for general settings to track anything that may change during gameplay. */
public static class stSettings
{
    private static bool _canSwitchSquadMembers = true;
    public static bool CanSwitchSquadMembers
    {
        get { return _canSwitchSquadMembers; }
        set { _canSwitchSquadMembers = value; }
    }

    // Resets all settings to default values
    public static void Reset()
    {
        _canSwitchSquadMembers = true;
    }
	
}
