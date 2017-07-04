using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This script should be placed on each playable squad member in the scene. 
 * Handles enabling/disabling of character AI & Controller scripts when switching to and from squad member. */
public class SquaddieSwitchController : MonoBehaviour
{

    public List<MonoBehaviour> _AIScripts;
    public List<MonoBehaviour> _ControllerScripts;

    void Start ()
    {
        // Add an event handler for squad member switching
        stSquadManager.OnSwitchSquaddie += StSquadManager_OnSwitchSquaddie;
    }

    // Event Handler for squad member switching. Handles selecting/deselecting the squad member
    private void StSquadManager_OnSwitchSquaddie()
    {
        SquaddieSwitchController currentSquaddie = stSquadManager.GetCurrentSquaddie;

        if (currentSquaddie == this)
            SelectSquaddie();
        else
            DeselectSquaddie();
    }

    void Update ()
    {
		
	}

    // Internal use only. Sets all elements in list to bool state
    private void SetListEnableState(List<MonoBehaviour> list, bool state)
    {
        foreach (MonoBehaviour m in list)
        {
            m.enabled = state;
        }
    }

    public void DeselectSquaddie()
    {
        // Enable AI, disable Controller scripts
        SetListEnableState(_AIScripts, true);
        SetListEnableState(_ControllerScripts, false);
    }

    public void SelectSquaddie()
    {
        // Disable AI, enable Controller scripts
        SetListEnableState(_AIScripts, false);
        SetListEnableState(_ControllerScripts, true);
    }

    void OnDrawGizmos()
    {
        // Temporary code. Draw gizmo over selected object's head
        if (stSquadManager.GetCurrentSquaddie == this)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position + Vector3.up * 3, 0.2f);
        }
    }
}
