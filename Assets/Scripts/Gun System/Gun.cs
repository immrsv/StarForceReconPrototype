using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField]    private Transform _gunOrigin = null;
    private bool _triggerDown = false;

    #region Ammo Variables

    private bool _bottomlessClip = false;
    private uint _clipSize = 30;

    #endregion

    void Awake()
    {
        
    }

    void Start ()
    {
		
	}

    void FixedUpdate ()
    {
        if (_gunOrigin && _triggerDown)
        {
            ;
        }
	}

    public void TriggerDown()
    {
        _triggerDown = true;
    }

    public void TriggerUp()
    {
        _triggerDown = false;
    }
}
