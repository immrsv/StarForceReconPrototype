using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LaserTest : MonoBehaviour
{

    public Vector3 _origin;
    private PlayerAim _aimScript;
    public LineRenderer _line;
    
	void Start ()
    {
        _aimScript = GetComponent<PlayerAim>();
    }
	
	void Update ()
    {
        // TODO: Rotate laser with player
        if (_aimScript && _line)
        {
            Vector3 endPoint = _aimScript.GetAimMousePoint;
            Vector3[] positions = { _origin + _line.transform.position, endPoint};
            _line.SetPositions(positions);


            if (_aimScript.IsAiming)
                _line.enabled = true;
            else
                _line.enabled = false;
        }
	}
}
