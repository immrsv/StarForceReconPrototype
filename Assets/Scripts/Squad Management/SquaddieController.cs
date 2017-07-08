using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This script should be placed on each playable squad member in the scene. 
 * This is the main 'hub' for all playable character related scripts.
 * Handles enabling/disabling of character AI & Controller scripts when switching to and from squad member. */
public class SquaddieController : MonoBehaviour
{

    // Switcher variables
    [Header("Character Switching")]
    [Tooltip("A list of AI scripts which will be enabled when the character is not being controlled by the player")]
    public List<MonoBehaviour> _AIScripts;
    [Tooltip("A list of Controller scripts which will be enabled when the character is being controlled by the player")]
    public List<MonoBehaviour> _ControllerScripts;

    void Start ()
    {
        // Add an event handler for squad member switching
        stSquadManager.OnSwitchSquaddie += StSquadManager_OnSwitchSquaddie;
    }

    /// <summary>
    /// Event Handler for squad member switching. Handles cam lerping to new location
    /// </summary>
    private void StSquadManager_OnSwitchSquaddie()
    {
        SquaddieController currentSquaddie = stSquadManager.GetCurrentSquaddie;

        if (currentSquaddie == this)
            SelectSquaddie();
        else
            DeselectSquaddie();
    }

    /// <summary>
    /// Sets all elements in the list to bool state. Useful for disabling all AI scripts
    /// when switching to a character, etc.
    /// </summary>
    private void SetListEnableState(List<MonoBehaviour> list, bool state)
    {
        foreach (MonoBehaviour m in list)
        {
            m.enabled = state;
        }
    }

    /// <summary>
    /// Enables AI & disables Controller scripts so the squad member is played by the AI
    /// </summary>
    public void DeselectSquaddie()
    {
        SetListEnableState(_AIScripts, true);
        SetListEnableState(_ControllerScripts, false);
    }

    /// <summary>
    /// Disables AI & enables Controller scripts so the player is controlling this squad member
    /// </summary>
    public void SelectSquaddie()
    {
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
