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

    private Vector3 _centerPoint;       // Tracks the main focus of the camera
    private Vector3 _oldPoint;      // Previous center point. Used for lerping
    private bool _camLerping = false;
    private float _camLerpProgress = 0.0f;

    [Range(0.2f, 0.6f), Tooltip("Time in seconds it takes for the camera to reach a new target location, when switching controlled characters")]
    public float _switchCharacterTime;     // Time taken for the camera to lerp to new position when switching characters

    private List<Transform> _pointsOfInterest;      // A list of positions the camera will aim to keep in view

    // Rotation info
    [SerializeField]        private float _rotation;
    [Range(60.0f, 270.0f), Tooltip("The number of degrees the camera will rotate each second when using the rotate buttons")]
    public float _rotationSpeed = 60.0f;

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
            _oldPoint = currentSquaddie.transform.position;
            _centerPoint = currentSquaddie.transform.position;
        }

        // Add an event handler for squad member switching
        stSquadManager.OnSwitchSquaddie += StSquadManager_OnSwitchSquaddie;
	}

    // Event Handler for squad member switching. Handles cam lerping to new location
    private void StSquadManager_OnSwitchSquaddie()
    {
        _camLerping = true;
        _camLerpProgress = 0.0f;
        _oldPoint = _centerPoint;
        _centerPoint = stSquadManager.GetCurrentSquaddie.transform.position;
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
        
        // Get focus point of the camera
        Vector3 focusPoint = GetCenterPoint();

        // Get camera offset based on rotation
        Vector3 offset = GetRotationalOffset();

        // TODO: Multiply offset by camera distance & add vertical offset. (offset vector currently has y = 0)
        offset += new Vector3(0,0.68f,0);
        offset *= 10.0f;
        // TODO: REMOVE TEMP CODE ^

        Vector3 camPos = focusPoint + offset;
        mainCam.transform.position = camPos;

        // TODO: Find actual center point of all points of interest, etc. Weigh focus point highly

        // Set camera to look at the focus point
        // TODO: Later this should look at the center point, once it is found
        mainCam.transform.rotation = Quaternion.LookRotation(focusPoint - mainCam.transform.position, Vector3.up);
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

    // Internal use only. Handles lerping of center point
    private Vector3 GetCenterPoint()
    {
        if (!_camLerping)
            return _centerPoint;

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
        Vector3 prevToCurrentPoint = _centerPoint - _oldPoint;

        // Get the position between both points @ progress
        Vector3 point = _oldPoint + (prevToCurrentPoint * progress);
        
        return point;
    }

    // Internal use only. Calculates the camera offset based on rotation
    private Vector3 GetRotationalOffset()
    {
        Vector3 offset = new Vector3(Mathf.Sin( (_rotation - 90) * Mathf.Deg2Rad ), 0, Mathf.Cos( (_rotation + 90) * Mathf.Deg2Rad) );
        return offset;
    }
}
