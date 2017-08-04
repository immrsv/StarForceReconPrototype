using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This script should be placed on each enemy & playable character in the scene.
 * Handles health, damage, healing, etc. */
public class Health : MonoBehaviour
{
    #region Delegates

    public delegate void DamageEventDelegate(object sender, float damageValue, float previousHealth, float newHealth);

    public event DamageEventDelegate OnDamage;
    public event DamageEventDelegate OnDeath;
    public event DamageEventDelegate OnHeal;
    public event DamageEventDelegate OnHealthChanged;

    #endregion

    #region Health Variables

    [Range(1, 1000), SerializeField]    private int _maxHealth = 100;
    [SerializeField, HideInInspector]   private float _currentHealth = 0.0f;

    #endregion

    #region Start Value Variables

    [Range(1, 1000), SerializeField]private int _startingHealth = 100;

    [SerializeField]    private bool _startRandom = false;
    [Range(1, 1000), SerializeField]  private int _startMinimum = 60;
    [Range(1, 1000), SerializeField] private int _startMaximum = 100;

    #endregion

    #region Recharge Variables

    [SerializeField]    private bool _rechargeWhenLow;

    [Tooltip("Health will begin recharging when below this percentage, and stop once reached.")]
    [Range(5.0f, 50.0f), SerializeField]    private float _lowThresholdRecharge = 15.0f;

    [Tooltip("Time in seconds after taking damage before recharge will begin.")]
    [Range(1.0f, 5.0f), SerializeField] private float _delayAfterDamage = 2.0f;

    [Tooltip("Percent of Max Health to regain each second while recharging.")]
    [Range(1.0f, 50.0f), SerializeField]    private float _gainPerSecond = 5.0f;

    #endregion

    void Awake()
    {

    }

    void Start ()
    {
		
	}
	
	void Update ()
    {
		
	}
    
    /// <summary>
    /// Safely raises the specified event if it has any registered listeners.
    /// </summary>
    private void RaiseEvent(DamageEventDelegate e, float damageValue, float previousHealth)
    {
        // Check there are any registered listeners before firing event
        DamageEventDelegate del = e;
        if (del != null)
            e(this, damageValue, previousHealth, _currentHealth);
    }

    private void Death(float damage, float previousHealth)
    {
        // TODO: Handle death
        
        RaiseEvent(OnDeath, damage, previousHealth);
    }

    public void ApplyDamage(float damage)
    {
        float previousHealth = _currentHealth;

        // TODO: Apply damage

        RaiseEvent(OnHealthChanged, damage, previousHealth);
        RaiseEvent(OnDamage, damage, previousHealth);
    }

    void OnDestroy()
    {
        RaiseEvent(OnDeath, -1, -1);
    }
}
