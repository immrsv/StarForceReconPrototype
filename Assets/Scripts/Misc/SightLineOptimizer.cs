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
        private readonly static RaycastHit _defaultRaycastHit = default(RaycastHit);

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
        /// /// <param name="validTags">
        /// An array of tags which are considered as valid hits.
        /// Any raycasts performed will only consider hits with a contained tag as valid.
        /// </param>
        /// <param name="maxAllowedAngle">
        /// The maximum allowed angle between ray from sightPosition to targetPoint
        /// and ray from sightPosition to potential-optimal-point.
        /// <para>Use value less or equal 0 to allow any angle.</para>
        /// </param>
        /// <returns>Returns true if a visible point was found, else returns false.</returns>
        public static bool FindOptimalViewablePoint(out RaycastHit hit, Vector3 sightPosition, Vector3 targetPoint, Collider target, 
            bool includeChildren, uint iterations, string[] validTags, float maxAllowedAngle = 0.0f)
        {
            return FindOptimalViewablePoint(out hit, sightPosition, targetPoint, target, includeChildren, 
                iterations, validTags, Physics.AllLayers, maxAllowedAngle);
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
        /// /// <param name="validTags">
        /// An array of tags which are considered as valid hits.
        /// Any raycasts performed will only consider hits with a contained tag as valid.
        /// </param>
        /// <param name="layerMask">Layermask to be applied to all raycasts performed.</param>
        /// <param name="maxAllowedAngle">
        /// The maximum allowed angle between ray from sightPosition to targetPoint
        /// and ray from sightPosition to potential-optimal-point.
        /// <para>Use value less or equal 0 to allow any angle.</para>
        /// </param>
        /// <returns>Returns true if a visible point was found, else returns false.</returns>
        public static bool FindOptimalViewablePoint(out RaycastHit hit, Vector3 sightPosition, Vector3 targetPoint, Collider target, 
            bool includeChildren, uint iterations, string[] validTags, LayerMask layerMask, float maxAllowedAngle = 0.0f)
        {
            return FindOptimalViewablePoint(out hit, sightPosition, targetPoint, target, includeChildren, 
                iterations, validTags, layerMask, _emptyStringArray, maxAllowedAngle);
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
        /// <param name="validTags">
        /// An array of tags which are considered as valid hits.
        /// Any raycasts performed will only consider hits with a contained tag as valid.
        /// </param>
        /// <param name="smartAimIgnoreTags">
        /// If the target collider's tag is found in this list, smart-aim 
        /// functionality will not be used.
        /// </param>
        /// <param name="maxAllowedAngle">
        /// The maximum allowed angle between ray from sightPosition to targetPoint
        /// and ray from sightPosition to potential-optimal-point.
        /// <para>Use value less or equal 0 to allow any angle.</para>
        /// </param>
        /// <returns>Returns true if a visible point was found, else returns false.</returns>
        public static bool FindOptimalViewablePoint(out RaycastHit hit, Vector3 sightPosition, Vector3 targetPoint, Collider target, 
            bool includeChildren, uint iterations, string[] validTags, string[] smartAimIgnoreTags, float maxAllowedAngle = 0.0f)
        {
            return FindOptimalViewablePoint(out hit, sightPosition, targetPoint, target, includeChildren, 
                iterations, validTags, Physics.AllLayers, smartAimIgnoreTags, maxAllowedAngle);
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
        /// /// <param name="validTags">
        /// An array of tags which are considered as valid hits.
        /// Any raycasts performed will only consider hits with a contained tag as valid.
        /// </param>
        /// /// <param name="layerMask">Layermask to be applied to all raycasts performed.</param>
        /// <param name="smartAimIgnoreTags">
        /// If the target collider's tag is found in this list, smart-aim 
        /// functionality will not be used.
        /// </param>
        /// <param name="maxAllowedAngle">
        /// The maximum allowed angle between ray from sightPosition to targetPoint
        /// and ray from sightPosition to potential-optimal-point.
        /// <para>Use value less or equal 0 to allow any angle.</para>
        /// </param>
        /// <returns>Returns true if a visible point was found, else returns false.</returns>
        public static bool FindOptimalViewablePoint(out RaycastHit hit, Vector3 sightPosition, Vector3 targetPoint, Collider target, 
            bool includeChildren, uint iterations, string[] validTags, LayerMask layerMask, string[] smartAimIgnoreTags, float maxAllowedAngle = 0.0f)
        {
            if (!target)
            {
                hit = _defaultRaycastHit;
                return false;
            }

            if (smartAimIgnoreTags.Contains(target.transform.tag))
            {
                /* The specified collider is ignored by smart-aim. 
                 * Find point on ray nearest to targetPoint. */
                _nonAllocHit = ClosestPointOnRay(sightPosition, targetPoint, validTags);
                
                if (_nonAllocHit.transform)
                {
                    hit = _nonAllocHit;
                    return true;
                }

                hit = _defaultRaycastHit;
                return false;
            }

            /* The specified collider is not ignored by smart-aim.
             * Check if the ray from sightPosition - targetPoint hits a valid
             * visible point. */
            
            _nonAllocRay.origin = sightPosition;
            _nonAllocRay.direction = targetPoint - sightPosition;
            Debug.DrawRay(sightPosition, Vector3.up * 5);
            Debug.DrawRay(sightPosition, _nonAllocRay.direction * 10);

            int hits = Physics.RaycastNonAlloc(_nonAllocRay, _nonAllocHits, Vector3.Distance(sightPosition, targetPoint) + 1.0f);

            if (hits > 0)
            {
                SortNonAllocArrayByDistance(ref _nonAllocHits, hits, sightPosition);
                if (FirstValidHit(out _nonAllocHit, _nonAllocHits, hits, validTags))
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
                return SmartAimIteration(out hit, sightPosition, targetPoint, target, 
                    includeChildren, iterations, validTags, layerMask, maxAllowedAngle);
            }
            else
            {
                hit = _defaultRaycastHit;
                return false;
            }
        }
        
        private static bool SmartAimIteration(out RaycastHit hit, Vector3 sightPosition, Vector3 targetPoint, Collider target, 
            bool includeChildren, uint iterations, string[] validTags, LayerMask layerMask, float maxAllowedAngle = 0.0f)
        {
            // Get bounds of target collider (& children colliders if includeChildren)
            Bounds bounds = (includeChildren) ? SightLineUtilityFunctions.EncapsulatedChildren(target): target.bounds;
            Vector3 centerToMaxCorner = (bounds.max - bounds.center);

            RaycastHit bestPoint = default(RaycastHit);
            float distFromBestPoint = float.MaxValue;

            // Get step values
            float x = centerToMaxCorner.x / (float)iterations;
            float y = centerToMaxCorner.y / (float)iterations;
            float z = centerToMaxCorner.z / (float)iterations;

            /* Find directions to ignore. If the sight direction is close to a world direction, the direction can be ignored,
             * as setting the test point deeper into the target object's bounds will increase the number of tests neeeded
             * without producing much more accurate results */
            Vector3 sightToTarget = sightPosition - target.transform.position;
            bool ignoreX = (!SightLineUtilityFunctions.Between
                (((int)Vector3.Angle(sightToTarget, Vector3.right)), 45, 135));

            bool ignoreY = (!SightLineUtilityFunctions.Between
                (((int)Vector3.Angle(sightToTarget, Vector3.up)), 45, 135));

            bool ignoreZ = (!SightLineUtilityFunctions.Between
                (((int)Vector3.Angle(sightToTarget, Vector3.forward)), 45, 135));

            _nonAllocRay.origin = sightPosition;
            bool originChecked = false;
            int i = 1;
            while (i <= iterations)
            {
                // Set up vectors for step in each direction
                Vector3 xStep = new Vector3(x * i, 0, 0);
                Vector3 yStep = new Vector3(0, y * i, 0);
                Vector3 zStep = new Vector3(0, 0, z * i);

                // Loop through each axis, applying a translation to the axes with each iteration
                int[] loopValues = new int[] { 0, 1, -1 };        // This is the order directions will be tested

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
                            if (!ignoreX) currentOffset += thisXStep;
                            if (!ignoreY) currentOffset += thisYStep;
                            if (!ignoreZ) currentOffset += thisZStep;

                            Vector3 currentPoint = bounds.center + currentOffset;

                            // Check if angle is valid
                            if (Vector3.Angle((currentPoint - sightPosition), (targetPoint - sightPosition)) <= maxAllowedAngle
                                || maxAllowedAngle <= 0)
                            {
                                // Check if this point could potentially be closer than the current best point
                                float distance = Vector3.Distance(targetPoint, currentPoint);
                                if (distance < distFromBestPoint)
                                {
                                    // Raycast to this potential point
                                    _nonAllocRay.direction = (currentPoint - sightPosition);
                                    int hits = Physics.RaycastNonAlloc(_nonAllocRay, _nonAllocHits,
                                        Vector3.Distance(sightPosition, currentPoint) + 1.0f, layerMask);

                                    SortNonAllocArrayByDistance(ref _nonAllocHits, hits, sightPosition);

                                    // Find the first valid hit
                                    if (FirstValidHit(out _nonAllocHit, _nonAllocHits, hits, validTags))
                                    {
                                        // Check if the valid hit was part of the target collider
                                        bool isPartOfTarget = (includeChildren) ?
                                            (_nonAllocHit.transform.IsChildOf(target.transform)) 
                                            : (_nonAllocHit.transform == target.transform);

                                        if (isPartOfTarget)
                                        {
                                            distFromBestPoint = distance;
                                            bestPoint = _nonAllocHit;
                                        }
                                    }
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
            {
                hit = bestPoint;
                return true;
            }

            /* No optimal aim point was found.
             * Return first valid hit on ray to targetPoint */

            _nonAllocHit = ClosestPointOnRay(sightPosition, targetPoint, validTags);
            if (_nonAllocHit.transform)
            {
                hit = _nonAllocHit;
                return true;
            }

            hit = _defaultRaycastHit;
            return false;
        }

        private static RaycastHit ClosestPointOnRay(Vector3 origin, Vector3 destination)
        {
            return ClosestPointOnRay(origin, destination, _emptyStringArray);
        }

        private static RaycastHit ClosestPointOnRay(Vector3 origin, Vector3 destination, string[] validTags)
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
                if (validTags.Length == 0)
                    return _nonAllocHits[0];    // Every tag is valid, return closest hit

                // Iterate through all hits from this raycast & return first instance without an ignored tag
                for (int i = 0; i < hitCount; i++)
                {
                    RaycastHit thisHit = _nonAllocHits[i];
                    if (validTags.Contains(thisHit.transform.tag))
                        return thisHit;
                }
            }

            // Nothing was hit by this ray
            return _defaultRaycastHit;
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

            for (int i = 0; i < hits - 1; i++)
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
        /// <param name="validTags">An array of tags which are considered as valid hits.</param>
        /// <returns>Returns true if a valid hit was found.</returns>
        private static bool FirstValidHit(out RaycastHit hit, RaycastHit[] array, int hits, string[] validTags)
        {
            for (int i = 0; i < hits; i++)
            {
                if (validTags.Length == 0)
                {
                    hit = array[i];
                    return true;
                }

                if (validTags.Contains(array[i].transform.tag))
                {
                    hit = array[i];
                    return true;
                }
            }

            hit = _defaultRaycastHit;
            return false;
        }
    }

    /* A collection of helper functions used within  */
    public static class SightLineUtilityFunctions
    {
        /// <summary>
        /// Returns bounding box encapsulating the specified collider & all child colliders.
        /// </summary>
        public static Bounds EncapsulatedChildren(Collider col)
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

        /// <summary>
        /// Returns true if the specified value falls between minimum (inclusive) 
        /// & maximum (inclusive).
        /// </summary>
        public static bool Between(float value, float minimum, float maximum)
        {
            return (value >= minimum && value <= maximum);
        }

        /// <summary>
        /// Returns true if the specified value falls between minimum (inclusive) 
        /// & maximum (exclusive).
        /// </summary>
        public static bool Between(int value, int minimum, int maximum)
        {
            return (value >= minimum && value < maximum);
        }
    }
}
