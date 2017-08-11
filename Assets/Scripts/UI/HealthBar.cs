using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    Health health;
    RectTransform rect;
    OverheadHUD hud = null;
    float maxWidth = 0;

    private void Health_OnDamage(Health sender, float damageValue)
    {
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (sender.health / sender.maxHealth) * maxWidth);
    }

    private void Health_OnDeath(Health sender, float damageValue)
    {
        gameObject.SetActive(false);
        if (hud)
            hud.gameObject.SetActive(false);
    }

    void Start ()
    {
        rect = GetComponent<RectTransform>();
        maxWidth = rect.sizeDelta.x;
        hud = transform.parent.GetComponent<OverheadHUD>();

        health = hud.target.GetComponent<Health>();
        if (health)
        {
            health.OnDamage += Health_OnDamage;
            health.OnDeath += Health_OnDeath;
        }
    }
    
}
