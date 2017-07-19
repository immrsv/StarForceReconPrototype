using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Base class for utility AI properties. */
namespace JakePerry
{
    [System.Serializable]
    public abstract class uaiBaseProp
    {
        [SerializeField()]  protected string _name = "";
        public string name
        {
            get { return _name; }
            set { _name = value; }
        }
        
        /// <summary>
        /// Returns the normalized value of this property, that is the value in range 0-1.
        /// </summary>
        public abstract float normalizedValue { get; }  // Each derived class must implement this property

        [Tooltip("If true, the property will start with a random value between the specified minimum and maximum starting values.")]
        [SerializeField()]  protected bool _startRandom = false;
        [Range(0.0f, 1.0f), SerializeField()]   protected float _minStartValue = 0.0f;
        [Range(0.0f, 1.0f), SerializeField()]   protected float _maxStartValue = 1.0f;

        public float minStartValue
        {
            get { return _minStartValue; }
            set { _minStartValue = value; }
        }
        public float maxStartValue
        {
            get { return _maxStartValue; }
            set { _maxStartValue = value; }
        }

        // Constructor
        public uaiBaseProp()
        {
            Start();
        }

        /// <summary>
        /// Used internally by the constructor to initialize property variables.
        /// </summary>
        protected abstract void Start();

        /// <summary>
        /// Sets the value of the property, clamped to the range of 0-1.
        /// For boolean properties, value is set to true if the val parameter is
        /// greater than 0.5.
        /// </summary>
        public abstract void SetValue(float val);

    }
}
