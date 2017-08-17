using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using JakePerry;

/* This script should be placed on the Main Camera object in the scene.
 * Controls the camera by smoothly lerping between it's current position and a destination position. */
[DisallowMultipleComponent()]
public class CameraController : MonoBehaviour
{
    #region Member Variables

    #region Singleton

    private static CameraController _singletonInstance;
    public static CameraController GetInstance
    {
        get { return _singletonInstance; }
    }

    #endregion

    #region References & Tracking Variables

    private Camera _cam = null;

    // Data for lerping between look-focus points
    private Vector3 _previousAimOffset = Vector3.zero;
    private Vector3 _currentLookPoint = Vector3.zero;
    private Vector3 _destinationLookPoint;
    private Vector3 LookPoint
    {
        set { _currentLookPoint = value; }
    }

    #endregion
    
    #region Point-Switching Variables

    [Header("Point Switching Variables")]
    [Range(0.2f, 0.6f), Tooltip("Time in seconds it takes for the camera to reach a new target location, when switching controlled characters")]
    [SerializeField]  private float _switchCharacterTime = 0.2f;
    private float _switchTimeCurrent;
    private bool _isSwitchingToNewPoint = false;
    private float _camSwitchProgress = 0.0f;
    private Vector3 _destinationSwitchPoint = Vector3.zero;
    private Vector3 _currentSwitchPoint = Vector3.zero;
    private Vector3 _startSwitchPoint = Vector3.zero;

    #endregion

    #region Look-Point Lerping Variables

    [Header("Look-Point Lerping")]
    [Range(5.0f, 30.0f), Tooltip("The distance at which the camera will start to move at farSpeed.")]
    [SerializeField]    private float _farDistance = 10.0f;
    [Range(40.0f, 100.0f), Tooltip("The speed at which the camera will move when it is further than farDistance from it's destination")]
    [SerializeField]    private float _farSpeed = 95.0f;
    [Tooltip("Determines how quickly the camera will lerp towards it's destination.\n\n(x=0) Represents  distance 0; That is, when the camera is very close to it's destination already.\n\n(x=1) Represents the farDistance variable above.\n\n(y=0) Represents a speed of 0.\n\n(y=1) Represents the farSpeed variable above.\nIdeally this curve should have a positive gradient to make the camera slow down as it reaches it's destination.")]
    [SerializeField]    private AnimationCurve _speedAtDistance = 
                                new AnimationCurve(new Keyframe(0, 0.01f), new Keyframe(0.2f, 0.35f), 
                                                    new Keyframe(1, 1.0f));

    #endregion

    #region General

    [Header("General")]
    [Range(7.0f, 30.0f), SerializeField]    private float _hoverDistance = 10.0f;
    [Range(10.0f, 85.0f), SerializeField]   private float _pitch = 45.0f;
    [Range(0.0f, 0.25f), Tooltip("How much of a priority is the player's aim point for the camera? \nat 0: Aiming further away from the player will not offset the camera.\nat 0.25: Aiming further away will cause the camera to focus on a point 25% of the way between the character and their aim point.")]
    [SerializeField]    private float _aimOffsetDistance = 0.1f;
    [Range(100.0f, 270.0f), Tooltip("The number of degrees the camera will rotate each second when using the rotate buttons")]
    [SerializeField]    private float _rotationSpeed = 150.0f;
    private float _rotation = 0.0f;

    public Vector3 horizontalForward
    {
        get { return new Vector3(Mathf.Cos(Mathf.Deg2Rad * _rotation), 0, Mathf.Sin(Mathf.Deg2Rad * _rotation)); }
    }

    #endregion

    #region Starting variables

    [Header("Start Transform")]
    [Tooltip("If true, the following settings will be applied to the camera's transform at start")]
    [SerializeField]    private bool _overrideTransformAtStart = false;
    [Range(0, 360), SerializeField] private float _startRotation = 0;
    [SerializeField]    private Vector3 _startLookPosition = Vector3.zero;

    #endregion

    #endregion

    #region Member Functions

    void Awake()
    {
        // Create singleton instance
        if (!_singletonInstance)
            _singletonInstance = this;
        else
        {
            Debug.LogWarning("Warning: An instance of the CameraController script already exists. Instance will be deleted on object: " + gameObject.name);
            DestroyImmediate(this); // Instance already exists
        }
    }

