using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    #region General

    [SerializeField]    private Transform _gunOrigin = null;
    [Range(0.0f, 10.0f), SerializeField]    private float _spread = 0.0f;

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
    private float _fireRate = 0.0f;

    #endregion

    #region Ammo Variables

    [SerializeField]    private bool _bottomlessClip = false;
    [SerializeField]    private bool _infiniteAmmo = false;
    [SerializeField]    private uint _startAmmo = 100;
    [Range(1, 5), SerializeField]   private uint _ammoPerShot = 1;
    [Range(5, 100), SerializeField] private uint _clipSize = 30;
    private uint _currentClip = 0;
    private bool _reloadRequired = false;
    public bool reloadRequired
    {
        get { return _reloadRequired; }
    }

    #endregion

    #region Heat Mechanics

    private bool _useHeat = false;

    #endregion

    void Awake()
    {
        // Get fire rate in shots per second
        _fireRate = _fireRateRPM / 60;
    }

    /// <summary>
    /// Fires the gun.
    /// </summary>
    public void Fire()
    {
        // Handle fire-rate
        _timeSinceLastFire += Time.deltaTime;
        bool fireRateQualified = (_timeSinceLastFire >= _fireRate);

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
            else
                _reloadRequired = true;
        }
    }

    private Vector3 GetSpreadDirection()
    {
        // Randomize a spread angle
        float angle = Random.Range(0, 360);
        float spread = Random.Range(-_spread, _spread);

        float spreadX = Mathf.Cos(angle * Mathf.Deg2Rad) * spread;
        float spreadY = Mathf.Sin(angle * Mathf.Deg2Rad) * spread;

        // Get initial forward point
        Vector3 point = _gunOrigin.position + _gunOrigin.forward;

        // Offset point to apply spread
        point += (_gunOrigin.up * spreadY) + (_gunOrigin.right * spreadX);

        // Get vector to the new point
        Vector3 toSpreadPoint = point - _gunOrigin.position;

        return toSpreadPoint.normalized;
    }

    /// <summary>
    /// Used internally to fire a single shot using a raycast.
    /// </summary>
    private void FireShot()
    {
        Vector3 spreadDirection = GetSpreadDirection();

        // TODO: Shoot ray in this direction
    }
}
