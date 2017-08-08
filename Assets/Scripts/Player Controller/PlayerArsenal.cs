using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Handles weapon switching for playable characters. */
public class PlayerArsenal : MonoBehaviour
{

    private class GunHolster
    {
        public Gun _gun = null;
        public Transform _holsterPoint = null;
    }

    [SerializeField]    private GunHolster _primary = null;
    [SerializeField]    private GunHolster _secondary = null;
    private GunHolster _current = null;
    private bool _currentIsPrimary = false;

	void Awake ()
    {
        if (_primary != null)
        {
            _current = _primary;
            _currentIsPrimary = true;
        }
        else if (_secondary != null)
        {
            _current = _secondary;
            _currentIsPrimary = false;
        }
        else
        {
            Debug.LogError("No primary or secondary gun specified. Script will be destroyed.", this);
            Destroy(this);
            return;
        }

        // Subscribe EarlyUpdate function
        EarlyUpdateManager.EarlyUpdate += EarlyUpdate;
	}

    private void EarlyUpdate()
    {
        if (enabled)
        {
            if (Input.GetKeyDown("SwitchWeapon"))
            {
                // TODO
            }
        }
    }
}
