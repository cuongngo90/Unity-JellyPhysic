using UnityEngine;
using System.Collections;

public class JellyDetectCollision : MonoBehaviour
{
    public LayerMask validLayers = -1;
    public float minCollisionRepeatTime = 0.0f;
    JellySprite jellySprite;
    float lastCollisionTime;

    /// <summary>
    /// Awake this instance.
    /// </summary>
    void Awake()
    {
        jellySprite = GetComponent<JellySprite>();
    }

    /// <summary>
    /// Raises the jelly collision enter event for 3D collisions.
    /// </summary>
    void OnJellyCollisionEnter(JellySprite.JellyCollision jellyCollision)
    {
        if(Time.time >= lastCollisionTime + minCollisionRepeatTime)
        {
            // Check if the colliding object's layer matches the layerMask
            if((1 << jellyCollision.Collision.collider.gameObject.layer) != 0)
            {
                Debug.Log("Collision Detected with " + jellyCollision.Collision.gameObject.name +  " at " + jellyCollision.Collision.contacts[0].point);
                lastCollisionTime = Time.time;
            }
        }      
    }

    /// <summary>
    /// Raises the jelly collision enter event for 2D collisions.
    /// </summary>
    void OnJellyCollisionEnter2D(JellySprite.JellyCollision2D jellyCollision)
    {
        if(Time.time >= lastCollisionTime + minCollisionRepeatTime)
        {
            // Check if the colliding object's layer matches the layerMask
            if((1 << jellyCollision.Collision2D.gameObject.layer) != 0)
            {
                Debug.Log("Collision Detected with " + jellyCollision.Collision2D.gameObject.name + " at " + jellyCollision.Collision2D.contacts[0].point);
                lastCollisionTime = Time.time;
            }
        }
    }

    /// <summary>
    /// Raises the jelly trigger enter event for 3D triggers.
    /// </summary>
    void OnJellyTriggerEnter(JellySprite.JellyCollider jellyCollider)
    {
        if(Time.time >= lastCollisionTime + minCollisionRepeatTime)
        {
            // Check if the colliding object's layer matches the layerMask
            if((1 << jellyCollider.Collider.gameObject.layer) != 0)
            {
                Debug.Log("Trigger hit - " + jellyCollider.Collider.gameObject.name);
                lastCollisionTime = Time.time;
            }
        }
    }

    /// <summary>
    /// Raises the jelly trigger enter event for 2D triggers.
    /// </summary>
    void OnJellyTriggerEnter2D(JellySprite.JellyCollider2D jellyCollider)
    {
        if(Time.time >= lastCollisionTime + minCollisionRepeatTime)
        {
            // Check if the colliding object's layer matches the layerMask
            if((1 << jellyCollider.Collider2D.gameObject.layer) != 0)
            {
                Debug.Log("Trigger hit - " + jellyCollider.Collider2D.gameObject.name);
                lastCollisionTime = Time.time;
            }
        }
    }
}