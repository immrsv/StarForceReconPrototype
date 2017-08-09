using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Static class for squad management. 
 * Tracks squad members, current selected member, etc. */
public static class stSquadManager
{
    // Event info
    public delegate void SquadMemberSwitchEvent();
    public static event SquadMemberSwitchEvent OnSwitchSquaddie;  // This event is called every time the selected squad member is changed

    // Selection variables
    private static List<SquaddieController> _squadMembers;
    private static int _selectedIndex;
    private static SquaddieController _selected = null;

    public static List<SquaddieController> GetSquadMembers
    {
        get { return _squadMembers; }
    }

    public static SquaddieController GetCurrentSquaddie
    {
        get { return _selected; }
    }

    // Internal use only. Checks if there are any registered listeners for the OnSwitchSquaddie event, and calls the event if so
    private static void SafeFireOnSwitchSquaddie()
    {
        SquadMemberSwitchEvent switchEvent = OnSwitchSquaddie;
        if (switchEvent != null)    // Check there are registered listeners before firing event
            switchEvent();
    }
    
    /// <summary>
    /// Sets the list of controllable squad members
    /// </summary>
    /// <param name="members"></param>
    public static void SetSquadList(List<SquaddieController> members)
    {
        // Call switch event with null selection
        _selected = null;
        _selectedIndex = 0;
        SafeFireOnSwitchSquaddie();

        // Set list
        _squadMembers = members;

        // Select first member
        if (_squadMembers.Count > 0)
        {
            _selected = _squadMembers[0];

            SafeFireOnSwitchSquaddie();
        }
    }

    /// <summary>
    /// Switches to the next available character. Will iterate backwards if reverse is true.
    /// </summary>
    public static void Switch(bool reverse = false)
    {
        if (!stSettings.CanSwitchSquadMembers)
            return;

        if (_squadMembers.Count == 0)
        {
            Debug.LogWarning("Warning: No current squad members. Cannot switch.");
            return;
        }

        if (_squadMembers.Count == 1)
        {
            // Only one squad member available
            _selected = _squadMembers[0];
            _selectedIndex = 0;
        }

        // Get index of the next squad member
        int finalIndex = _squadMembers.Count - 1;
        int next = _selectedIndex;

        if (!reverse)
        {
            if (next >= finalIndex)
                next = 0;
            else
                next++;
        }
        else
        {
            if (next == 0)
                next = finalIndex;
            else
                next--;
        }

        // Select new squad member
        _selectedIndex = next;
        _selected = _squadMembers[_selectedIndex];

        // Trigger switch event
        SafeFireOnSwitchSquaddie();
    }
    
    /// <summary>
    /// Switches to a character at the specified zero-based index. Returns true if 
    /// the switch was successful, otherwise returns false.
    /// </summary>
    public static bool SwitchTo(int index)
    {
        if (!stSettings.CanSwitchSquadMembers)
            return false;

        if (index >= 0 && index < _squadMembers.Count)
        {
            if (index != _selectedIndex)
            {
                _selectedIndex = index;
                _selected = _squadMembers[index];

                // Trigger switch event
                SafeFireOnSwitchSquaddie();

                return true;
            }
        }

        return false;
    }
}
