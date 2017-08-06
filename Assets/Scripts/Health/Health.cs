using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This script should be placed on each enemy & playable character in the scene.
 * Handles health, damage, healing, etc. */
public class Health : MonoBehaviour
{
    #region Delegates & Events

    public delegate void DamageEventDelegate(Health sender, float damageValue);

    public event DamageEventDelegate OnDamage;
    public event DamageEventDelegate OnDeath;
    public event DamageEventDelegate OnHeal;
    public event DamageEventDelegate OnHealthChanged;

    #endregion

    #region Health Variables

    [Range(1, 1000), SerializeField]    private int _maxHealth = 100;
    [SerializeField, HideInInspector]   private float _currentHealth = 0.0f;
    [SerializeField]    private bool _isAlive = true;

    #endregion

    #region Start Value Variables

    [Range(1, 1000), SerializeField]    private int _startingHealth = 100;

    [SerializeField]    private bool _startRandom = false;
    [Range(1, 1000), SerializeField]    private int _startMinimum = 60;
    [Range(1, 1000), SerializeField]    private int _startMaximum = 100;

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

    #region Get Properties

    public int maxHealth { get { return _maxHealth; } }
    public bool alive { get { return _isAlive; } }

    public float health
    {
        get { return _currentHealth; }
        set
        {
            if (value >= 0)
            {
                float oldHealth = _currentHealth;
                _currentHealth = value;

                if (value == 0)
                    Death(oldHealth);
            }
        }
    }

    #endregion

    void Start ()
    {
        /* NOTE: Components will not receive the OnDestroy method call
         * if they do not have Start, Update, FixedUpdate, etc implemented.
         * 
         * The Start function is only implemented here to ensure the OnDestroy
         * method is called. 
         */
    }

    /// <summary>
    /// Safely raises the specified event if it has any registered listeners.
    /// </summary>
    private void RaiseEvent(DamageEventDelegate e, float damageValue)
    {
        // Check there are any registered listeners before firing event
        DamageEventDelegate del = e;
        if (del != null)
            e(this, damageValue);
    }

    private void Death(float damage)
    {
        _isAlive = false;
        _currentHealth = 0;
        
        // Raise event
        RaiseEvent(OnDeath, damage);
    }

    /// <param name="damage">Damage to apply. Absolute value is used, use ApplyHeal to heal the entity.</param>
    public void ApplyDamage(float damage)
    {
        if (_isAlive)
        {
            if (damage < 0) damage *= -1.0f;

            _currentHealth -= damage;

            // Raise events
            RaiseEvent(OnHealthChanged, damage);
            RaiseEvent(OnDamage, damage);
        }
    }

    /// <param name="heal">Healing to apply. Absolute value is used, use ApplyDamage to damage the entity.</param>
    public void ApplyHeal(float heal)
    {
        if (_isAlive)
        {
            if (heal < 0) heal *= -1.0f;

            _currentHealth += heal;

            // Raise events
            RaiseEvent(OnHealthChanged, heal);
            RaiseEvent(OnHeal, heal);
        }
    }

    void OnDestroy()
    {
        // Call death of script, raising OnDeath event
        Death(_currentHealth);
    }
}
