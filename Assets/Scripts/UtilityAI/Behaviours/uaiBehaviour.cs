using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JakePerry
{
    /* Struct for holding consideration name and priority curve. */
    [System.Serializable]
    public struct uaiConsideration
    {
        public string _propertyName;
        public bool _enabled;

        [SerializeField]    private AnimationCurve _priority;
        private uaiBaseProp _propertyReference;
        /// <summary>
        /// The property attached to this consideration. 
        /// </summary>
        public uaiBaseProp property
        {
            get { return _propertyReference; }
            set { _propertyReference = value; }
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
        /// Returns the current priority for this consideration.
        /// </summary>
        public float Evaluate()
        {
            if (!_enabled) return 0.0f;
            if (_propertyReference == null) return 0.0f;

            return _priority.Evaluate(_propertyReference.normalizedValue);
        }
    }

    /* Utility AI behaviour script. Behaviours have a list of
     * considerations which determine their priority, and a delegate
     * to an action which is executed when the behaviour is run.
     */
    public class uaiBehaviour : MonoBehaviour
    {
        private uaiAgent _agent;  // Stores a reference to the agent script
        [SerializeField, HideInInspector]    private List<uaiConsideration> _considerations = new List<uaiConsideration>();
        
        [Header("Action Delegates")]
        public UnityEngine.Events.UnityEvent _action;

        void Start()
        {
            // Get a reference to the agent component
            _agent = GetComponentInParent<uaiAgent>();

            // Remove this behaviour if there is no agent tied to the gameobject
            if (!_agent)
            {
                Debug.LogError("No Agent script was found on this object or any parent objects. The behaviour script will be deleted.");
                DestroyImmediate(this);
            }

            // Add this behaviour to the agent
            _agent.AddBehaviour(this);

            // Link all considerations to their correct properties
            LinkConsiderations();
        }

        /// <summary>
        /// Used internally to iterate through each consideration & 
        /// link them to the correct properties.
        /// </summary>
        private void LinkConsiderations()
        {
            if (_agent)
            {
                // Iterate through each consideration
                for (int i = 0; i < _considerations.Count; i++)
                {
                    uaiConsideration c = _considerations[i];

                    // Find the correct property attached to the agent
                    uaiBaseProp property = _agent.FindProperty(c._propertyName);

                    if (property != null)
                    {
                        c.property = property;
                    }
                    else
                    {
                        // No property matching the consideration's name was found
                        Debug.LogWarning("No attached property with name '" + c._propertyName + "' was found. The consideration will not have any weight.");
                        c._propertyName = "";
                    }
                }
            }
        }

        /// <summary>
        /// Sets the action delegate which will be called when the behaviour is executed.
        /// </summary>
        public void SetAction(UnityEngine.Events.UnityEvent actionDelegate)
        {
            _action = actionDelegate;
        }

        /// <summary>
        /// Returns the current priority for this behaviour, based on all attached & enabled considerations.
        /// </summary>
        public float Evaluate()
        {
            float totalPriority = 0.0f;
            int i = 0;
            int considerations = 0;

            // Iterate through each consideration to get total priority
            while (i < _considerations.Count)
            {
                uaiConsideration c = _considerations[i];

                if (c._enabled)
                {
                    totalPriority += c.Evaluate();
                    considerations++;
                }

                i++;
            }

            if (considerations == 0) return 0.0f;

            return totalPriority / considerations;
        }

        /// <summary>
        /// Runs the code attached to this behaviour via delegate.
        /// </summary>
        public void ExecuteAction()
        {
            _action.Invoke();
        }
    }
}