    void Start ()
    {
        // Get a reference to the camera
        _cam = Camera.main;

        // Set start transform
        if (_overrideTransformAtStart)
        {
            _cam.transform.rotation = Quaternion.Euler(new Vector3(0, _startRotation, 0));
            _rotation = -_startRotation;
            _currentLookPoint = _startLookPosition;
            _destinationLookPoint = _startLookPosition;
        }

        // Set follow points to current squad members location
        SquaddieController currentSquaddie = stSquadManager.GetCurrentSquaddie;
        if (currentSquaddie)
            LookAtPosition(currentSquaddie.transform.position, _switchCharacterTime);

        // Add an event handler for squad member switching
        stSquadManager.OnSwitchSquaddie += StSquadManager_OnSwitchSquaddie;
    }
    
    /// <summary>
    /// Event Handler for squad member switching. Handles cam lerping to new location
    /// </summary>
    private void StSquadManager_OnSwitchSquaddie()
    {
        // Set the camera to switch to the new squaddie's position over _switchCharacterTime seconds.
        SquaddieController currentSquaddie = stSquadManager.GetCurrentSquaddie;
        if (currentSquaddie)
            LookAtPosition(currentSquaddie.transform.position, _switchCharacterTime);
    }

    void Update ()
    {
        if (!_cam) _cam = Camera.main;

        HandleRotation();
        HandlePointSwitching();

        // If the camera is lerping, no offsets should apply
        if (_isSwitchingToNewPoint)
        {
            _currentLookPoint = _currentSwitchPoint;
            _destinationLookPoint = _currentSwitchPoint;

            // Get new camera position with offsets
            Vector3 newPos = _currentLookPoint + GetRotationalOffset() * _hoverDistance;
            _cam.transform.position = newPos;
            _cam.transform.LookAt(_currentLookPoint, Vector3.up);
        }
        else
        {
            // The camera is not lerping. Get a look destination that is offset from the current character's position
            SquaddieController currentSquaddie = stSquadManager.GetCurrentSquaddie;
            if (currentSquaddie)
            {
                Vector3 characterPos = currentSquaddie.transform.position;
                Vector3 aimPointOffset = GetAimOffset();

                Vector3 finalPoint = characterPos + aimPointOffset;
                FocusOnPoint(finalPoint);
            }
            else
                FocusOnPoint(_currentLookPoint);
        }
    }

    /// <summary>
    /// Sets the camera's current destination to switch to, as well as the time it should take to get there.
    /// The camera will travel at a constant speed to reach the destination in the specified amount of time.
    /// </summary>
    public void LookAtPosition(Vector3 destination, float switchTime)
    {
        if (switchTime > 0)
        {
            // Lerp to the destination
            _isSwitchingToNewPoint = true;
            _camSwitchProgress = 0.0f;
            _startSwitchPoint = _currentLookPoint;
            _destinationSwitchPoint = destination;
            _switchTimeCurrent = switchTime;
        }
        else
        {
            // Switch immediately
            _isSwitchingToNewPoint = false;
            _camSwitchProgress = 0.0f;
            _startSwitchPoint = _currentLookPoint;
            _destinationSwitchPoint = destination;
            _switchTimeCurrent = 0.0f;

            _destinationLookPoint = destination;
            _currentLookPoint = _destinationLookPoint;
        }
    }

    /// <summary>
    /// Modifies rotation angle via player input
    /// </summary>
    private void HandleRotation()
    {
        if (Input.GetButton("CameraRotation"))
        {
            // Add or subtract rotation based on input
            if (Input.GetAxisRaw("CameraRotation") > 0)
                _rotation += Time.deltaTime * _rotationSpeed;
            else
                _rotation -= Time.deltaTime * _rotationSpeed;

            // Keep rotation value mapped in the range 0-360
            if (_rotation < 0.0f || _rotation >= 360.0f)
            {
                int rotInt = (int)_rotation;
                float rotDec = _rotation - (float)rotInt;

                rotInt = rotInt % 360;
                _rotation = (float)rotInt + rotDec;
                if (_rotation < 0)
                    _rotation += 360;
            }
        }
    }

