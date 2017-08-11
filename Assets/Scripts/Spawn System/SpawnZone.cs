using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnZone : MonoBehaviour
{
    #region Member variables

    #region General

    [Tooltip("Should this zone start disabled? If true, the zone must be enabled externally (trigger event, etc).")]
    [SerializeField]    private bool _startDisabled = false;

    #endregion

    #region Enemy tracking

    private List<SpawnNode> _nodes = new List<SpawnNode>();
    private int _registeredEnemies = 0;

    #endregion

    #region Spawn timing

    [Tooltip("The time in seconds to wait between wave spawning, where: \n0 = no minimum wait time; The zone will immediately spawn a wave upon death of an enemy belonging to the zone, and \n5 = a minimum of 5 seconds delay between waves being spawned.")]
    [Range(0.0f, 5.0f), SerializeField] private float _restTime = 0;
    private float _currentRestDuration = 0;

    [Tooltip("Upon spawning a wave, should the spawn zone attempt to replenish to the maximum enemy quantity? \nIf false, Wave Size will be used to determine how many enemies to spawn.")]
    [SerializeField]    private bool _spawnToMax = true;

    [Tooltip("Number maximum of enemies to spawn per wave.")]
    [Range(1, 10), SerializeField]  private uint _waveSize = 10;

    #endregion

    #region Spawn quantities

    [Tooltip("The total number of enemies available to this zone. Use 0 for unlimited potential spawning.")]
    [Range(0, 100), SerializeField]    private uint _enemyPoolSize = 0;
    private uint _spawnCount = 0;

    [Tooltip("The maximum number of enemies allowed to be alive in this zone at once.")]
    [Range(1, 100), SerializeField]    private uint _maxAliveEnemies = 10;

    [Tooltip("The minimum number of enemies allowed to be alive at one time. If the number of alive enemies drops below this value, the zone will attempt to spawn a wave of enemies immediately, ignoring rest-time.")]
    [Range(0, 10), SerializeField]  private uint _minAliveEnemies = 0;

    /* Returns true if the spawn zone is still capable of spawning enemies. */
    public bool hasRemainingSpawns
    { get { return (_spawnCount < _enemyPoolSize) || _enemyPoolSize == 0; } }

    /* Returns the number of possible spawns left for this zone. Returns -1 if the zone is allowed unlimited spawns. */
    public int remainingSpawns
    {
        get
        {
            if (_enemyPoolSize == 0)
                return -1;
            return ((int)_enemyPoolSize - (int)_spawnCount);
        }
    }

    #endregion

    #endregion

    #region Member Functions

    void Awake()
    {
        if (_startDisabled)
            this.enabled = false;
    }

    void Update ()
    {
        if (hasRemainingSpawns)
        {
            bool restQualified = true;

            // Wave timer logic
            if (_restTime > 0)
            {
                // Check if this zone has been resting long enough to qualify for another wave-spawn
                _currentRestDuration += Time.deltaTime;
                if ( !(_currentRestDuration >= _restTime) )
                    restQualified = false;
            }

            // Attempt to spawn a wave if not resting, or if there are less enemies alive than minimum allowed
            if (restQualified || (_registeredEnemies < _minAliveEnemies))
            {
                // Calculate how many enemies to spawn
                int quantity = 0;
                if (_spawnToMax)
                    quantity = Mathf.Min(remainingSpawns, ((int)_maxAliveEnemies - _registeredEnemies));
                else
                    quantity = Mathf.Min(remainingSpawns, (int)_waveSize);

                if (quantity > 0)
                    SpawnEnemies(quantity);
            }
        }
	}

    private void OnEnemyDeath(Health sender, float damageValue)
    {
        _registeredEnemies--;
    }

    /// <summary>
    /// Spawns and evenly distributes the specified number of enemies over
    /// all attached SpawnNodes.
    /// </summary>
    private void SpawnEnemies(int quantity)
    {
        if (quantity <= 0)
        {
            Debug.LogError("SpawnZone's SpawnEnemy function was called with quantity paramater <= 0.");
            return;
        }

        // Split spawns evenly between all attached nodes
        int nodes = _nodes.Count;
        if (nodes > 0)
        {
            // Divide quantity up by each node
            int qtyEachNode = (int)(quantity / nodes);
            int remainder = quantity % nodes;

            // Loop through each node and spawn enemies
            for (int i = 0; i < nodes; i++)
            {
                SpawnNode n = _nodes[i];

                int quantityForThisNode = qtyEachNode;
                if (i < remainder)
                    quantityForThisNode++;

                if (quantityForThisNode > 0)
                    n.SpawnEnemies(quantityForThisNode);
            }

            // Reset rest time
            _currentRestDuration = 0;
        }
    }

    /// <summary>
    /// Registers the specified enemy as belonging to this zone.
    /// Once registered, the enemy will be tracked & accounted for by the system.
    /// </summary>
    public void RegisterEnemyToZone(Health h)
    {
        if (h)
        {
            _registeredEnemies++;
            _spawnCount++;
            h.OnDeath += OnEnemyDeath;
        }
    }

    /// <summary>
    /// Adds the specified node, allowing it to spawn enemies for this zone.
    /// </summary>
    public void AddNode(SpawnNode node)
    {
        if (node)
            _nodes.Add(node);
    }

    /// <summary>
    /// Removes the specified node from the zone.
    /// </summary>
    public void RemoveNode(SpawnNode node)
    {
        if (node)
            _nodes.Remove(node);
    }

    #endregion
}
