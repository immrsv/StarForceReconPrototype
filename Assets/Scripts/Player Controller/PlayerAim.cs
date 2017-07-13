using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using JakePerry;

/* This script should be placed on each playable squad member in the scene. 
 * Handles player aiming. */
[DisallowMultipleComponent()]
public class PlayerAim : MonoBehaviour
{
    [Tooltip("Where the character's gun fires from. This should be an empty gameobject at the end of the gun barrel.")]
    [SerializeField()]  private Transform _gunOrigin;
    private LogOnce _gunOriginWarningMessage = null;

    [Header("Smart Aim")]
    [Range(2, 5), Tooltip("The number of iterations to make in each direction, positively and negatively, when searching for a better aim point.\nNOTE: Each extra iteration can result in up to 26 new raycast checks per frame (worst case scenario). Although less accurate, a lower value may slightly improve performance.")]
    [SerializeField()]  private int _smartAimIterations = 3;
    [Tooltip("An array of tags that will be ignored by the smart aim functionality. If the cursor point is over a transform with a tag found in this array, the script will not attempt to find an optimal aim point.")]
    [SerializeField()]  private string[] _smartAimIgnoreTags = new string[] { "Untagged" };

    [Tooltip("A list of tags the player can aim at. The player will aim at the first object under the mouse which has a tag included in this list.")]
    [SerializeField()]  private string[] _aimTags = new string[] { "Untagged", "Enemy" };
    private RaycastHit[] _nonAllocHits = new RaycastHit[16];    // RaycastHit array declared here for use with RaycastNonAlloc method
    private RaycastHit _nonAllocHit;

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
        _gunOriginWarningMessage = new LogOnce("Warning: No gun origin transform specified on PlayerAim script.", LogOnce.LogType.Warning, gameObject);
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

        if (_smartAimIgnoreTags.Contains(_mousePointCollider.transform.tag))
        {
            if (!_gunOrigin)
                return _aimMousePoint;

            // Smart aim functionality should be ignored. Instead, find the nearest hit on the ray to _aimMousePoint
            _nonAllocHit = GetClosestPointOnRay(_gunOrigin.position, _aimMousePoint);

            if (_nonAllocHit.transform)
                return _nonAllocHit.point;

            return _aimMousePoint;
        }

