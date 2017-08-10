using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Simple prototyping script to disable an object when the OnDeath event is
 * called by the Health script. */
public class DisableOnDeath : MonoBehaviour
{
	void Start ()
    {
        Health h = GetComponentInParent<Health>();
        if (!h) h = GetComponentInChildren<Health>();

        if (h)
            h.OnDeath += OnDeath;
	}

    private void OnDeath(Health sender, float damageValue)
    {
        sender.gameObject.SetActive(false);
    }
}
