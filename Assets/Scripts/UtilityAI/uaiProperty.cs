using UnityEngine;

namespace JakePerry
{
    [System.Serializable]
    public class uaiProperty
    {
        #region Member Variables

        public enum UAIPROPTYPE { BOOL, INT, FLOAT };

        // Store property type
        [SerializeField, HideInInspector]   private UAIPROPTYPE _propertyType;
        public bool isBool  { get { return _propertyType == UAIPROPTYPE.BOOL;   } }
        public bool isInt   { get { return _propertyType == UAIPROPTYPE.INT;    } }
        public bool isFloat { get { return _propertyType == UAIPROPTYPE.FLOAT;  } }

        // Property-type specific values
        [SerializeField, HideInInspector]   private bool _boolValue;
        [SerializeField, HideInInspector]   private float _floatValue;
        [SerializeField, HideInInspector]   private int _intValue;

        public bool boolValue   { get { return _boolValue; } }
        public float floatValue { get { return _floatValue; } }
        public int intValue     { get { return _intValue; } }

        // Allowed value clamps
        [SerializeField, HideInInspector]   private float _fminValue;
        [SerializeField, HideInInspector]   private float _fmaxValue;
        [SerializeField, HideInInspector]   private int _iminValue;
        [SerializeField, HideInInspector]   private int _imaxValue;

        // Random starting values
        [SerializeField, HideInInspector]   private bool _startRandom;
        [SerializeField, HideInInspector]   private float _fminStartValue;
        [SerializeField, HideInInspector]   private float _fmaxStartValue;
        [SerializeField, HideInInspector]   private int _iminStartValue;
        [SerializeField, HideInInspector]   private int _imaxStartValue;

        public bool startRandom
        {
            get { return _startRandom; }
            set { _startRandom = value; }
        }

        [SerializeField]    private string _name;
        public string name
        {
            get { return _name; }
        }

        #endregion

        #region Member Functions

        /// <summary>
        /// Constructs a new float property.
        /// </summary>
        /// <param name="value">The value this property should start as (if not starting random).</param>
        /// <param name="minValue">The minimum allowed value for this property.</param>
        /// <param name="maxValue">The maximum allowed value for this property.</param>
        /// <param name="startRandom">Should this property's start value be randomized?</param>
        /// <param name="minRandom">Minimum starting value. This is clamped to minValue.</param>
        /// <param name="maxRandom">Maximum starting value. This is clamped to maxValue</param>
        public uaiProperty(float value, float minValue = -200.0f, float maxValue = 200.0f,
            bool startRandom = false, float minRandom = 0.0f, float maxRandom = 100.0f)
        {
            _propertyType = UAIPROPTYPE.FLOAT;
            _name = "Float property";

            _fminValue = minValue;
            _fmaxValue = maxValue;
            _floatValue = value;

            _startRandom = startRandom;
            _fminStartValue = minRandom;
            _fmaxStartValue = maxRandom;

            // Initialize unused properties
            _boolValue = false;
            _intValue = 0;
            _iminValue = -200;
            _imaxValue = 200;
            _iminStartValue = 0;
            _imaxStartValue = 100;
        }

        /// <summary>
        /// Constructs a new boolean property.
        /// </summary>
        /// <param name="value">The value this property should start as (if not starting random).</param>
        /// <param name="startRandom">Should this property's start value be randomized?</param>
        public uaiProperty(bool value, bool startRandom = false)
        {
            _propertyType = UAIPROPTYPE.BOOL;
            _name = "Boolean property";

            _startRandom = startRandom;

            // Initialize unused properties
            _boolValue = false;
            _intValue = 0;
            _iminValue = -200;
            _imaxValue = 200;
            _iminStartValue = 0;
            _imaxStartValue = 0;

            _floatValue = 0.0f;
            _fminValue = -200.0f;
            _fmaxValue = 200.0f;
            _fminStartValue = 0.0f;
            _fmaxStartValue = 100.0f;
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
        public uaiProperty(int value, int minValue = -200, int maxValue = 200,
            bool startRandom = false, int minRandom = 0, int maxRandom = 100)
        {
            _propertyType = UAIPROPTYPE.INT;
            _name = "Integer property";

            _iminValue = minValue;
            _imaxValue = maxValue;
            _intValue = value;

            _startRandom = startRandom;
            _iminStartValue = minRandom;
            _imaxStartValue = maxRandom;

            // Initialize unused properties
            _floatValue = 0.0f;
            _fminValue = -200.0f;
            _fmaxValue = 200.0f;
            _fminStartValue = 0.0f;
            _fmaxStartValue = 100.0f;

            _boolValue = false;
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
                        fVerifyBoundingOrder();

                        if (_startRandom)
                            SetValue(Random.Range(_fminStartValue, _fmaxStartValue));

                        break;
                    }

                case UAIPROPTYPE.INT:
                    {
                        iVerifyBoundingOrder();

                        if (_startRandom)
                            SetValue(Random.Range(_iminStartValue, _imaxStartValue));

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

        #endregion
    }
}
