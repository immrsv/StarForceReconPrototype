using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//public static void SetCursor(Texture2D texture, Vector2 hotspot, CursorMode cursorMode);
public class CursorSprite : MonoBehaviour
{
    [Tooltip("The texture to use for the cursor or null to set the default cursor. Note that a texture needs to be imported with 'Read / Write enabled' in the texture importer (or using the 'Cursor' defaults), in order to be used as a cursor.")]
    public Texture2D cursorTexture;

    [Tooltip("Allow this cursor to render as a hardware cursor on supported platforms, or force software cursor.")]
    public CursorMode cursorMode = CursorMode.Auto;

    [Tooltip("The offset from the top left of the texture to use as the target point (must be within the bounds of the cursor).")]
    public Vector2 hotSpot = Vector2.zero;

    //This variable flags whether the custom cursor is active or not  
    public bool ccEnabled = false;

    void Start ()
    {
        OnMouseEnter();
    }

    void OnMouseEnter()
    {
        Cursor.SetCursor(this.cursorTexture, hotSpot, cursorMode);
        Debug.Log("Custom cursor has been set.");
        this.ccEnabled = true;
    }

    /// <summary>
    /// Specify a custom cursor that you wish to use as a cursor.
    /// </summary>
    void OnDisable()
    {
        Cursor.SetCursor(null, Vector2.zero, cursorMode);
        this.ccEnabled = false;
}
    // Use this for initialization
   
}
