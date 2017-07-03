using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetSquadReferences : MonoBehaviour
{

    public List<SquaddieSwitchController> _members;
    
	void Awake ()
    {
        stSquadManager.SetSquadList(_members);

        DestroyImmediate(this);
	}
}
