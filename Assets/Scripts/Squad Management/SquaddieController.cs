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

    [Tooltip("Delay in seconds before re-enabling Controller scripts.")]
    [Range(0.2f, 1.0f), SerializeField]    private float _selectionDelay = 0.2f;

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
    /// Internal function called by SelectSquaddie after a delay.
    /// </summary>
    private void Select()
    {
        SetListEnableState(_AIScripts, false);
        SetListEnableState(_ControllerScripts, true);
    }

    /// <summary>
    /// Enables AI & disables Controller scripts so the squad member is played by the AI
    /// </summary>
    public void DeselectSquaddie()
    {
        // Cancel invoked select method
        CancelInvoke("Select");

        SetListEnableState(_AIScripts, true);
        SetListEnableState(_ControllerScripts, false);
    }

    /// <summary>
    /// Disables AI & enables Controller scripts so the player is controlling this squad member
    /// </summary>
    public void SelectSquaddie()
    {
        Invoke("Select", _selectionDelay);
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
