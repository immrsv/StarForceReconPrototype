using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private static CameraController _singletonInstance;

    public static CameraController GetInstance
    {
        get { return _singletonInstance; }
    }

    private Vector3 _focusPoint;       // Tracks the main focus of the camera
    private Vector3 _oldFocusPoint;      // Previous center point. Used for lerping
    private Vector3 _prevLookPoint;
    private bool _camLerping = false;
    private float _camLerpProgress = 0.0f;

    [Range(10.0f, 85.0f)]   public float _pitch = 45.0f;
    [Range(10.0f, 25.0f)]   public float _idealDistance = 15.0f;
    [Range(0.0f, 0.6f)]  public float _aimOffsetDistance = 0.25f;

    [Range(0.2f, 0.6f), Tooltip("Time in seconds it takes for the camera to reach a new target location, when switching controlled characters")]
    public float _switchCharacterTime;     // Time taken for the camera to lerp to new position when switching characters

    private List<Transform> _pointsOfInterest;      // A list of positions the camera will aim to keep in view

    // Rotation info
    [SerializeField]        private float _rotation;
    [Range(60.0f, 270.0f), Tooltip("The number of degrees the camera will rotate each second when using the rotate buttons")]
    public float _rotationSpeed = 60.0f;

    [Tooltip("Controls the speed the camera will aim to move at when at a given distance from it's focus point. \n(t=0) is the speed when at the focus point, (t=1) is the speed when at the _idealDistance")]
    public AnimationCurve _speedAtDistance;
    [Range(1.0f, 40.0f)]  public float _speedModifier = 30.0f;

    void Awake ()
    {
        // Create singleton instance
        if (!_singletonInstance)
            _singletonInstance = this;
        else
            DestroyImmediate(this); // Instance already exists

        // TODO: Initial placement of camera. Account for rotation, distance, etc.
    }

    void Start ()
    {
        // Set follow points to current squad members location
        SquaddieSwitchController currentSquaddie = stSquadManager.GetCurrentSquaddie;

        if (currentSquaddie)
        {
            _oldFocusPoint = currentSquaddie.transform.position;
            _focusPoint = currentSquaddie.transform.position;
        }

        // Add an event handler for squad member switching
        stSquadManager.OnSwitchSquaddie += StSquadManager_OnSwitchSquaddie;
	}

    // Event Handler for squad member switching. Handles cam lerping to new location
    private void StSquadManager_OnSwitchSquaddie()
    {
        _camLerping = true;
        _camLerpProgress = 0.0f;

        SquaddieSwitchController currentSquaddie = stSquadManager.GetCurrentSquaddie;
        if (currentSquaddie)
        {
            _oldFocusPoint = _focusPoint;
            _focusPoint = currentSquaddie.transform.position;
            _prevLookPoint = _focusPoint;
        }
    }

    void Update ()
    {
        // Handle rotation input
        if (Input.GetButton("CameraRotation"))
        {
            // Add or subtract rotation
            if (Input.GetAxisRaw("CameraRotation") > 0)
                _rotation += Time.deltaTime * _rotationSpeed;
            else
                _rotation -= Time.deltaTime * _rotationSpeed;

            // Keep rotation value mapped from 0 to 360
            MapRotation();
        }

        Camera mainCam = Camera.main;

        // Get some offset values
        Vector3 characterPoint = GetCenterPoint();                              // Focus point for current character in world coords. Lerps when switching characters
        Vector3 offset = GetRotationalOffset();                                 // Get camera offset based on rotation. This is normalized
        Vector3 aimOffset = (!_camLerping) ? GetAimOffset() : Vector3.zero;     // Get offset from aim pointer. Only effective if not lerping to a new character

        // Get current & desired positions
        Vector3 currentPos = mainCam.transform.position;
        Vector3 lookTargetDesired = (characterPoint + aimOffset);

        // Lerp look target over time
        Vector3 lookPrevToDesired = (lookTargetDesired - _prevLookPoint);
        float lookPct = (_prevLookPoint - currentPos).magnitude / (lookTargetDesired - currentPos).magnitude;
        Vector3 lookTarget = _prevLookPoint + lookPrevToDesired * lookPct;

        Vector3 desiredPos = lookTarget + offset;
        desiredPos = lookTarget - (lookTarget - desiredPos).normalized * _idealDistance;

        // TEMP: DELETE THIS
        Debug.DrawRay(lookTarget, Vector3.up);
        Debug.DrawRay(desiredPos, Vector3.up);

        if (desiredPos != currentPos)
        {
            // Get the vector from the current position to the desired position
            Vector3 toDesired = desiredPos - currentPos;

            // Find how far along the _idealDistance the camera is & find the desired speed
            float movementPercent = (lookTarget - currentPos).magnitude / (lookTarget - desiredPos).magnitude;
            float distPercent = movementPercent / _idealDistance;
            float moveSpeedThisFrame = _speedAtDistance.Evaluate(distPercent) * _speedModifier * Time.deltaTime;
            
            Vector3 finalOffset = toDesired.normalized * moveSpeedThisFrame;
            Vector3 finalPos = finalOffset + currentPos;
            
            // Check if the new position overshot the desired position
            if (Vector3.Distance(currentPos, finalPos) > Vector3.Distance(currentPos, desiredPos))
                finalPos = desiredPos;
            
            mainCam.transform.position = finalPos;
        }

        mainCam.transform.LookAt(lookTarget);

        _prevLookPoint = lookTarget;

        //// Get desired & current position
        //Vector3 camFocusPoint = Vector3.forward;
        //Vector3 desiredPosition = GetNewCameraPosition(out camFocusPoint);
        //Vector3 currentPosition = mainCam.transform.position;
        //
        //Debug.DrawRay(desiredPosition, Vector3.up);
        //
        //if (desiredPosition != currentPosition)
        //{
        //    // Get the vector from the current position to the desired position
        //    Vector3 toDesired = desiredPosition - currentPosition;
        //
        //    // Find how far along the _idealDistance the camera is & find the desired speed
        //    float movementPercent = (camFocusPoint - currentPosition).magnitude / (camFocusPoint - desiredPosition).magnitude;
        //    float distPercent = movementPercent / _idealDistance;
        //    float moveSpeedThisFrame = _speedAtDistance.Evaluate(distPercent) * _speedModifier * Time.deltaTime;
        //
        //    Vector3 finalOffset = toDesired.normalized * moveSpeedThisFrame;
        //    Vector3 finalPos = finalOffset + currentPosition;
        //
        //    // Check if the new position overshot the desired position
        //    if (Vector3.Distance(currentPosition, finalPos) > Vector3.Distance(currentPosition, desiredPosition))
        //        finalPos = desiredPosition;
        //
        //    mainCam.transform.position = finalPos;
        //}
        //
        //mainCam.transform.LookAt(camFocusPoint);
    }

    // Adds the specified transform as a point of interest for the camera
    public void PushPointOfInterest(Transform t)
    {
        if (t)
            _pointsOfInterest.Add(t);
    }

    // Removes the specified transform as a point of interest for the camera
    public void PopPointOfInterest(Transform t)
    {
        if (t)
            _pointsOfInterest.RemoveAll(trans => trans == t);
    }
    
    // Internal use only. Keeps rotation value mapped between 0 & 360
    private void MapRotation()
    {
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

    // Internal use only. Calculates the final camera offset & lerps to it
    private Vector3 GetNewCameraPosition(out Vector3 lookPoint)
    {
        Camera mainCam = Camera.main;

        // Get focus point of the camera
        Vector3 focusPoint = GetCenterPoint();

        // Find how close the focusPoint is from it's desired point
        float diff = (focusPoint - _focusPoint).magnitude;

        // Get camera offset based on rotation
        Vector3 offset = GetRotationalOffset();

        // Get offset from aim pointer
        Vector3 aimOffset = (diff < 0.5f) ? GetAimOffset() : Vector3.zero;

        lookPoint = focusPoint + aimOffset;
        Vector3 desiredPos = lookPoint + offset * _idealDistance;
        
        return desiredPos;
    }

    // Internal use only. Handles lerping of center point when switching between characters
    private Vector3 GetCenterPoint()
    {
        if (!_camLerping)
            return _focusPoint;

        // Calculate how far into the lerp the camera currently is
        _camLerpProgress += Time.deltaTime;

        // Get value 0-1 for progress through lerp
        float progress = _camLerpProgress / _switchCharacterTime;

        // Check for end of lerp
        if (_camLerpProgress >= _switchCharacterTime)
        {
            _camLerpProgress = _switchCharacterTime;
            _camLerping = false;
        }

        // Clamp progress value.
        if (progress > 1.0f)    progress = 1.0f;

        // Get a vector from the old point to the new point
        Vector3 prevToCurrentPoint = _focusPoint - _oldFocusPoint;

        // Get the position between both points @ progress
        Vector3 point = _oldFocusPoint + (prevToCurrentPoint * progress);
        
        return point;
    }

    // Internal use only. Calculates the camera offset based on rotation
    private Vector3 GetRotationalOffset()
    {
        Vector3 offset = new Vector3(Mathf.Sin( (_rotation - 90) * Mathf.Deg2Rad ), 
                                        Mathf.Tan(_pitch * Mathf.Deg2Rad), 
                                        Mathf.Cos( (_rotation + 90) * Mathf.Deg2Rad) );

        offset = offset.normalized;
        return offset;
    }

    // Internal use only. Calculates a camera offset based on how far the mouse is from the center of the screen
    private Vector3 GetAimOffset()
    {
        if (_aimOffsetDistance > 0)
        {
            // Get a reference to the current squaddie's aim script
            SquaddieSwitchController currentSquaddie = stSquadManager.GetCurrentSquaddie;
            if (currentSquaddie)
            {
                PlayerAim aimScript = currentSquaddie.gameObject.GetComponent<PlayerAim>();
                if (aimScript)
                {
                    // Get aim point
                    if (aimScript.IsAiming)
                    {
                        Vector3 aimPoint = aimScript.GetAimPoint;
                        Vector3 offsetPoint = aimPoint - currentSquaddie.transform.position;

                        return offsetPoint * _aimOffsetDistance;
                    }
                }
            }
        }
        return Vector3.zero;
    }
}
