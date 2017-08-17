using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using JakePerry;

public class TrackSquadMember : MonoBehaviour
{
    NavMeshAgent _agent = null;
    uaiAgent _aiAgent = null;

    [Range(2, 5), SerializeField]   private float _meleeRange = 2;
    private SquaddieController closest = null;

	void Start ()
    {
        // Get the reference to the agent on this gameobject
        _agent = GetComponentInParent<NavMeshAgent>();
        _aiAgent = GetComponentInParent<uaiAgent>();
	}

    void Update()
    {
        // Find closest squaddie

        // Get all squad members
        List<SquaddieController> squadMembers = stSquadManager.GetSquadMembers;

        // Find closest squad member
        float minDistance = float.MaxValue;
        foreach (var member in squadMembers)
        {
            // Find distance between this transform & the member's transform
            float distance = Vector3.Distance(member.transform.position, transform.position);

            if (distance < minDistance)
                closest = member;
        }

        // Get reference to the range property on the agent
        uaiProperty p = _aiAgent.FindProperty("InMeleeRange");
        if (p != null)
        {
            bool inRange = Vector3.Distance(closest.transform.position, transform.position) < _meleeRange;
            if (inRange)
            {
                p.SetValue(1.0f);
                _agent.destination = _agent.transform.position;
                _agent.isStopped = true;
            }
            else
            {
                p.SetValue(0.0f);
                _agent.isStopped = false;
            }
        }
    }

    public void DoBehaviour()
    {
        // Set _agent's target position to the closest squaddie's position
        if (closest != null)
        {
            _agent.destination = closest.transform.position;
        }
    }
}