    /// <summary>
    /// Moves the camera towards it's destination at a constant speed when a switch action is active
    /// </summary>
    private void HandlePointSwitching()
    {
        if (_isSwitchingToNewPoint)
        {
            // Calculate how far into the lerp the camera currently is
            _camSwitchProgress += Time.deltaTime;

            // Get value 0-1 for progress through lerp
            float progress = Mathf.Clamp01(_camSwitchProgress / Mathf.Abs(_switchTimeCurrent));

            // Check if the switching action should end this frame
            if (progress == 1)
                _isSwitchingToNewPoint = false;

            // Get a vector from the point the camera was focussing on when the switch started, to the switch destination
            Vector3 prevToDest = _destinationSwitchPoint - _startSwitchPoint;

            // Get the position between both points @ progress
            Vector3 point = _startSwitchPoint + (prevToDest * progress);
            _currentSwitchPoint = point;
        }
    }

    /// <summary>
    /// Returns a normalized Vector3 for the camera offset based on rotation & pitch
    /// </summary>
    private Vector3 GetRotationalOffset()
    {
        Vector3 offset = new Vector3(   Mathf.Sin((_rotation - 90) * Mathf.Deg2Rad),
                                        Mathf.Tan(_pitch * Mathf.Deg2Rad),
                                        Mathf.Cos((_rotation + 90) * Mathf.Deg2Rad));

        offset = offset.normalized;
        return offset;
    }

    /// <summary>
    /// Returns a Vector3 for the camera offset based on where the currently controlled squad member is aiming.
    /// </summary>
    private Vector3 GetAimOffset()
    {
        if (_aimOffsetDistance > 0)
        {
            // Get a reference to the current squaddie's aim script
            SquaddieController currentSquaddie = stSquadManager.GetCurrentSquaddie;
            if (currentSquaddie)
            {
                PlayerAim aimScript = currentSquaddie.gameObject.GetComponent<PlayerAim>();
                if (aimScript)
                {
                    // Get aim point
                    if (aimScript.IsAiming)
                    {
                        Vector3 aimPoint = aimScript.GetAimMousePoint;
                        Vector3 offsetPoint = aimPoint - currentSquaddie.transform.position;
                        
                        // Get offset & track for next frame
                        Vector3 offset = offsetPoint * Mathf.Clamp01(_aimOffsetDistance);
                        _previousAimOffset = offset;

                        return offset;
                    }
                    else
                        return _previousAimOffset;
                }
            }
        }
        return Vector3.zero;
    }

    /// <summary>
    /// Sets the destination look point to the specified point & transitions the camera over time
    /// </summary>
    private void FocusOnPoint(Vector3 point)
    {
        _destinationLookPoint = point;

        // Check if the look point is already at it's destination
        if (Vector3.Distance(_currentLookPoint, _destinationLookPoint) > 0)
        {
            // Get the vector from the current look point to the destination look point
            Vector3 currentToDest = _destinationLookPoint - _currentLookPoint;
            float distFromDest = currentToDest.magnitude;

            // Calculate which point on the speed-graph this distance corresponds to & get speed at that point
            float distOnGraph = Mathf.Clamp01(distFromDest / _farDistance);
            float speedAtDistance = _farSpeed * _speedAtDistance.Evaluate(distOnGraph);
            float speedThisFrame = speedAtDistance * Time.deltaTime;

            // Find the new focus point
            Vector3 newLookPoint = _currentLookPoint + (currentToDest.normalized * speedThisFrame);

            // Check for overshoot
            if (Vector3.Distance(_currentLookPoint, newLookPoint) > Vector3.Distance(_currentLookPoint, _destinationLookPoint))
                newLookPoint = _destinationLookPoint;

            // Set the look point
            _currentLookPoint = newLookPoint;
        }

        // Get new camera position with offsets
        Vector3 newPos = _currentLookPoint + GetRotationalOffset() * _hoverDistance;
        _cam.transform.position = newPos;
        _cam.transform.LookAt(_currentLookPoint, Vector3.up);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(_destinationLookPoint, 0.4f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_currentLookPoint, 0.3f);

        if (_overrideTransformAtStart)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, Quaternion.Euler(new Vector3(0, _startRotation, 0)) * Vector3.right);

            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(_startLookPosition, 0.2f);
        }
    }

    #endregion
}
