using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JakePerry
{
    /* A collection of useful functions */
    public static class Utils
    {
        /// <summary>
        /// Determines whether or not the specified value is between the min and max values provided
        /// </summary>
        public static bool IsBetween(float val, float min, float max, bool includeMinValue = true, bool includeMaxValue = true)
        {
            return (
                    (val < max || (val == max && includeMaxValue))
                &&  (val > min || (val == min && includeMinValue))
                );
        }

        /// <summary>
        /// Applies a tag mask in the form of a string array to the specified collection.
        /// An array is returned containing only objects with a transform tag that
        /// is contained in the tagMask.
        /// </summary>
        public static Transform[] ApplyTagMask(Transform[] collection, string[] tagMask)
        {
            if (tagMask.Length == 0)
                return collection;

            List<Transform> collectionClone = new List<Transform>(collection);

            // Loop through each object in the clone list
            int i = 0;
            while (i < collectionClone.Count)
            {
                Transform t = collectionClone[i];

                bool match = false;
                // Loop through each string in the tagMask array
                foreach (string s in tagMask)
                {
                    if (t.tag == s)
                    {
                        match = true;
                        break;
                    }
                }

                // Remove element if there was no match, else increment i
                if (!match)
                    collectionClone.RemoveAt(i);
                else
                    i++;
            }

            return collectionClone.ToArray();
        }

        /// <summary>
        /// Applies a tag mask in the form of a string array to the specified collection.
        /// An array is returned containing only objects with a transform tag that
        /// is contained in the tagMask.
        /// </summary>
        public static RaycastHit[] ApplyTagMask(RaycastHit[] collection, string[] tagMask)
        {
            if (tagMask.Length == 0)
                return collection;

            List<RaycastHit> collectionClone = new List<RaycastHit>(collection);

            // Loop through each object in the clone list
            int i = 0;
            while (i < collectionClone.Count)
            {
                Transform t = collectionClone[i].transform;

                bool match = false;
                // Loop through each string in the tagMask array
                foreach (string s in tagMask)
                {
                    if (t.tag == s)
                    {
                        match = true;
                        break;
                    }
                }

                // Remove element if there was no match, else increment i
                if (!match)
                    collectionClone.RemoveAt(i);
                else
                    i++;
            }

            return collectionClone.ToArray();
        }

        /// <summary>
        /// Applies a tag mask in the form of a string array to the specified collection.
        /// An array is returned containing only objects with a transform tag that
        /// is contained in the tagMask.
        /// </summary>
        public static Collider[] ApplyTagMask(Collider[] collection, string[] tagMask)
        {
            if (tagMask.Length == 0)
                return collection;

            List<Collider> collectionClone = new List<Collider>(collection);

            // Loop through each object in the clone list
            int i = 0;
            while (i < collectionClone.Count)
            {
                Transform t = collectionClone[i].transform;

                bool match = false;
                // Loop through each string in the tagMask array
                foreach (string s in tagMask)
                {
                    if (t.tag == s)
                    {
                        match = true;
                        break;
                    }
                }

                // Remove element if there was no match, else increment i
                if (!match)
                    collectionClone.RemoveAt(i);
                else
                    i++;
            }

            return collectionClone.ToArray();
        }

        /// <summary>
        /// Filters the specified collection and returns an array of only instances
        /// which belong to the specified transform's hierarchy
        /// </summary>
        public static Transform[] GetChildrenOf(Transform parent, Transform[] collection)
        {
            List<Transform> collectionClone = new List<Transform>(collection);

            // Loop through each object in the clone list
            int i = 0;
            while (i < collectionClone.Count)
            {
                Transform t = collectionClone[i];

                bool match = (t.IsChildOf(parent));

                // Remove element if there was no match, else increment i
                if (!match)
                    collectionClone.RemoveAt(i);
                else
                    i++;
            }

            return collectionClone.ToArray();
        }

        /// <summary>
        /// Filters the specified collection and returns an array of only instances
        /// which belong to the specified transform's hierarchy
        /// </summary>
        public static RaycastHit[] GetChildrenOf(Transform parent, RaycastHit[] collection)
        {
            List<RaycastHit> collectionClone = new List<RaycastHit>(collection);

            // Loop through each object in the clone list
            int i = 0;
            while (i < collectionClone.Count)
            {
                Transform t = collectionClone[i].transform;

                bool match = (t.IsChildOf(parent));

                // Remove element if there was no match, else increment i
                if (!match)
                    collectionClone.RemoveAt(i);
                else
                    i++;
            }

            return collectionClone.ToArray();
        }

        /// <summary>
        /// Filters the specified collection and returns an array of only instances
        /// which belong to the specified transform's hierarchy
        /// </summary>
        public static Collider[] GetChildrenOf(Transform parent, Collider[] collection)
        {
            List<Collider> collectionClone = new List<Collider>(collection);

            // Loop through each object in the clone list
            int i = 0;
            while (i < collectionClone.Count)
            {
                Transform t = collectionClone[i].transform;

                bool match = (t.IsChildOf(parent));

                // Remove element if there was no match, else increment i
                if (!match)
                    collectionClone.RemoveAt(i);
                else
                    i++;
            }

            return collectionClone.ToArray();
        }

        /// <summary>
        /// Iterates through the specified collection and returns the first instance
        /// with a tag found in tagMask
        /// </summary>
        public static Transform GetFirstInstanceMatchingTag(Transform[] collection, string[] tagMask)
        {
            if (tagMask.Length == 0)
                return null;

            foreach (Transform t in collection)
            {
                foreach (string s in tagMask)
                {
                    if (t.tag == s)
                        return t;
                }
            }

            return null;
        }

        /// <summary>
        /// Iterates through the specified collection and returns the first instance
        /// with a tag found in tagMask
        /// </summary>
        public static RaycastHit GetFirstInstanceMatchingTag(RaycastHit[] collection, string[] tagMask)
        {
            if (tagMask.Length == 0)
                return new RaycastHit();
            
            foreach (RaycastHit t in collection)
            {
                foreach (string s in tagMask)
                {
                    if (t.transform)
                    {
                        if (t.transform.tag == s)
                            return t;
                    }
                }
            }

            return new RaycastHit();
        }

        /// <summary>
        /// Iterates through the specified collection and returns the first instance
        /// with a tag found in tagMask
        /// </summary>
        public static Collider GetFirstInstanceMatchingTag(Collider[] collection, string[] tagMask)
        {
            if (tagMask.Length == 0)
                return null;

            foreach (Collider t in collection)
            {
                foreach (string s in tagMask)
                {
                    if (t.transform.tag == s)
                        return t;
                }
            }

            return null;
        }
    }

    /* A collection of Extension methods */
    public static class Extensions
    {
        /// <summary>
        /// Returns true if all components of this vector are between the components of the
        /// specified min and max vectors. Else, returns false.
        /// </summary>
        public static bool InRange(this Vector2 v, Vector2 min, Vector2 max, bool includeMinValues = true, bool includeMaxValues = true)
        {
            return (Utils.IsBetween(v.x, min.x, max.x, includeMinValues, includeMinValues)
                &&  Utils.IsBetween(v.y, min.y, max.y, includeMinValues, includeMinValues)
                );
        }

        /// <summary>
        /// Returns true if all components of this vector are between the components of the
        /// specified min and max vectors. Else, returns false.
        /// </summary>
        public static bool InRange(this Vector3 v, Vector3 min, Vector3 max, bool includeMinValues = true, bool includeMaxValues = true)
        {
            return (Utils.IsBetween(v.x, min.x, max.x, includeMinValues, includeMinValues)
                &&  Utils.IsBetween(v.y, min.y, max.y, includeMinValues, includeMinValues)
                &&  Utils.IsBetween(v.z, min.z, max.z, includeMinValues, includeMinValues)
                );
        }

        /// <summary>
        /// Returns true if all components of this vector are between the components of the
        /// specified min and max vectors. Else, returns false.
        /// </summary>
        public static bool InRange(this Vector4 v, Vector4 min, Vector4 max, bool includeMinValues = true, bool includeMaxValues = true)
        {
            return (Utils.IsBetween(v.x, min.x, max.x, includeMinValues, includeMinValues)
                &&  Utils.IsBetween(v.y, min.y, max.y, includeMinValues, includeMinValues)
                &&  Utils.IsBetween(v.z, min.z, max.z, includeMinValues, includeMinValues)
                &&  Utils.IsBetween(v.w, min.w, max.w, includeMinValues, includeMinValues)
                );
        }

        /// <summary>
        /// Returns a Vector made up of the smallest x & y components
        /// found in this and other vector
        public static Vector2 GetMinComponents(this Vector2 v, Vector2 other)
        {
            return new Vector2(Mathf.Min(v.x, other.x), Mathf.Min(v.y, other.y));
        }

        /// <summary>
        /// Returns a Vector made up of the smallest x, y & z components
        /// found in this and other vector
        public static Vector3 GetMinComponents(this Vector3 v, Vector3 other)
        {
            return new Vector3(Mathf.Min(v.x, other.x), Mathf.Min(v.y, other.y), Mathf.Min(v.z, other.z));
        }

        /// <summary>
        /// Returns a Vector made up of the smallest x, y, z & w components
        /// found in this and other vector
        public static Vector4 GetMinComponents(this Vector4 v, Vector4 other)
        {
            return new Vector4(Mathf.Min(v.x, other.x), Mathf.Min(v.y, other.y), Mathf.Min(v.z, other.z), Mathf.Min(v.w, other.w));
        }

        /// <summary>
        /// Returns a Vector made up of the largest x & y components
        /// found in this and other vector
        public static Vector2 GetMaxComponents(this Vector2 v, Vector2 other)
        {
            return new Vector2(Mathf.Max(v.x, other.x), Mathf.Max(v.y, other.y));
        }

        /// <summary>
        /// Returns a Vector made up of the smallest x, y & z components
        /// found in this and other vector
        public static Vector3 GetMaxComponents(this Vector3 v, Vector3 other)
        {
            return new Vector3(Mathf.Max(v.x, other.x), Mathf.Max(v.y, other.y), Mathf.Max(v.z, other.z));
        }

        /// <summary>
        /// Returns a Vector made up of the smallest x, y, z & w components
        /// found in this and other vector
        public static Vector4 GetMaxComponents(this Vector4 v, Vector4 other)
        {
            return new Vector4(Mathf.Max(v.x, other.x), Mathf.Max(v.y, other.y), Mathf.Max(v.z, other.z), Mathf.Max(v.w, other.w));
        }

        /// <summary>
        /// Finds and returns the top-level ancestor transform.
        /// </summary>
        public static Transform GetTopLevelParent(this Transform t)
        {
            Transform parent = t;
            while (parent.parent != null)
            {
                parent = parent.parent;
            }

            return parent;
        }

        /// <summary>
        /// Returns bounds for all colliders attached to any child
        /// objects of this collider's transform.
        /// </summary>
        public static Bounds GetGroupedBounds(this Collider col)
        {
            Transform t = col.transform;
            Bounds colBounds = col.bounds;

            // Get an array of all colliders on the object & child objects
            Collider[] colliders = t.GetComponentsInChildren<Collider>(false);

            // Iterate through each collider and encapsulate its bounds
            foreach (Collider c in colliders)
            {
                // Get bounds info
                Bounds b = c.bounds;
                colBounds.Encapsulate(b);
            }

            return colBounds;
        }

        /// <summary>
        /// Sorts the array by distance from the specified position. If reverseOrder is true, 
        /// the furthest RaycastHit will be at the start of the array.
        /// </summary>
        public static void SortByDistance(this RaycastHit[] hits, Vector3 position, bool reverseOrder = false)
        {
            System.Array.Sort(hits, delegate (RaycastHit r1, RaycastHit r2) {
                return Vector3.Distance(position, r1.point).CompareTo(Vector3.Distance(position, r2.point)) * ((reverseOrder) ? -1 : 1);
            });
        }

        /// <summary>
        /// Returns whether or not this array contains the specified object
        /// </summary>
        public static bool Contains<T>(this T[] array, T target)
        {
            foreach (T o in array)
            {
                if (o.Equals(target))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Ascends the heirarchy to find the top parent collider, breaking search
        /// when a parent with a different tag is found.
        /// </summary>
        public static Collider TopParentMatchingTag(this Collider col)
        {
            string tag = col.transform.tag;
            Transform t = col.transform;

            Stack<Transform> stack = new Stack<Transform>();
            stack.Push(t);

            // Ascend heirarchy & track all parent transforms with matching tag
            while (t.parent != null)
            {
                if (t.parent.tag == tag)
                {
                    t = t.parent;
                    stack.Push(t);
                }
                else
                    break;
            }

            // Find first collider in stack
            Collider result = null;
            do
            {
                Transform topOfStack = stack.Pop();
                result = topOfStack.GetComponent<Collider>();

            } while (result == null && stack.Count > 0);

            // Return initial collider if no higher collider was found
            if (result == null)
                return col;

            return result;
        }
    }
}
