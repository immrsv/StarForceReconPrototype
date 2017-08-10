using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    Health health;
    RectTransform rect;

    private void _health_OnDamage(Health sender, float damageValue)
    {
        // This is called each time the health is changed. 
        // This is where we update the bar's values

        


        // TODO:
        // Use the 'sender' parameter to get the max health
        // Get the current health
        // divide current by max and set _barFill to this value
        // This is now the value to fill the bar

        // Attach a slider UI component or something, and set the value
    }

    void Start ()
    {
        rect = GetComponent<RectTransform>();
        OverheadHUD hud = transform.parent.GetComponent<OverheadHUD>();
        health = hud.target.GetComponent<Health>();
    }

    void Update()
    {
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (100*health.health)/health.maxHealth);
    }

}
