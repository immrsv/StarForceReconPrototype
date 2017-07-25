using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Base class for utility AI properties.
 * NOTE: Due to Unity's inability to serialize abstract classes,
 * this class is not abstract as it should be. Functions which must 
 * be overwritten by inheriting classes are:
 * normalizedValue property get.
 * Start function.
 * SetValue function.
 */
namespace JakePerry
{
    [System.Serializable]
    public class uaiProperty : UnityEngine.Object
    {
        public enum UAIPROPTYPE { BOOL, INT, FLOAT };

        // Store property type
        [SerializeField, HideInInspector]   private UAIPROPTYPE _propertyType = UAIPROPTYPE.BOOL;
        public bool isBool  { get { return _propertyType == UAIPROPTYPE.BOOL;   } }
        public bool isInt   { get { return _propertyType == UAIPROPTYPE.INT;    } }
        public bool isFloat { get { return _propertyType == UAIPROPTYPE.FLOAT;  } }

        // Property-type specific values
        [SerializeField, HideInInspector]   private bool _boolValue = false;
        [SerializeField, HideInInspector]   private float _floatValue = 0.0f;
        [SerializeField, HideInInspector]   private int _intValue = 0;

        // Allowed value clamps
        [SerializeField]    private float _fminValue = float.MinValue;
        [SerializeField]    private float _fmaxValue = float.MaxValue;
        [SerializeField]    private int _iminValue = int.MinValue;
        [SerializeField]    private int _imaxValue = int.MaxValue;
        
        // Random starting values
        [SerializeField]    private bool _startRandom = false;
        [SerializeField]    private float _fminStartValue = 0.0f;
        [SerializeField]    private float _fmaxStartValue = 100.0f;
        [SerializeField]    private int _iminStartValue = 0;
        [SerializeField]    private int _imaxStartValue = 100;

        private string _name = "";
        public new string name
        {
            get { return _name; }
        }

        /// <summary>
        /// Constructs a new float property.
        /// </summary>
        /// <param name="value">The value this property should start as (if not starting random).</param>
        /// <param name="minValue">The minimum allowed value for this property.</param>
        /// <param name="maxValue">The maximum allowed value for this property.</param>
        /// <param name="startRandom">Should this property's start value be randomized?</param>
        /// <param name="minRandom">Minimum starting value. This is clamped to minValue.</param>
        /// <param name="maxRandom">Maximum starting value. This is clamped to maxValue</param>
        public uaiProperty(float value, float minValue = float.MinValue, float maxValue = float.MaxValue, 
            bool startRandom = false, float minRandom = 0.0f, float maxRandom = 100.0f)
        {
            Debug.Log("Float constructor");
            _propertyType = UAIPROPTYPE.FLOAT;

            _fminValue = minValue;
            _fmaxValue = maxValue;
            _floatValue = value;

            _startRandom = startRandom;
            _fminStartValue = minRandom;
            _fmaxStartValue = maxRandom;
        }

        /// <summary>
        /// Constructs a new boolean property.
        /// </summary>
        /// <param name="value">The value this property should start as (if not starting random).</param>
        /// <param name="startRandom">Should this property's start value be randomized?</param>
        public uaiProperty(bool value, bool startRandom = false)
        {
            Debug.Log("Bool constructor");
            _propertyType = UAIPROPTYPE.BOOL;
            _startRandom = startRandom;
        }

        /// <summary>
        /// Constructs a new integer property.
        /// </summary>
        /// <param name="value">The value this property should start as (if not starting random).</param>
        /// <param name="minValue">The minimum allowed value for this property.</param>
        /// <param name="maxValue">The maximum allowed value for this property.</param>
        /// <param name="startRandom">Should this property's start value be randomized?</param>
        /// <param name="minRandom">Minimum starting value. This is clamped to minValue.</param>
        /// <param name="maxRandom">Maximum starting value. This is clamped to maxValue</param>
        public uaiProperty(int value, int minValue = int.MinValue, int maxValue = int.MaxValue,
            bool startRandom = false, int minRandom = 0, int maxRandom = 100)
        {
            Debug.Log("Int constructor");
            _propertyType = UAIPROPTYPE.INT;

            _iminValue = minValue;
            _imaxValue = maxValue;
            _intValue = value;

            _startRandom = startRandom;
            _iminStartValue = minRandom;
            _imaxStartValue = maxRandom;
        }

        /// <summary>
        /// Returns the value at normalized range between the minimum and maximum allowed values.
        /// </summary>
        /// <param name="normalizedVal">Value between 0-1 at which to retrieve the unnormalized value</param>
        private float fGetUnnormalizedValue(float normalizedVal)
        {
            float normVal = Mathf.Clamp01(normalizedVal);

            fVerifyBoundingOrder();

            float difference = _fmaxValue - _fminValue;
            return _fminValue + (normVal * difference);
        }

