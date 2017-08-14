﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    #region Delegates & Events

    public delegate void GunEventDelegate(Gun sender);

    public event GunEventDelegate OnReloadSuccess;
    public event GunEventDelegate OnReloadFailed;

    #endregion

    #region General

    [SerializeField]    private Transform _gunOrigin = null;
    [Tooltip("Amount of damage done per shot. For multi-shot weapons, damage will be divided by the number of projectiles per shot.")]
    [Range(0.1f, 10.0f), SerializeField]    private float _damage = 1.0f;
    [Range(0.0f, 10.0f), SerializeField]    private float _spread = 0.0f;

    #endregion

    #region Target Variables

    [Tooltip("Which layers will be hit/ignored by the gun's shots?")]
    [SerializeField]    private LayerMask _layerMask;
    //private string[] _hittableTags;

    #endregion

    #region Firing Variables

    [SerializeField]    private bool _semiAuto = false;
    public bool semiAuto
    {
        get { return _semiAuto; }
        /* NOTE: To allow Guns to be used by the player as well as AI, semi-auto
         * functionality is only implemented by the PlayerShoot script. This allows
         * AI to call the Fire method without having to handle trigger status.
         */
    }
    private float _timeSinceLastFire = 0.0f;

    [Tooltip("Firing speed in Rounds per Minute")]
    [Range(30.0f, 300.0f), SerializeField]  private float _fireRateRPM = 150.0f;
    private float _fireWaitTime = 0.0f;

    #endregion

    #region Ammo Variables

    [SerializeField]    private bool _bottomlessClip = false;
    [SerializeField]    private bool _infiniteAmmo = false;
    [SerializeField]    private uint _startAmmo = 100;
    private uint _currentAmmo = 0;
    public uint remainingAmmoUnloaded
    {
        get { return _currentAmmo; }
    }

    [Range(1, 5), SerializeField]   private uint _ammoPerShot = 1;
    [Range(5, 100), SerializeField] private uint _clipSize = 30;
    private uint _currentClip = 0;
    public uint ammoInClip
    { get { return _currentClip; } }

    [Tooltip("Time in seconds taken to reload.")]
    [Range(0.0f, 10.0f), SerializeField]    private float _reloadTime = 0.0f;
    private bool _isReloading = false;
    public bool reloadRequired
    {
        get { return (!_bottomlessClip && _currentClip == 0); }
    }
    private IEnumerator _reloadCoroutine = null;

    #endregion

    #region Heat Mechanics

    private bool _useHeat = false;

    #endregion

    #region NonAlloc Variables

    private RaycastHit _nonAllocHit;
    private RaycastHit[] _nonAllocHits = new RaycastHit[16];
    private Ray _nonAllocRay = new Ray();

    #endregion

    private void OnValidate()
    {
        // Get time to wait before each consecutive shot
        _fireWaitTime = 1 / (_fireRateRPM / 60);
    }

    void Awake()
    {
        // Initialize ammo
        _currentClip = (_clipSize < _startAmmo) ? _clipSize : _startAmmo;
        _currentAmmo = _startAmmo - _clipSize;
    }

    private void Update()
    {
        _timeSinceLastFire += Time.deltaTime;
    }

    /// <summary>
    /// Safely raises the specified event if it has any registered listeners.
    /// </summary>
    private void RaiseEvent(GunEventDelegate e)
    {
        // Check there are any registered listeners before firing event
        GunEventDelegate del = e;
        if (del != null)
            e(this);
    }

    /// <summary>
    /// Fires the gun.
    /// </summary>
    public void Fire()
    {
        bool fireRateQualified = (_timeSinceLastFire >= _fireWaitTime);
        
        if (fireRateQualified)
        {
            // Reset fire-rate tracking
            _timeSinceLastFire = 0.0f;

            // Check ammo in clip
            if (_currentClip > 0 || _bottomlessClip)
            {
                uint ammoThisShot = _ammoPerShot;

                // Consume ammo
                if (!_bottomlessClip)
                {
                    // Only use ammo that is loaded in clip
                    if (_ammoPerShot > _clipSize)
                        ammoThisShot = _clipSize;

                    _currentClip -= ammoThisShot;
                }

                // Fire shots
                for (int i = 0; i < ammoThisShot; i++)
                {
                    FireShot();
                }
            }
        }
    }
    
    /// <returns>Vector3 direction for a random spread angle.</returns>
    private Vector3 GetSpreadDirection()
    {
        // Randomize a spread angle
        float angle = Random.Range(0, 360);
        float spread = Random.Range(0, _spread);
        float tanOfSpread = Mathf.Tan(spread * Mathf.Deg2Rad);
        
        float spreadX = Mathf.Sin(angle * Mathf.Deg2Rad) * tanOfSpread;
        float spreadY = Mathf.Cos(angle * Mathf.Deg2Rad) * tanOfSpread;

        // Get initial forward point
        Vector3 spreadPoint = _gunOrigin.forward + (_gunOrigin.up * spreadY) + (_gunOrigin.right * spreadX);

        return spreadPoint.normalized;
    }

    /// <summary>
    /// Used internally to fire a single shot using a raycast.
    /// </summary>
    private void FireShot()
    {
        Vector3 spreadDirection = GetSpreadDirection();

        // Raycast in this direction
        _nonAllocRay.origin = _gunOrigin.position;
        _nonAllocRay.direction = spreadDirection;
        int hits = Physics.RaycastNonAlloc(_nonAllocRay, _nonAllocHits, 1000.0f, (int)_layerMask, QueryTriggerInteraction.Ignore);

        if (hits > 0)
        {
            // Sort the hits array by distance from the shot origin to find the closest hit
            if (hits > 1)
            {
                for (int i = 0; i < hits - 1; i++)
                {
                    if (i == _nonAllocHits.Length) break;

                    // Get this hit & next hit
                    RaycastHit thisHit = _nonAllocHits[i];
                    RaycastHit nextHit = _nonAllocHits[i + 1];

                    // Compare hits, order by ascending distance
                    if (Vector3.Distance(thisHit.point, _gunOrigin.position) > Vector3.Distance(nextHit.point, _gunOrigin.position))
                    {
                        _nonAllocHits.SetValue(nextHit, i);
                        _nonAllocHits.SetValue(thisHit, i + 1);

                        // De-increment iterator
                        i = (i < 2) ? 0 : i - 2;
                    }
                }
            }

            _nonAllocHit = _nonAllocHits[0];
            Debug.DrawLine(_gunOrigin.position, _nonAllocHit.point, Color.blue, 0.5f);

            // Deal damage to the object hit
            float damage = _damage / _ammoPerShot;
            _nonAllocHit.transform.SendMessageUpwards("ApplyDamage", damage, SendMessageOptions.DontRequireReceiver);

        }
    }

    /// <summary>
    /// Begins a reload for this gun.
    /// </summary>
    public void DoReload()
    {
        if (!_bottomlessClip 
            && !(_currentClip == _clipSize)
            && !_isReloading)
        {
            _reloadCoroutine = HandleReloadTimer();
            StartCoroutine(_reloadCoroutine);
        }
    }
    
    private void CancelReload()
    {
        if (_reloadCoroutine != null
            && _isReloading)
            StopCoroutine(_reloadCoroutine);

        _isReloading = false;
    }

    private IEnumerator HandleReloadTimer()
    {
        uint reloadableAmmo = (_clipSize > _currentAmmo) ? _currentAmmo : _clipSize;

        // Does the gun have any ammo remaining for reload?
        if (reloadableAmmo > 0)
        {
            if (_reloadTime > 0)
            {
                _isReloading = true;
                yield return new WaitForSeconds(_reloadTime);
            }

            _isReloading = false;

            if (_infiniteAmmo)
                _currentClip = _clipSize;
            else
                _currentClip = reloadableAmmo;

            RaiseEvent(OnReloadSuccess);
        }
        else
        {
            // Raise event for reload failure
            RaiseEvent(OnReloadFailed);
        }
    }
}
