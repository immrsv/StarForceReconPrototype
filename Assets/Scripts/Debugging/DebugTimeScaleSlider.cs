using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugTimeScaleSlider : MonoBehaviour
{

    private Slider _slider = null;

    void Start()
    {
        _slider = GetComponent<Slider>();
        if (_slider)
        {
            _slider.normalizedValue = 1;
            _slider.onValueChanged.AddListener(delegate { valueChanged(); });
        }
    }

    private void valueChanged()
    {
        Time.timeScale = _slider.normalizedValue;
    }
}
