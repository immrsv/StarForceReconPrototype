using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBrain : MonoBehaviour
{

    [Range(3.0f, 25.0f), SerializeField]    private float _sightRange = 15.0f;
    private List<Health> _visibleEnemies = new List<Health>();

    private Health _target = null;
    public Health target
    {
        get { return _target; }
    }

    public void UpdateView()
    {
        // TODO: Clear & refresh list of visible enemies

        // Get all enemies in radius
        // Raycast for sight
        // Add if visible
    }
}
