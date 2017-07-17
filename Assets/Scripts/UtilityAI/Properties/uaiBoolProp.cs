using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JakePerry
{
    public class uaiBoolProp : uaiBaseProp
    {
        [SerializeField()]  private bool _value = false;

        public override float normalizedValue
        {
            get { return (_value) ? 1.0f : 0.0f; }
        }

        protected override void Start()
        {
            if (_startRandom)
                _value = (Random.value >= 0.5f);
        }

        /// <summary>
        /// Sets the value of the property to true if val is greater than 
        /// or equal to 0.5, else the value is set to false.
        /// </summary>
        public override void SetValue(float val)
        {
            _value = (val >= 0.5f);
        }
    }
}
