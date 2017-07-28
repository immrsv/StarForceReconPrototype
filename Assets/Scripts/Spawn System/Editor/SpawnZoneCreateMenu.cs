using UnityEngine;
using UnityEditor;

public class SpawnZoneCreateMenu
{
    [MenuItem("GameObject/StarForceRecon/Spawn Zone", false, 10)]
    public static void CreateSpawnZone()
    {
        // Get selected object for parent
        GameObject selected = Selection.activeGameObject;

        // Create spawn zone object
        GameObject g = new GameObject("Spawn Zone");
        if (selected)
        {
            g.transform.parent = selected.transform;
            g.transform.localPosition = Vector3.zero;
        }
        else
            g.transform.position = Vector3.zero;

        g.AddComponent<SpawnZone>();

        // Select the new item
        Selection.activeGameObject = g;
    }
}
