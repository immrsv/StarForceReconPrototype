using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnZoneTrigger : MonoBehaviour
{

    [SerializeField]    private SpawnZone _zone = null;

    void Awake()
    {
        if (!_zone)
            _zone = GetComponentInParent<SpawnZone>();
    }

    void OnTriggerEnter(Collider c)
    {
        if (_zone)
        {
            if (c.tag == "Player")
                _zone.enabled = true;
        }
    }
}
