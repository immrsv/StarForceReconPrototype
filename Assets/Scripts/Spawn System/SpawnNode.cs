using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnNode : MonoBehaviour
{
    #region Member Variables

    #region Zone reference

    [Tooltip("The spawn zone this node belongs to.")]
    [SerializeField, HideInInspector]    private SpawnZone _zone = null;

    [Tooltip("If true and zone is not specified, this node will find the closest Spawn Zone when the scene is loaded.")]
    [SerializeField, HideInInspector]    private bool _getClosestZone = false;

    #endregion

    #region Spawn prefabs

    // Struct used for grouping enemy prefabs with spawn probability
    [System.Serializable]
    private struct SpawnNodeEnemy
    {
        public GameObject _enemy;
        public float _chance;
    }

    [SerializeField, HideInInspector]    private List<SpawnNodeEnemy> _spawnables;

    #endregion

    #endregion

    #region Member Functions

    void Awake ()
    {
        // Auto-find closest zone
        if (_getClosestZone)
        {
            SpawnZone[] zones = FindObjectsOfType<SpawnZone>();
            SpawnZone nearest = null;

            // Find closest zone
            if (zones.Length > 0)
            {
                float smallestDistance = float.MaxValue;
                foreach (SpawnZone z in zones)
                {
                    // Get distance to the zone
                    float dist = Vector3.Distance(z.transform.position, transform.position);

                    if (dist < smallestDistance)
                    {
                        smallestDistance = dist;
                        nearest = z;
                    }
                }
            }

            if (nearest == null)
                Debug.LogError("No Spawn Zones were found. Right click in the scene heirarchy to create a new zone.");

            _zone = nearest;
        }

        // Register this script with the zone, or remove this script if there is no zone specified
        if (_zone)
            _zone.AddNode(this);
        else
        {
            Debug.LogError("SpawnNode script on " + gameObject.name + " is not assigned to a SpawnZone. The script will be destroyed.");
            Destroy(this);
        }
	}
    
    public void SpawnEnemies(int quantity)
    {
        // TODO: Add delegate to health script to call event on death.
        // Subscribe connected zone as a listener which de-increments enemy count
    }

    #endregion
}
