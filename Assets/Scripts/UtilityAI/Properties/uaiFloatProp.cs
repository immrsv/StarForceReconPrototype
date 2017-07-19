using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JakePerry
{
    [System.Serializable]
    public class uaiFloatProp : uaiBaseProp
    {
        [SerializeField()]  private float _value;
        [SerializeField()]  private float _minValue = 0.0f;
        [SerializeField()]  private float _maxValue = 1.0f;

        public override float normalizedValue
        {
            get
            {
                VerifyMinMaxOrder();
                float difference = _maxValue - _minValue;
                return (_value - _minValue) / difference;
            }
        }

        protected override void Start()
        {
            if (_startRandom)
                _value = Random.Range(_minValue, _maxValue);
        }

        /// <summary>
        /// Used internally to verify the min and max values are
        /// correctly ordered.
        /// </summary>
        private void VerifyMinMaxOrder()
        {
            if (_minValue > _maxValue)
            {
                // Switch the two values
                float temp = _minValue;
                _minValue = _maxValue;
                _maxValue = temp;
            }
        }

        /// <summary>
        /// Sets the value of the property to val. This value is clamped
        /// to the minimum & maximum values stored by this property.
        /// </summary>
        public override void SetValue(float val)
        {
            VerifyMinMaxOrder();
            float newValue = Mathf.Clamp(val, _minValue, _maxValue);
            _value = newValue;
        }
    }
}
