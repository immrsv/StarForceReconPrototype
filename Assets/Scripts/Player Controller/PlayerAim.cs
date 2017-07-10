using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using JakePerry;

/* This script should be placed on each playable squad member in the scene. 
 * Handles player aiming. */
public class PlayerAim : MonoBehaviour
{
    [Tooltip("Where the player character sees out of. This should be an child empty GameObject at the character's head.")]
    public Transform _eyes;
    private LogOnce _eyesWarningMessage = null;

    [Tooltip("A list of tags the player can aim at. The player will aim at the first object under the mouse which has a tag included in this list.")]
    public string[] _aimTags;

    private Vector3 _aimMousePoint;
    private bool _aimingAtGeometry = false;     // Tracks whether or not the mouse is over aimable geometry
    private Collider _mousePointCollider = null;
    private Vector3 _aimPoint;

    /// <summary>
    /// Returns the point under the mouse. For the actual aim point, use GetAimPoint
    /// </summary>
    public Vector3 GetAimMousePoint
    {
        get { return _aimMousePoint; }
    }

    /// <summary>
    /// Returns the aim point. For the point under the mouse, use GetAimMousePoint
    /// </summary>
    public Vector3 GetAimPoint
    {
        get { return _aimPoint; }
    }
    public bool IsAiming
    {
        get { return _aimingAtGeometry; }
    }

    void Awake()
    {
        _eyesWarningMessage = new LogOnce("Warning: No eyes transform specified on PlayerAim script.", LogOnce.LogType.Warning, gameObject);
    }
    
	void Start ()
    {
    }
	
	void Update ()
    {
        GetHitUnderMouse();

        _aimPoint = FindOptimalHitPoint();
    }

    /// <summary>
    /// Shoots a ray into the screen to find the first valid hit under the mouse pointer.
    /// The _aimMousePoint variable is set to the first valid contact point.
    /// </summary>
    private void GetHitUnderMouse()
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
            rayHits.SortByDistance(mainCamPos);

            // Loop through each hit & compare to allowed tags to find the first valid hit
            RaycastHit firstHit = new RaycastHit();
            for (uint i = 0; i < rayHits.Length; i++)
            {
                RaycastHit hit = rayHits[i];

                // Find the hit's transform tag in the aimTags list
                string found = null;
                found = System.Array.Find(_aimTags, delegate (string s) {
                    return s == hit.transform.tag;
                });

                if (found != null)
                {
                    firstHit = hit;
                    break;
                }
            }

            // Check if a valid hit was found
            if (firstHit.transform)
            {
                _aimMousePoint = firstHit.point;
                _aimingAtGeometry = true;
                _mousePointCollider = firstHit.collider;
                return;
            }
        }

        _aimingAtGeometry = false;
        _mousePointCollider = null;
    }

    /// <summary>
    /// Finds the best aim point for the currently selected character. If there are
    /// any objects blocking the character's view of the point under the mouse,
    /// the function will aim to find a better point on the target object.
    /// </summary>
    private Vector3 FindOptimalHitPoint()
    {
        if (!_mousePointCollider)
            return _aimMousePoint;

        if (_eyes)
        {
            // Get ray from character's eyes to mouse aim point & get hits along ray
            Ray ray = new Ray(_eyes.position, _aimMousePoint - _eyes.position);
            RaycastHit[] hits = Physics.RaycastAll(ray);

            if (hits.Length > 0)
            {
                // Sort hits array by distance from character, and apply aimTags mask
                hits.SortByDistance(_eyes.position);
                hits = Utils.ApplyTagMask(hits, _aimTags);

                // TESTING: DELETE LATER!!!
                foreach (RaycastHit h in hits)
                {
                    Debug.DrawRay(h.point, Vector3.up);
                }

                // Get the transform attached to the collider under the mouse
                Transform underMouseTransform = _mousePointCollider.transform.GetTopLevelParent();

                // Check if the closest hit belongs to the same transform
                RaycastHit closestHit = hits[0];
                Transform hitTransform = closestHit.transform.GetTopLevelParent();

                if (hitTransform == underMouseTransform)
                    return closestHit.point;

                // If the code reaches this point, there is something obstructing the character's view
                // of the point under the mouse. The script will attempt to find a new point

                // TODO: Finish this
                return _aimMousePoint;
            }
            else
            {
                // Something probably went wrong, the ray should've hit the _aimMousePoint.
                // This could be an issue if the function is called somewhere when _aimingAtGeometry
                // is false
                return _aimMousePoint;
            }
        }
        else
        {
            _eyesWarningMessage.Log();
            return _aimMousePoint;
        }
    }

    void OnDrawGizmos()
    {
        if (enabled && _aimingAtGeometry)
        {
            Gizmos.color = Color.red;
            if (_eyes)
                Gizmos.DrawLine(_eyes.position, _aimMousePoint);
            else
                Gizmos.DrawLine(transform.position, _aimMousePoint);
            Bounds b = _mousePointCollider.GetFullBounds();
            Gizmos.DrawWireCube(b.center, b.size);

            Gizmos.DrawWireSphere(_aimPoint, 0.2f);
        }
    }
}
