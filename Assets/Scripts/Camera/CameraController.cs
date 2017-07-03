using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Transform _camTransform;

    // Rotation info
    float _rotation;
    
	void Start ()
    {
        // Get a reference to the main camera's transform
        _camTransform = Camera.main.transform;

        if (!_camTransform)
            DestroyImmediate(this);
	}
	
	void Update ()
    {
		
	}
}
