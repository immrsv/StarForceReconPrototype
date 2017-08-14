using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Arsenal))]
public class PlayerShoot : MonoBehaviour
{

    private Arsenal _arsenal = null;
    private Gun _currentGun = null;

    private bool _fireKeyDown = false;
    private bool _reloadKeyDown = false;

	void Start ()
    {
        // Get reference to arsenal script
        _arsenal = GetComponent<Arsenal>();
        _currentGun = _arsenal.currentGun;

        // Subscribe to arsenal's gun-switch event
        _arsenal.OnSwitchGun += OnSwitchGun;
    }

    /// <summary>
    /// Updates gun currently in use when the gun is switched in the arsenal
    /// </summary>
    private void OnSwitchGun(Arsenal sender)
    {
        _currentGun = sender.currentGun;
    }

    void Update ()
    {
        if (_currentGun)
        {
            // Is the current gun semi automatic?
            bool semiAuto = _currentGun.semiAuto;

            // Get fire input
            if (Input.GetAxisRaw("Fire1") != 0)
            {
                if (!_fireKeyDown || !semiAuto)
                {
                    // Fire the weapon
                    _currentGun.Fire();
                    return;
                }
            }
            else
                _fireKeyDown = false;

            // Get reload input
            if (Input.GetAxisRaw("Reload") != 0)
            {
                if (!_reloadKeyDown)
                {
                    // Reload the weapon
                    _currentGun.DoReload();
                    return;
                }
            }
            else
                _reloadKeyDown = false;
        }
	}
}
