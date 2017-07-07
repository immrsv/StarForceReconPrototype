using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This script should be placed on a Game Manager object in the scene.
 * Adds the list of SquaddieController scripts as squad members. */
public class SetSquadReferences : MonoBehaviour
{

    public List<SquaddieController> _members;
    
	void Awake ()
    {
        // Set each instance in the list to AI mode
        foreach (SquaddieController s in _members)
            s.DeselectSquaddie();

        // Set the list
        stSquadManager.SetSquadList(_members);

        // Set selected squaddie to Control mode
        SquaddieController currentSquaddie = stSquadManager.GetCurrentSquaddie;
        if (currentSquaddie)
            currentSquaddie.SelectSquaddie();
        
        DestroyImmediate(this);
	}
}
