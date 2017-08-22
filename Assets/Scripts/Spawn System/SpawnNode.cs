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
    private class SpawnNodeEnemy
    {
        public GameObject _enemy;
        [Range(0, 100)] public float _chance;
        public float _prevChance;
    }

    [SerializeField, HideInInspector]    private List<SpawnNodeEnemy> _spawnables;

    private GameObject _enemyEmptyParent = null;
    [SerializeField, HideInInspector]   private Transform _spawnLocation = null;

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

    #region Chance-Ratio Functionality

    /// <summary>Used internally to find the index of the spawnable enemy of which has had it's chance to spawn altered.</summary>
    private int FindAlteredChanceIndex()
    {
        for (int i = 0; i < _spawnables.Count; i++)
        {
            if (_spawnables[i]._prevChance != _spawnables[i]._chance)
            {
                return i;
            }
        }

        return -1;
    }

    /// <summary>Returns the sum of each spawnable's chance value.</summary>
    /// <param name="indexToIgnore">Optional index to ignore. Element at this index will not be added to the sum.</param>
    private float GetChanceSum(int indexToIgnore = -1)
    {
        float total = 0.0f;
        for (int i = 0; i < _spawnables.Count; i++)
        {
            if (i != indexToIgnore)
                total += _spawnables[i]._chance;
        }

        return total;
    }

    /// <summary>Used internally to update each spawnable's previous-chance so it may be used next validation.</summary>
    private void UpdateTrackersForChanceValues()
    {
        for (int i = 0; i < _spawnables.Count; i++)
        {
            SpawnNodeEnemy s = _spawnables[i];
            s._prevChance = s._chance;
        }
    }

    private void KeepSpawnChancesInRatio()
    {
        if (_spawnables.Count > 0)
        {
            // Find which spawnable had it's chance value modified.
            int index = FindAlteredChanceIndex();

            // Get the sum of all chance values
            float sum = GetChanceSum();
            float sumExcludingModified = GetChanceSum(index);
            float remainder = (index >= 0) ? 100 - _spawnables[index]._chance : 100;

            if (sum != 100.0f)
            {
                bool changeMade = false;
                // Iterate through each spawnable
                for (int i = 0; i < _spawnables.Count; i++)
                {
                    SpawnNodeEnemy s = _spawnables[i];

                    // Find the ratio, then set the value to this ratio of the remainder
                    if (s._chance > 0 && i != index)
                    {
                        changeMade = true;
                        float ratio = s._chance / sumExcludingModified;
                        s._chance = remainder * ratio;
                    }
                }

                if (!changeMade)
                {
                    // No changes were made! Either there is only one spawnable, or all non-modified are at 0
                    if (_spawnables.Count == 1)
                        _spawnables[0]._chance = 100;
                    else
                    {
                        int zeroQuantity = 0;
                        foreach (SpawnNodeEnemy s in _spawnables)
                        {
                            if (s._chance == 0)
                                zeroQuantity++;
                        }

                        float ratio = remainder / zeroQuantity;
                        for (int i = 0; i < _spawnables.Count; i++)
                        {
                            SpawnNodeEnemy s = _spawnables[i];

                            if (i != index)
                                s._chance = ratio;
                        }
                    }
                }
            }

            // Update each spawnable's previous-chance for use next Validation
            UpdateTrackersForChanceValues();
        }
    }

    void OnValidate()
    {
        KeepSpawnChancesInRatio();
    }

    #endregion

    void Start()
    {
        /* NOTE: Components will not receive the OnDestroy method call
         * if they do not have Start, Update, FixedUpdate, etc implemented.
         * 
         * The Start function is only implemented here to ensure the OnDestroy
         * method is called. 
         */
    }

    private GameObject PickRandomEnemy()
    {
        // Remove any instances with null enemy reference
        bool anyRemoved = false;
        int i = 0;
        while (i < _spawnables.Count)
        {
            if (_spawnables[i]._enemy == null)
            {
                _spawnables.RemoveAt(i);
                anyRemoved = true;
                i--;
            }

            i++;
        }

        // Re-calculate ratios if any elements were removed
        if (anyRemoved)
        {
            Debug.LogWarning("Warning: One or more spawnable-enemies were removed due to null enemy reference.", this);
            KeepSpawnChancesInRatio();
        }

        // Check there are still spawnables left
        if (_spawnables.Count == 0)
        {
            Debug.LogWarning("Warning: No spawnable-enemies available. Destroying spawn node.", this);
            Destroy(this);
            return null;
        }

        // Sort list by ascending chance
        _spawnables.Sort((x, y) => x._chance.CompareTo(y._chance));

        // Pick a random enemy
        float rand = Random.Range(0, 100);
        float currentChance = 0.0f;
        for (int j = 0; j < _spawnables.Count; j++)
        {
            currentChance += _spawnables[j]._chance;

            if (rand <= currentChance)
                return _spawnables[j]._enemy;
        }

        return _spawnables[_spawnables.Count - 1]._enemy;
    }

    public void SpawnEnemies(int quantity)
    {
        if (_spawnables.Count > 0)
        {
            for (int i = 0; i < quantity; i++)
            {
                GameObject e = PickRandomEnemy();
                if (e == null)
                    return;

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

                // Set the initial position for the NavMeshAgent component
                UnityEngine.AI.NavMeshAgent agent = enemy.GetComponent<UnityEngine.AI.NavMeshAgent>();
                if (agent)
                    agent.Warp(agent.transform.position);

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
