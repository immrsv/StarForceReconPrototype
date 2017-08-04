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
        for (int i = 0; i < _members.Count; i++)
        {
            SquaddieController s = _members[i];

            if (s == null)
            {
                // Remove the reference from the list
                _members.Remove(s);

                // De-increment iterator
                i--;
            }
            else
                s.DeselectSquaddie();
        }
        foreach (SquaddieController s in _members)
        {
            if (s == null)
                _members.Remove(s);
            else
                s.DeselectSquaddie();
        }

        // Set the list
        stSquadManager.SetSquadList(_members);

        // Set selected squaddie to Control mode
        SquaddieController currentSquaddie = stSquadManager.GetCurrentSquaddie;
        if (currentSquaddie)
            currentSquaddie.SelectSquaddie();
        
        DestroyImmediate(this);
	}
}
