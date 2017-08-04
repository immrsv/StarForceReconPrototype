using UnityEngine;
using UnityEditor;

public class SpawnNodeCreateMenu
{
    [MenuItem("GameObject/StarForceRecon/Spawn Node", false, 10)]
    public static void CreateSpawnNode()
    {
        // Get selected object for parent
        GameObject selected = Selection.activeGameObject;

        // Create spawn node
        if (selected)
            selected.AddComponent<SpawnNode>();
        else
        {
            // Create a new object to hold the node component
            GameObject g = new GameObject("Spawn Node");
            g.transform.position = Vector3.zero;
            g.AddComponent<SpawnNode>();

            // Select the new item
            Selection.activeGameObject = g;
        }
    }
}