        if (_gunOrigin)
        {
            // Get ray from character's gun origin to mouse aim point & get hits along ray
            Ray gunToMousePointRay = new Ray(_gunOrigin.position, _aimMousePoint - _gunOrigin.position);
            RaycastHit[] hits = Physics.RaycastAll(gunToMousePointRay, Vector3.Distance(_gunOrigin.position, _aimMousePoint));

            if (hits.Length > 0)
            {
                // Sort hits array by distance from character, and apply aimTags mask
                hits.SortByDistance(_gunOrigin.position);
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

                /* NOTE: If the code reaches this point, there is something obstructing the character's view
                 * of the point under the mouse. The script will attempt to find a new point */
                
                return SmartAimFindPoint(underMouseTransform);
            }
            else
            {
                /* Something probably went wrong, the ray should've hit the _aimMousePoint.
                 * This could be an issue if the function is called somewhere when 
                 * _aimingAtGeometry is false */
                return _aimMousePoint;
            }
        }
        else
        {
            _gunOriginWarningMessage.Log();
            return _aimMousePoint;
        }
    }

    /// <summary>
    /// Used internally to find the closest valid hit on a ray, using the _nonAllocHits array.
    /// </summary>
    private RaycastHit GetClosestPointOnRay(Vector3 origin, Vector3 destination)
    {
        // Raycast to get hits
        Vector3 direction = destination - origin;
        int hitCount = Physics.RaycastNonAlloc(origin, direction, _nonAllocHits, Vector3.Distance(origin, destination) + 0.1f);

        // Loop through all the hits added by the above raycast & sort them by distance from origin
        for (int i = 0; i < hitCount - 1; i++)
        {
            if (i == _nonAllocHits.Length)
                break;

            // Get this and next hit
            RaycastHit thisHit = _nonAllocHits[i];
            RaycastHit nextHit = _nonAllocHits[i + 1];

            // Check if the two hits are not ordered by ascending distance
            if (Vector3.Distance(thisHit.point, origin) > Vector3.Distance(nextHit.point, origin))
            {
                // Swap the two hits
                _nonAllocHits.SetValue(nextHit, i);
                _nonAllocHits.SetValue(thisHit, i + 1);

                // Move the iterator back
                i -= 2;
                if (i < 0)
                    i = -1;
            }
        }

        if (hitCount > 0)
            return Utils.GetFirstInstanceMatchingTag(_nonAllocHits, _aimTags);

        return new RaycastHit();
    }

    /// <summary>
    /// Used internally to check if a raycast's first valid hit is the target transform.
    /// </summary>
    private bool CheckRayFirstHitIsTarget(out Vector3 resultPoint, Vector3 origin, Vector3 destination, Transform target)
    {
        _nonAllocHit = GetClosestPointOnRay(origin, destination);
        if (_nonAllocHit.transform)
        {
            resultPoint = _nonAllocHit.point;
            return _nonAllocHit.transform.IsChildOf(target);
        }

        resultPoint = Vector3.zero;
        return false;
    }

    /// <summary>
    /// Attempts to find and return a better aim point by raycasting towards 
    /// offset positions on the under-mouse-transform's bounding box.
    /// </summary>
    private Vector3 SmartAimFindPoint(Transform target)
    {
        // Get the bounds of the transform under the mouse
        Collider parentColliderUnderMouse = target.GetComponent<Collider>();
        if (!parentColliderUnderMouse)
            parentColliderUnderMouse = _mousePointCollider;

        Bounds bounds = parentColliderUnderMouse.GetGroupedBounds();
        Vector3 centerToMaxCorner = (bounds.max - bounds.center);
        
        Vector3 bestPoint = Vector3.zero;
        float distFromBestPoint = float.MaxValue;

        // Get step values
        float x = centerToMaxCorner.x / (float)_smartAimIterations;
        float y = centerToMaxCorner.y / (float)_smartAimIterations;
        float z = centerToMaxCorner.z / (float)_smartAimIterations;

        /* Find directions to ignore. If the character's aim is close to a world direction, the direction can be ignored,
         * as setting the test point deeper into the target object's bounds will increase the number of tests neeeded
         * without producing much more accurate results */
        Vector3 characterToMousePoint = _aimMousePoint - transform.position;
        bool ignoreX = ( !Utils.IsBetween(((int)Vector3.Angle(characterToMousePoint, Vector3.right)),   35, 145 ) );
        bool ignoreY = ( !Utils.IsBetween(((int)Vector3.Angle(characterToMousePoint, Vector3.up)),      35, 145 ) );
        bool ignoreZ = ( !Utils.IsBetween(((int)Vector3.Angle(characterToMousePoint, Vector3.forward)), 35, 145 ) );
        
        bool originChecked = false;
        int i = 1;
        while (i <= _smartAimIterations)
        {
            // Set up vectors for step in each direction
            Vector3 xStep = new Vector3(x * i, 0, 0);
            Vector3 yStep = new Vector3(0, y * i, 0);
            Vector3 zStep = new Vector3(0, 0, z * i);

            // Loop through each axis, applying a translation to the axes with each iteration
            int[] loopValues = new int[] {0, 1, -1};        // This is the order directions will be tested
            Vector3 resultPoint;
            
            int xIter = 0, yIter = 0, zIter = 0;
            while ((ignoreX && xIter == 0) || (!ignoreX && xIter <= 2))
            {
                Vector3 thisXStep = xStep * loopValues[xIter];

                yIter = 0;
                while ((ignoreY && yIter == 0) || (!ignoreY && yIter <= 2))
                {
                    Vector3 thisYStep = yStep * loopValues[yIter];
                    zIter = 0;
                    while ((ignoreZ && zIter == 0) || (!ignoreZ && zIter <= 2))
                    {
                        // Check for case where origin is being checked more than once
                        if (xIter == 0 && yIter == 0 && zIter == 0)
                        {
                            if (originChecked)
                            {
                                zIter++;
                                continue;
                            }
                            else
                                originChecked = true;
                        }

                        Vector3 thisZStep = zStep * loopValues[zIter];

                        // Test ray at this translation
                        Vector3 currentOffset = Vector3.zero;
                        if (!ignoreX)   currentOffset += thisXStep;
                        if (!ignoreY)   currentOffset += thisYStep;
                        if (!ignoreZ)   currentOffset += thisZStep;

                        // Check if this point could potentially be closer than the current best point
                        float distance = Vector3.Distance(_aimMousePoint, bounds.center + currentOffset);
                        if (distance < distFromBestPoint)
                        {
                            Debug.DrawRay(bounds.center + currentOffset, Vector3.up * 0.2f);
                            // Check the ray for a valid hit
                            if (CheckRayFirstHitIsTarget(out resultPoint, _gunOrigin.position, (bounds.center + currentOffset), target))
                            {
                                Debug.DrawRay(bounds.center + currentOffset, Vector3.up);
                                distFromBestPoint = distance;
                                bestPoint = resultPoint;
                            }
                        }

                        zIter++;
                    }

                    yIter++;
                }

                xIter++;
            }

            i++;
        }

        // Check if a point was found
        if (distFromBestPoint != float.MaxValue)
            return bestPoint;

        // If the code reaches this point, no better aim point was found
        return _aimMousePoint;
    }

    void OnDrawGizmosSelected()
    {
        if (enabled && _aimingAtGeometry)
        {
            Gizmos.color = Color.red;
            if (_gunOrigin)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(_gunOrigin.position, _aimMousePoint);
                Gizmos.color = Color.red;
                Gizmos.DrawLine(_gunOrigin.position, _aimPoint);
            }
            else
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, _aimMousePoint);
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, _aimPoint);
            }

            Bounds b = _mousePointCollider.GetGroupedBounds();
            Gizmos.DrawWireCube(b.center, b.size);

            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(_aimPoint, 0.21f);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(_aimMousePoint, 0.19f);
        }
    }
}
