using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JakePerry
{
    /* Struct for holding consideration name and priority curve. */
    public struct uaiConsideration
    {
        public string _propertyName;
        public bool _enabled;

        private AnimationCurve _priority;

        // Constructor
        public uaiConsideration(string propertyName)
        {
            _propertyName = propertyName;
            _enabled = true;
            _priority = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 0));
        }

        /// <summary>
        /// Sets the priority values of this consideration to the specified curve.
        /// Please ensure the curve's t-values fall between 0-1.
        /// </summary>
        public void SetPriorityCurve(AnimationCurve curve)
        {
            if (curve != null)
                _priority.keys = curve.keys;
        }

        /// <summary>
        /// Evaluate the priority curve at time.
        /// </summary>
        public float Evaluate(float time)
        {
            return (_enabled) ? _priority.Evaluate(time) : 0.0f;
        }
    }

    /* Base class for utility AI behaviours.
     * Behaviours have a list of consideration properties
     */
    public abstract class uaiBaseBehaviour : MonoBehaviour
    {
        protected uaiConsideration[] _considerations;

        public float Evaluate()
        {
            // TODO: Loop through each consideration and find weight
            return 0.0f;
        }
    }
}
