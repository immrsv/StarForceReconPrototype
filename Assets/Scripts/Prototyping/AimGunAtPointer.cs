using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimGunAtPointer : MonoBehaviour
{
  
    private PlayerAim _aim = null;

	void Start ()
    {

        _aim = GetComponentInParent<PlayerAim>();
	}
	
	void Update ()
    {
        if (_aim)
        {
            if (_aim.IsAiming)
            {
                Vector3 point = _aim.GetAimPoint;
                transform.LookAt(point, Vector3.up);
            }
        }
	}
}
