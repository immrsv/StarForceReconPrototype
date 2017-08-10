using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverheadHUD : MonoBehaviour {

    // who the HUD is attached to
    public Transform target;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = target.transform.position + Vector3.up;
        transform.forward = Camera.main.transform.forward;
	}
}
