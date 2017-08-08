using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Subscribe functions to the EarlyUpdate event within Awake function. 
/// Ensure the EarlyUpdateManager script is set first in Unity's script execution order.
/// <para>
/// Delegated functions should check enabled state before executing.
/// </para>
/// </summary>
public class EarlyUpdateManager : MonoBehaviour {

    public static event System.Action EarlyUpdate;

    void Update ()
    {
        if (EarlyUpdate != null)
            EarlyUpdate();
	}
}
