using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LaserTest : MonoBehaviour
{

    public Transform _origin;
    private PlayerAim _aimScript;
    public LineRenderer _line;
    
	void Start ()
    {
        _aimScript = GetComponent<PlayerAim>();
    }
	
	void Update ()
    {
        if (_aimScript && _line && _origin)
        {
            Vector3 endPoint = _aimScript.GetAimPoint;
            Vector3[] positions = { _origin.position, endPoint};
            _line.SetPositions(positions);


            if (_aimScript.IsAiming)
                _line.enabled = true;
            else
                _line.enabled = false;
        }
	}

    void OnDisable()
    {
        _line.enabled = false;
    }
}
