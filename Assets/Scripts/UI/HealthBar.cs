using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [SerializeField]    private Health _health = null;

    // float from 0-1 for how full the bar is
    private float _barFill = 0;

    void Awake()
    {
        // Subscribe to the damage event for the health script
        if (_health != null)
            _health.OnDamage += _health_OnDamage;
    }

    private void _health_OnDamage(Health sender, float damageValue)
    {
        // This is called each time the health is changed. 
        // This is where we update the bar's values

        // TODO:
        // Use the 'sender' parameter to get the max health
        // Get the current health
        // divide current by max and set _barFill to this value
        // This is now the value to fill the bar

        // Attach a slider UI component or something, and set the value
    }

    void Start ()
    {
		
	}
	
	void Update ()
    {
		
	}
}
