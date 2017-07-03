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

    private Transform _camTransform;

    private List<Transform> _pointsOfInterest;      // A list of positions the camera will aim to keep in view

    // Rotation info
    [SerializeField]        private float _rotation;
    [Range(1.0f, 180.0f)]   public float _rotationSpeed = 20.0f;

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
        // Get a reference to the main camera's transform
        _camTransform = Camera.main.transform;

        if (!_camTransform)
            DestroyImmediate(this);
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

        // TODO
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
}
