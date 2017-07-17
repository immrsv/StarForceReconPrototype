using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JakePerry
{
    public class uaiAgent : MonoBehaviour
    {
        [SerializeField()]  private List<uaiBaseBehaviour> _behaviours;
        private uaiBaseBehaviour _currentBehaviour = null;

        public List<uaiBaseBehaviour> behaviourList
        {
            get { return _behaviours; }
        }

        [Tooltip("When evaluating all behaviours, the current behaviour will be given this much extra priority. \nThe higher this value is, the less likely the agent is to suddenly switch tasks before completing the task.")]
        [Range(0.01f, 0.15f), SerializeField()]  private float _commitmentValue = 0.1f;



        void Start()
        {

        }
        
        void Update()
        {
            // Find the top priority behaviour
            uaiBaseBehaviour topPriority = GetTopPriorityBehaviour();
            if (topPriority != null)
                _currentBehaviour = topPriority;

            if (_currentBehaviour != null)
            {
                // TODO: Execute behaviour
            }
        }

        private uaiBaseBehaviour GetTopPriorityBehaviour()
        {
            uaiBaseBehaviour topPriority = null;
            float topScore = 0.0f;

            // Loop through each behaviour to find highest score
            foreach (uaiBaseBehaviour b in _behaviours)
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