        /// <summary>
        /// Returns a normalized value based on the minimum and maximum allowed values.
        /// </summary>
        private float fGetNormalizedValue(float value)
        {
            fVerifyBoundingOrder();
            float difference = _fmaxValue - _fminValue;
            return Mathf.Clamp01((value - _fminValue) / difference);
        }

        /// <summary>
        /// Used internally to verify the min & max values are correctly ordered.
        /// </summary>
        private void fVerifyBoundingOrder()
        {
            if (_fminValue > _fmaxValue)
            {
                // Switch values
                float temp = _fminValue;
                _fminValue = _fmaxValue;
                _fmaxValue = temp;
            }
        }

        /// <summary>
        /// Returns the value at normalized range between the minimum and maximum allowed values.
        /// </summary>
        /// <param name="normalizedVal">Value between 0-1 at which to retrieve the unnormalized value</param>
        private int iGetUnnormalizedValue(float normalizedVal)
        {
            float normVal = Mathf.Clamp01(normalizedVal);

            iVerifyBoundingOrder();

            int difference = _imaxValue - _iminValue;
            return (int)(_iminValue + (normVal * difference));
        }

        /// <summary>
        /// Returns a normalized value based on the minimum and maximum allowed values.
        /// </summary>
        private float iGetNormalizedValue(int value)
        {
            iVerifyBoundingOrder();
            int difference = _imaxValue - _iminValue;
            return Mathf.Clamp01((value - _iminValue) / difference);
        }

        /// <summary>
        /// Used internally to verify the min & max values are correctly ordered.
        /// </summary>
        private void iVerifyBoundingOrder()
        {
            if (_iminValue > _imaxValue)
            {
                // Switch values
                int temp = _iminValue;
                _iminValue = _imaxValue;
                _imaxValue = temp;
            }
        }

        /// <summary>
        /// Returns the normalized value of this property, that is the value in range 0-1.
        /// </summary>
        public float normalizedValue
        {
            get
            {
                switch (_propertyType)
                {
                    case UAIPROPTYPE.BOOL:
                        return (_boolValue) ? 1.0f : 0.0f;
                    case UAIPROPTYPE.FLOAT:
                        return fGetNormalizedValue(_floatValue);
                    case UAIPROPTYPE.INT:
                            return iGetNormalizedValue(_intValue);

                    default:
                        return 0.0f;
                }
            }
        }

        public override string ToString()
        {
            switch (_propertyType)
            {
                case UAIPROPTYPE.BOOL:
                    return "Boolean Utility-AI Property";
                case UAIPROPTYPE.FLOAT:
                    return "Float Utility-AI Property";
                case UAIPROPTYPE.INT:
                    return "Integer Utility-AI Property";

                default:
                    return base.ToString();
            }
        }

        /// <summary>
        /// Initializes the property. This should be called via separate monobehaviour script
        /// </summary>
        public void Start()
        {
            switch (_propertyType)
            {
                case UAIPROPTYPE.BOOL:
                    {
                        if (_startRandom)
                            _boolValue = (Random.value >= 0.5f);

                        break;
                    }

                case UAIPROPTYPE.FLOAT:
                    {
                        if (_startRandom)
                            _floatValue = Mathf.Clamp(Random.Range(_fminStartValue, _fmaxStartValue), _fminValue, _fmaxValue);

                        fVerifyBoundingOrder();
                        _floatValue = Mathf.Clamp(_floatValue, _fminValue, _fmaxValue);

                        break;
                    }

                case UAIPROPTYPE.INT:
                    {
                        if (_startRandom)
                            _intValue = Mathf.Clamp(Random.Range(_iminStartValue, _imaxStartValue), _iminValue, _imaxValue);

                        iVerifyBoundingOrder();
                        _intValue = Mathf.Clamp(_intValue, _iminValue, _imaxValue);

                        break;
                    }
            }
        }

        /// <summary>
        /// Sets the value of the property.
        /// <para>
        /// For boolean properties, value is set to true if the val parameter is
        /// greater than 0.5.</para>
        /// <para>
        /// For integer properties, value is rounded down to the nearest integer.</para>
        /// </summary>
        public void SetValue(float val)
        {
            switch (_propertyType)
            {
                case UAIPROPTYPE.BOOL:
                    {
                        _boolValue = (val > 0.5f) ? true : false;
                        break;
                    }

                case UAIPROPTYPE.FLOAT:
                    {
                        fVerifyBoundingOrder();
                        _floatValue = Mathf.Clamp(val, _fminValue, _fmaxValue);
                        break;
                    }

                case UAIPROPTYPE.INT:
                    {
                        iVerifyBoundingOrder();
                        _intValue = Mathf.Clamp((int)val, _iminValue, _imaxValue);
                        break;
                    }
            }
        }

    }
}
