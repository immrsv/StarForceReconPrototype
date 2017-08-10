using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Handles weapon holding and switching for characters. */
public class Arsenal : MonoBehaviour
{

    #region Delegates & Events

    public delegate void GunSwitchDelegate(Arsenal sender);
    public event GunSwitchDelegate OnSwitchGun;

    #endregion

    #region Guns

    [System.Serializable]
    private class GunHolster
    {
        [Tooltip("Reference to the gun.")]
        public Gun _gun = null;
        [Tooltip("Point at which the gun will be holstered.")]
        public Transform _holsterPoint = null;

        public static implicit operator bool(GunHolster g)
        {
            if (g == null) return false;
            return (g._gun != null && g._holsterPoint != null);
        }
    }

    [SerializeField]    private List<GunHolster> _guns = new List<GunHolster>();
    private GunHolster _current = null;
    private int _currentIndex = 0;

    public Gun currentGun
    {
        get { return _current._gun; }
    }

    #endregion

    #region General

    private bool _switchKeyDown = false;

    #endregion

    void Awake ()
    {
        // Remove null guns
        for (int i = 0; i < _guns.Count; i++)
        {
            GunHolster g = _guns[0];

            if (!g)
            {
                _guns.RemoveAt(i);
                i--;
            }
        }

        if (_guns.Count <= 0)
            Debug.LogWarning("No guns specified.", this);
        else
            _current = _guns[0];

        // Subscribe EarlyUpdate function
        EarlyUpdateManager.EarlyUpdate += EarlyUpdate;
	}

    void Start()
    {
        /* NOTE: Components will not receive the OnDestroy method call
         * if they do not have Start, Update, FixedUpdate, etc implemented.
         * 
         * The Start function is only implemented here to allow the script
         * to be enabled and disabled.
         */
    }

    private void EarlyUpdate()
    {
        if (enabled)
        {
            // Detect key down for SwitchWeapon axis
            if (Input.GetAxisRaw("SwitchWeapon") != 0)
            {
                if (!_switchKeyDown)
                {
                    _switchKeyDown = true;

                    // Get index direction
                    int newIndex = _currentIndex;
                    if (Input.GetAxisRaw("SwitchWeapon") > 0)
                    {
                        // Positive
                        newIndex += 1;
                        if (newIndex >= _guns.Count)
                            newIndex = 0;
                    }
                    else
                    {
                        // Negative
                        newIndex -= 1;
                        if (newIndex < 0)
                            newIndex = _guns.Count - 1;
                    }

                    SwitchToWeapon(newIndex);
                }
            }
            else
                _switchKeyDown = false;
        }
    }

    private void HolsterGun(GunHolster holster)
    {
        // TODO: Attach to holster transform & move to holster point
    }

    private void EquipGun(GunHolster gun)
    {
        _current = gun;

        // TODO: Attach to hands transform
    }

    private void SwitchToWeapon(int index)
    {
        if (index > 0 && index < _guns.Count)
        {
            if (index != _currentIndex)
            {
                _currentIndex = index;

                // Holster current gun
                HolsterGun(_current);

                // Equip new gun
                EquipGun(_guns[_currentIndex]);

                // Call switch event
                if (OnSwitchGun != null)
                    OnSwitchGun(this);
            }
        }
    }
}
