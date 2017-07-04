using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This script should be placed on each playable squad member in the scene. 
 * Handles player aiming. */
public class PlayerAim : MonoBehaviour
{

    [Tooltip("A list of tags the player can aim at. The player will aim at the first object under the mouse which has a tag included in this list.")]
    public string[] _aimTags;

    private Vector3 _aimPoint;
    private bool _aimingAtGeometry = false;     // Tracks whether or not the mouse is over aimable geometry
    
	void Start ()
    {
		
	}
	
	void Update ()
    {
        // Get ray through main camera @ mouse position
        Camera mainCam = Camera.main;
        Vector3 mainCamPos = mainCam.transform.position;
        Ray mouseRay = mainCam.ScreenPointToRay(Input.mousePosition);

        // Get an array of all raycasthits for this ray
        RaycastHit[] rayHits = Physics.RaycastAll(mouseRay);

        if (rayHits.Length > 0)
        {
            // Sort by distance from camera
            System.Array.Sort(rayHits, delegate(RaycastHit r1, RaycastHit r2) {
                return Vector3.Distance(mainCamPos, r1.point).CompareTo(Vector3.Distance(mainCamPos, r2.point));
            } );

            // Loop through each hit & compare to allowed tags to find the first valid hit
            RaycastHit firstHit = new RaycastHit();
            for (uint i = 0; i < rayHits.Length; i++)
            {
                RaycastHit hit = rayHits[i];

                // Find the hit's transform tag in the aimTags list
                string found = null;
                found = System.Array.Find(_aimTags, delegate (string s) {
                    return s == hit.transform.tag;
                } );

                if (found != null)
                {
                    firstHit = hit;
                    break;
                }
            }

            // Check if a valid hit was found
            if (firstHit.transform)
            {
                _aimPoint = firstHit.point;
                _aimingAtGeometry = true;
                Debug.Log("HIT: " + firstHit.collider.gameObject.name);

            }
            else
                _aimingAtGeometry = false;
        }
        else
            _aimingAtGeometry = false;
    }

    void OnDrawGizmos()
    {
        if (_aimingAtGeometry)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, _aimPoint);
        }
    }
}
