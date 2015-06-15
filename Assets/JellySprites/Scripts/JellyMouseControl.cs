using UnityEngine;
using System.Collections;

[RequireComponent(typeof(JellySprite))]
public class JellyMouseControl : MonoBehaviour
{
    JellySprite jellySprite;

    void Start()
    {
       // Cache the jelly sprite component (saves having to find it each frame)
       jellySprite = GetComponent<JellySprite>();
   }

    void Update()
    {
        // Get the mouse position in world coords
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Keep the z position the same
        mousePosition.z = jellySprite.CentralPoint.GameObject.transform.position.z;

        // Move the jelly sprite to the mouse position (separate code for 2D/3D modes)
        if (jellySprite.CentralPoint.Body2D)
        {
            jellySprite.CentralPoint.Body2D.MovePosition(mousePosition);
        }
        else if (jellySprite.CentralPoint.Body3D)
        {
            Debug.Log("Mouse button: " + mousePosition);
            jellySprite.CentralPoint.Body3D.MovePosition(mousePosition);
        }
    }
}