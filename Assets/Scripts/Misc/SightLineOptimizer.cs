using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JakePerry
{
    /* Contains functions which may be useful for third person type games, where 
     * the point the player is aiming at may not necessarily be visible to the
     * player-character or their gun. Checking for an optimized aim-point may
     * prevent scenarios where the character shoots a wall in front of them rather
     * than the enemy under the player's mouse pointer, etc. */
    public static class SightLineOptimizer
    {
        #region NON-ALLOC Variables
        // Variables declared once only to prevent unnecessary memory allocation

        private readonly static string[] _emptyStringArray = new string[0];
        private static Ray _nonAllocRay = new Ray();
        private static RaycastHit[] _nonAllocHits = new RaycastHit[16];
        private static RaycastHit _nonAllocHit;

        #endregion

        #region Memory Allocation Variables

        private static bool _showMemoryWarning = true;
        public static bool showMemoryAllocationWarning
        {
            get { return _showMemoryWarning; }
            set { _showMemoryWarning = value; }
        }
        
        /// <summary>
        /// <para>
        /// Get: Returns the number of RaycastHit objects currently alloced in memory for use 
        /// in SightLineOptimizer functions. Default: 16
        /// </para>
        /// <para>
        /// Set: Sets the allocated array to an array of specified length.
        /// </para>
        /// </summary>
        public static int allocatedRaycastHits
        {
            get { return _nonAllocHits.Length; }
            set
            {
                if (value != _nonAllocHits.Length)
                {
                    if (value > 1)
                    {
                        // Warning check
                        if (value > 50 && _showMemoryWarning)
                            Debug.LogWarning("Warning: Allocating memory for " + value.ToString() + " RaycastHit objects. For performance reasons, this value should be kept to a minimum. \nOnly allocate a high number if the scene features complex geometry.");

                        _nonAllocHits = new RaycastHit[value];
                    }
                }
            }
        }

        #endregion

        /// <summary>
        /// Calculates an optimal point on the target which is visible from a given position.
        /// </summary>
        /// <param name="hit">
        /// Out parameter for the resulting RaycastHit. Use resulting value only if 
        /// function returns true.
        /// </param>
        /// <param name="sightPosition">The position to check for line of signt from.</param>
        /// /// <param name="targetPoint">
        /// World-space point to look at.
        /// <para>
        /// This is the first position tested for line of sight via raycast.
        /// If the point is not visible, potential optimal points will be tested per their distance
        /// from this point (Close points will be prioritized).
        /// </para>
        /// </param>
        /// <param name="target">The collider to find an optimal viewable point on.</param>
        /// <param name="includeChildren">Should children of the target collider be considered valid?</param>
        /// <param name="iterations">
        /// The number of points the target's bounds will be divided up by on each side of the origin.
        /// <para>
        /// Increasing this value directly affects the maximum number of raycast checks performed
        /// by this function in the order of:
        /// N = (2i + 1)^2
        /// </para>
        /// </param>
        /// <param name="maxAllowedAngle">
        /// The maximum allowed angle between ray from sightPosition to targetPoint
        /// and ray from sightPosition to potential-optimal-point.
        /// <para>Use value less or equal 0 to allow any angle.</para>
        /// </param>
        /// <returns>Returns true if a visible point was found, else returns false.</returns>
        public static bool FindOptimalViewablePoint(out RaycastHit hit, Vector3 sightPosition, Vector3 targetPoint, Collider target, 
            bool includeChildren, uint iterations, float maxAllowedAngle = 0.0f)
        {
            return FindOptimalViewablePoint(out hit, sightPosition, targetPoint, target, includeChildren, 
                iterations, Physics.AllLayers, maxAllowedAngle);
        }

        /// <summary>
        /// Calculates an optimal point on the target which is visible from a given position.
        /// </summary>
        /// <param name="hit">
        /// Out parameter for the resulting RaycastHit. Use resulting value only if 
        /// function returns true.
        /// </param>
        /// <param name="sightPosition">The position to check for line of signt from.</param>
        /// /// <param name="targetPoint">
        /// World-space point to look at.
        /// <para>
        /// This is the first position tested for line of sight via raycast.
        /// If the point is not visible, potential optimal points will be tested per their distance
        /// from this point (Close points will be prioritized).
        /// </para>
        /// </param>
        /// <param name="target">The collider to find an optimal viewable point on.</param>
        /// <param name="includeChildren">Should children of the target collider be considered valid?</param>
        /// <param name="iterations">
        /// The number of points the target's bounds will be divided up by on each side of the origin.
        /// <para>
        /// Increasing this value directly affects the maximum number of raycast checks performed
        /// by this function in the order of:
        /// N = (2i + 1)^2
        /// </para>
        /// </param>
        /// <param name="layerMask">Layermask to be applied to all raycasts performed.</param>
        /// <param name="maxAllowedAngle">
        /// The maximum allowed angle between ray from sightPosition to targetPoint
        /// and ray from sightPosition to potential-optimal-point.
        /// <para>Use value less or equal 0 to allow any angle.</para>
        /// </param>
        /// <returns>Returns true if a visible point was found, else returns false.</returns>
        public static bool FindOptimalViewablePoint(out RaycastHit hit, Vector3 sightPosition, Vector3 targetPoint, Collider target, 
            bool includeChildren, uint iterations, LayerMask layerMask, float maxAllowedAngle = 0.0f)
        {
            return FindOptimalViewablePoint(out hit, sightPosition, targetPoint, target, includeChildren, 
                iterations, layerMask, _emptyStringArray, maxAllowedAngle);
        }

        /// <summary>
        /// Calculates an optimal point on the target which is visible from a given position.
        /// </summary>
        /// <param name="hit">
        /// Out parameter for the resulting RaycastHit. Use resulting value only if 
        /// function returns true.
        /// </param>
        /// <param name="sightPosition">The position to check for line of signt from.</param>
        /// /// <param name="targetPoint">
        /// World-space point to look at.
        /// <para>
        /// This is the first position tested for line of sight via raycast.
        /// If the point is not visible, potential optimal points will be tested per their distance
        /// from this point (Close points will be prioritized).
        /// </para>
        /// </param>
        /// <param name="target">The collider to find an optimal viewable point on.</param>
        /// <param name="includeChildren">Should children of the target collider be considered valid?</param>
        /// <param name="iterations">
        /// The number of points the target's bounds will be divided up by on each side of the origin.
        /// <para>
        /// Increasing this value directly affects the maximum number of raycast checks performed
        /// by this function in the order of:
        /// N = (2i + 1)^2
        /// </para>
        /// </param>
        /// <param name="ignoreTags">
        /// An array of tags to be ignored. Any raycast will be ignored if its transform's 
        /// tag is found in this array.
        /// </param>
        /// <param name="maxAllowedAngle">
        /// The maximum allowed angle between ray from sightPosition to targetPoint
        /// and ray from sightPosition to potential-optimal-point.
        /// <para>Use value less or equal 0 to allow any angle.</para>
        /// </param>
        /// <returns>Returns true if a visible point was found, else returns false.</returns>
        public static bool FindOptimalViewablePoint(out RaycastHit hit, Vector3 sightPosition, Vector3 targetPoint, Collider target, 
            bool includeChildren, uint iterations, string[] ignoreTags, float maxAllowedAngle = 0.0f)
        {
            return FindOptimalViewablePoint(out hit, sightPosition, targetPoint, target, includeChildren, 
                iterations, Physics.AllLayers, ignoreTags, maxAllowedAngle);
        }

        /// <summary>
        /// Calculates an optimal point on the target which is visible from a given position.
        /// </summary>
        /// <param name="hit">
        /// Out parameter for the resulting RaycastHit. Use resulting value only if 
        /// function returns true.
        /// </param>
        /// <param name="sightPosition">The position to check for line of signt from.</param>
        /// /// <param name="targetPoint">
        /// World-space point to look at.
        /// <para>
        /// This is the first position tested for line of sight via raycast.
        /// If the point is not visible, potential optimal points will be tested per their distance
        /// from this point (Close points will be prioritized).
        /// </para>
        /// </param>
        /// <param name="target">The collider to find an optimal viewable point on.</param>
        /// <param name="includeChildren">Should children of the target collider be considered valid?</param>
        /// <param name="iterations">
        /// The number of points the target's bounds will be divided up by on each side of the origin.
        /// <para>
        /// Increasing this value directly affects the maximum number of raycast checks performed
        /// by this function in the order of:
        /// N = (2i + 1)^2
        /// </para>
        /// </param>
        /// /// <param name="layerMask">Layermask to be applied to all raycasts performed.</param>
        /// <param name="ignoreTags">
        /// An array of tags to be ignored. Any raycast will be ignored if its transform's 
        /// tag is found in this array.
        /// </param>
        /// <param name="maxAllowedAngle">
        /// The maximum allowed angle between ray from sightPosition to targetPoint
        /// and ray from sightPosition to potential-optimal-point.
        /// <para>Use value less or equal 0 to allow any angle.</para>
        /// </param>
        /// <returns>Returns true if a visible point was found, else returns false.</returns>
        public static bool FindOptimalViewablePoint(out RaycastHit hit, Vector3 sightPosition, Vector3 targetPoint, Collider target, 
            bool includeChildren, uint iterations, LayerMask layerMask, string[] ignoreTags, float maxAllowedAngle = 0.0f)
        {
            if (!target)
            {
                hit = default(RaycastHit);
                return false;
            }

            if (ignoreTags.Contains(target.transform.tag))
            {
                /* The specified collider is ignored by smart-aim. 
                 * Find point on ray nearest to targetPoint. */
                _nonAllocHit = ClosestPointOnRay(sightPosition, targetPoint, ignoreTags);
                
                if (_nonAllocHit.transform)
                {
                    hit = _nonAllocHit;
                    return true;
                }

                hit = default(RaycastHit);
                return false;
            }

            /* The specified collider is not ignored by smart-aim.
             * Check if the ray from sightPosition - targetPoint hits a valid
             * visible point. */

            _nonAllocRay.origin = sightPosition;
            _nonAllocRay.direction = targetPoint - sightPosition;

            int hits = Physics.RaycastNonAlloc(_nonAllocRay, _nonAllocHits, Vector3.Distance(sightPosition, targetPoint));

            if (hits > 0)
            {
                SortNonAllocArrayByDistance(ref _nonAllocHits, hits, sightPosition);
                if (FirstValidHit(out _nonAllocHit, _nonAllocHits, hits, ignoreTags))
                {
                    /* Ray to targetPoint has a valid hit. Check if hit collider is target or child of target */
                    Transform hitTransform = _nonAllocHit.transform;
                    Transform targetTransform = target.transform;

                    if (hitTransform.IsChildOf(targetTransform))
                    {
                        hit = _nonAllocHit;
                        return true;
                    }
                }

                /* SightPosition cannot see the targetPoint. Use smart-aim iteration to find an optimal point */
                return SmartAimIteration(out hit, sightPosition, target, includeChildren, 
                    iterations, layerMask, targetPoint, maxAllowedAngle);
            }
            else
            {
                hit = default(RaycastHit);
                return false;
            }
        }
        
        private static bool SmartAimIteration(out RaycastHit hit, Vector3 sightPosition, Collider target, bool includeChildren,
            uint iterations, LayerMask layerMask, Vector3 targetPoint, float maxAllowedAngle = 0.0f)
        {
            // Get bounds of target collider (& children colliders if includeChildren)
            Bounds bounds = (includeChildren) ? EncapsulatedChildren(target): target.bounds;


            /* Maybe make a class which converts to 2d projection of AABB.
             * depth modifier to make sure it's alligned with the far side of the BB.
             * 
             *  */
        }

        private static RaycastHit ClosestPointOnRay(Vector3 origin, Vector3 destination)
        {
            return ClosestPointOnRay(origin, destination, _emptyStringArray);
        }

        private static RaycastHit ClosestPointOnRay(Vector3 origin, Vector3 destination, string[] ignoreTags)
        {
            Vector3 direction = destination - origin;
            _nonAllocRay.origin = origin;
            _nonAllocRay.direction = direction;

            // Cast ray & get number of hit objects
            int hitCount = Physics.RaycastNonAlloc(_nonAllocRay, _nonAllocHits, Vector3.Distance(origin, direction));

            // Iterate through all the hits added by this raycast & sort by dist from origin
            SortNonAllocArrayByDistance(ref _nonAllocHits, hitCount, origin);

            if (hitCount > 0)
            {
                if (ignoreTags.Length == 0)
                    return _nonAllocHits[0];    // No tags to ignore, return closest hit

                // Iterate through all hits from this raycast & return first instance without an ignored tag
                for (int i = 0; i < hitCount; i++)
                {
                    RaycastHit thisHit = _nonAllocHits[i];
                    if (!ignoreTags.Contains(thisHit.transform.tag))
                        return thisHit;
                }
            }

            // Nothing was hit by this ray
            return default(RaycastHit);
        }

        /// <summary>
        /// Sorts an array of RaycastHit by distance from point.
        /// </summary>
        /// <param name="array">Reference to the array to sort</param>
        /// <param name="hits">The number of elements from the beginning of the array to sort</param>
        /// <param name="point">Point to sort distance from</param>
        private static void SortNonAllocArrayByDistance(ref RaycastHit[] array, int hits, Vector3 point)
        {
            if (array.Length < 2) return;

            for (int i = 0; i < hits; i++)
            {
                if (i == array.Length) break;

                // Get this hit & next hit
                RaycastHit thisHit = array[i];
                RaycastHit nextHit = array[i + 1];

                // Compare hits, order by ascending distance
                if (Vector3.Distance(thisHit.point, point) > Vector3.Distance(nextHit.point, point))
                {
                    array.SetValue(nextHit, i);
                    array.SetValue(thisHit, i + 1);

                    // De-increment iterator
                    i = (i < 2) ? 0 : i - 2;
                }
            }
        }

        /// <summary>
        /// Iterates through a specified array to find the first valid hit.
        /// </summary>
        /// <param name="hit">
        /// Out parameter for the resulting RaycastHit. Use resulting value only if 
        /// function returns true.
        /// </param>
        /// <param name="array">Reference to the array to search</param>
        /// <param name="hits">The number of elements from the beginning of the array to check</param>
        /// <param name="ignoreTags">
        /// An array of tags to be ignored. Any RaycastHit in the provided
        /// array will be ignored if its tag is found in this array.
        /// </param>
        /// <returns>Returns true if a valid hit was found.</returns>
        private static bool FirstValidHit(out RaycastHit hit, RaycastHit[] array, int hits, string[] ignoreTags)
        {
            for (int i = 0; i < hits; i++)
            {
                if (!ignoreTags.Contains(array[i].transform.tag))
                {
                    hit = array[i];
                    return true;
                }
            }

            hit = default(RaycastHit);
            return false;
        }

        private static Bounds EncapsulatedChildren(Collider col)
        {
            Transform t = col.transform;
            Bounds colBounds = col.bounds;

            // Get an array of all colliders on the object & child objects
            Collider[] colliders = t.GetComponentsInChildren<Collider>(false);

            // Iterate through each collider and encapsulate its bounds
            foreach (Collider c in colliders)
                colBounds.Encapsulate(c.bounds);

            return colBounds;
        }
    }

    /*  */
    public class BoundingBoxRaycaster
    {
        #region Member Variables

        private Bounds? _boundsRef = null;
        public Bounds? bounds
        {
            get { return _boundsRef; }
            set
            {
                if (_boundsRef != value)
                {
                    _boundsRef = value;
                    UpdateCorners();
                }
            }
        }

        private readonly Vector3[] _corners = new Vector3[8];

        private Vector3 _bottomLeft;
        private Vector3 _bottomRight;
        private Vector3 _topLeft;
        private Vector3 _topRight;

        private Vector3 _viewPoint;
        public Vector3 viewingPoint
        {
            get { return _viewPoint; }
            set
            {
                if (_viewPoint != value)
                {
                    _viewPoint = value;
                    EvaluateBounds();
                }
            }
        }

        #endregion

        #region Member Functions

        public BoundingBoxRaycaster(Bounds b, Vector3 viewingPoint)
        {
            _boundsRef = b;
            _viewPoint = viewingPoint;
        }

        private void UpdateCorners()
        {
            if (_boundsRef.HasValue)
            {
                Vector3 bCenter = _boundsRef.Value.center;
                Vector3 bExtents = _boundsRef.Value.extents;

                _corners[0] = bCenter + bExtents;
                _corners[0] = bCenter - bExtents;
                _corners[0] = new Vector3(bCenter.x + bExtents.x, bCenter.y + bExtents.y, bCenter.z - bExtents.z);
                _corners[0] = new Vector3(bCenter.x + bExtents.x, bCenter.y - bExtents.y, bCenter.z + bExtents.z);
                _corners[0] = new Vector3(bCenter.x - bExtents.x, bCenter.y + bExtents.y, bCenter.z + bExtents.z);
                _corners[0] = new Vector3(bCenter.x - bExtents.x, bCenter.y - bExtents.y, bCenter.z + bExtents.z);
                _corners[0] = new Vector3(bCenter.x - bExtents.x, bCenter.y + bExtents.y, bCenter.z - bExtents.z);
                _corners[0] = new Vector3(bCenter.x + bExtents.x, bCenter.y - bExtents.y, bCenter.z - bExtents.z);
            }
        }

        private void EvaluateBounds()
        {
            if (_boundsRef.HasValue)
            {
                
            }
        }

        #endregion
    }
}
