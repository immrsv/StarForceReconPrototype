using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JakePerry
{
    public class uaiAgent : MonoBehaviour
    {
        private uaiBehaviour _currentBehaviour = null;
        private List<uaiBehaviour> _behaviours = new List<uaiBehaviour>();
        /// <summary>
        /// Returns an array of all behaviours attached to this agent.
        /// </summary>
        public uaiBehaviour[] behaviours
        {
            get { return _behaviours.ToArray(); }
        }

        [SerializeField, HideInInspector]   private List<uaiBaseProp> _properties = new List<uaiBaseProp>();
        /// <summary>
        /// Returns an array of all properties attached to this agent.
        /// </summary>
        public uaiBaseProp[] properties
        {
            get { return _properties.ToArray(); }
        }

        [Tooltip("When evaluating all behaviours, the current behaviour will be given this much extra priority. \nThe higher this value is, the less likely the agent is to suddenly switch tasks before completing the task.")]
        [Range(0.01f, 0.15f), SerializeField()]  private float _commitmentValue = 0.1f;
        
        void Update()
        {
            // Find the top priority behaviour
            uaiBehaviour topPriority = GetTopPriorityBehaviour();
            if (topPriority != null)
                _currentBehaviour = topPriority;

            if (_currentBehaviour != null)
            {
                // TODO: Execute behaviour
            }
        }

        /// <summary>
        /// Add the specified behaviour to the agent.
        /// </summary>
        public void AddBehaviour(uaiBehaviour b)
        {
            if (!_behaviours.Contains(b))
                _behaviours.Add(b);
        }

        /// <summary>
        /// Searches for an attached property with the specified name & returns the
        /// first instance in the list.
        /// </summary>
        public uaiBaseProp FindProperty(string name)
        {
            foreach (uaiBaseProp p in _properties)
            {
                if (p.name == name)
                    return p;
            }

            return null;
        }

        /// <summary>
        /// Finds the behaviour that is currently weighted as the top priority.
        /// </summary>
        private uaiBehaviour GetTopPriorityBehaviour()
        {
            uaiBehaviour topPriority = null;
            float topScore = 0.0f;

            // Loop through each behaviour to find highest score
            foreach (uaiBehaviour b in _behaviours)
            {
                if (!b) continue;

                float commitment = (b == _currentBehaviour) ? _commitmentValue : 0;
                float score = commitment + b.Evaluate();

                if (score > topScore)
                {
                    topScore = score;
                    topPriority = b;
                }
            }

            return topPriority;
        }
    }
}
