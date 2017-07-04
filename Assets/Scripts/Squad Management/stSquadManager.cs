﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Static class for squad management. 
 * Tracks squad members, current selected member, etc. */
public static class stSquadManager
{
    // Event info
    public delegate void EventHandler();
    public static event EventHandler OnSwitchSquaddie;  // This event is called every time the selected squad member is changed

    // Selection variables
    private static List<SquaddieSwitchController> _squadMembers;
    private static int _selectedIndex;
    private static SquaddieSwitchController _selected;

    public static SquaddieSwitchController GetCurrentSquaddie
    {
        get { return _selected; }
    }

    // Sets the list of controllable squad members
    public static void SetSquadList(List<SquaddieSwitchController> members)
    {
        // Set list
        _squadMembers = members;

        // Select first member
        if (_squadMembers.Count > 0)
        {
            _selected = _squadMembers[0];
            _selectedIndex = 0;
        }
    }

    public static SquaddieSwitchController Switch(bool reverse = false)
    {
        if (_squadMembers.Count == 0)
        {
            Debug.LogWarning("Warning: No current squad members. Cannot switch.");
            return null;
        }

        if (_squadMembers.Count == 1)
        {
            // Only one squad member available
            _selected = _squadMembers[0];
            _selectedIndex = 0;
            return _selected;
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

        // Deselect currently selected squad member (re-enable AI, etc)
        if (_selected)
            _selected.DeselectSquaddie();

        // Select new squad member
        _selectedIndex = next;
        _selected = _squadMembers[_selectedIndex];

        if (_selected)
            _selected.SelectSquaddie();

        OnSwitchSquaddie();

        return _selected;
    }
}
