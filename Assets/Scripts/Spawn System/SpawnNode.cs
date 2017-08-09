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

    private GameObject _enemyEmptyParent = null;
    [SerializeField]    private Transform _spawnLocation = null;

    #endregion

    #endregion

    #region Member Functions

    void Awake ()
    {
        // Create empty object to contain all spawned enemies
        _enemyEmptyParent = new GameObject("enemies");
        _enemyEmptyParent.transform.parent = this.transform;

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
            Debug.LogError("SpawnNode is not assigned to a SpawnZone. The node will be destroyed.", this);
            Destroy(this);
        }
	}

    void Start()
    {
        /* NOTE: Components will not receive the OnDestroy method call
         * if they do not have Start, Update, FixedUpdate, etc implemented.
         * 
         * The Start function is only implemented here to ensure the OnDestroy
         * method is called. 
         */
    }
    
    public void SpawnEnemies(int quantity)
    {
        if (_spawnables.Count > 0)
        {
            for (int i = 0; i < quantity; i++)
            {
                // TODO: Use chance to pick an enemy
                GameObject e = _spawnables[0]._enemy;
                
                // TODO: Check if null, remove from list & log error if so

                // Instantiate enemy
                GameObject enemy = Instantiate(e, _enemyEmptyParent.transform);
                if (_spawnLocation)
                    enemy.transform.position = _spawnLocation.position;
                else
                {
                    Debug.LogError("Spawn Node does not have a spawn-location transform set and is unable to spawn enemies. Please ensure the transform is set.");
                    Destroy(enemy);
                    return;
                }

                // Subscribe spawn-zone as listener for enemy's death event
                Health h = enemy.GetComponentInChildren<Health>();
                if (h)
                    _zone.RegisterEnemyToZone(h);
                else
                {
                    Debug.LogError("Spawn Node attempted to spawn a specified prefab which does not have an attached Health script. Please ensure all enemies have an attached Health script, as it is required for Spawn Zone tracking.");
                    Destroy(enemy);
                }
            }
        }
    }

    void OnDestroy()
    {
        if (_zone)
            _zone.RemoveNode(this);
    }

    #endregion
}
