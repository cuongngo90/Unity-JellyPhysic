
using UnityEngine;
using System.Collections;

/// <summary>
/// Helper class to allow you to pause and unpause a Jelly Sprite
/// </summary>
[RequireComponent(typeof(JellySprite))]
public class JellyPause : MonoBehaviour
{
    JellySprite jellySprite;
    Vector3[] velocity;
    Vector3[] angularVelocity;
    bool isPaused = false;

    /// <summary>
    /// Awake this instance.
    /// </summary>
    void Awake()
    {
        // Cache the Jelly Sprite to avoid having to fetch it each time
        jellySprite = GetComponent<JellySprite>();
    }

    /// <summary>
    /// Pause this instance.
    /// </summary>
    public void Pause()
    {
        // If we're not already paused...
        if (!isPaused)
        {
            // ...first create our arrays to save the rigid body velocities into, if we haven't already...
            if (velocity == null)
            {
                velocity = new Vector3[jellySprite.ReferencePoints.Count];
            }

            if (angularVelocity == null)
            {
                angularVelocity = new Vector3[jellySprite.ReferencePoints.Count];
            }

            // ...now for each reference point...
            for (int loop = 0; loop < jellySprite.ReferencePoints.Count; loop++)
            {
                JellySprite.ReferencePoint referencePoint = jellySprite.ReferencePoints[loop];

                //...if this is a valid reference point...
                if (!referencePoint.IsDummy)
                {
                    //...if we're in 2D mode...
                    if (referencePoint.Body2D != null)
                    {
                        //...store the velocity and angular velocity...
                        velocity[loop] = new Vector3(referencePoint.Body2D.velocity.x, referencePoint.Body2D.velocity.y, 0);
                        angularVelocity[loop] = new Vector3(0, 0, referencePoint.Body2D.angularVelocity);

                        //...and freeze the rigid body.
                        referencePoint.Body2D.velocity = Vector2.zero;
                        referencePoint.Body2D.angularVelocity = 0.0f;
                        referencePoint.Body2D.isKinematic = true;
                    }
                    //...if we're in 3D mode...
                    else if (referencePoint.Body3D != null)
                    {
                        //...store the velocity and angular velocity...
                        velocity[loop] = referencePoint.Body3D.velocity;
                        angularVelocity[loop] = referencePoint.Body3D.angularVelocity;

                        //...and freeze the rigid body.
                        referencePoint.Body3D.velocity = Vector3.zero;
                        referencePoint.Body3D.angularVelocity = Vector3.zero;
                        referencePoint.Body3D.isKinematic = true;
                    }
                }
            }

            isPaused = true;
        }
    }

    /// <summary>
    /// Unpause this instance.
    /// </summary>
    public void Unpause()
    {
        // If we are already paused...
        if (isPaused)
        {
            // ...for each reference point...
            for (int loop = 0; loop < jellySprite.ReferencePoints.Count; loop++)
            {
                JellySprite.ReferencePoint referencePoint = jellySprite.ReferencePoints[loop];

                //...if this is a valid reference point...
                if (!referencePoint.IsDummy)
                {
                    //...if we're in 2D mode...
                    if (referencePoint.Body2D != null)
                    {
                        //...set the body to non-kinematic and restore the saved velocities.
                        referencePoint.Body2D.isKinematic = false;
                        referencePoint.Body2D.velocity = velocity[loop];
                        referencePoint.Body2D.angularVelocity = angularVelocity[loop].z;
                    }
                    //...if we're in 3D mode...
                    else if (referencePoint.Body3D != null)
                    {
                        //...set the body to non-kinematic and restore the saved velocities.
                        referencePoint.Body3D.isKinematic = false;
                        referencePoint.Body3D.velocity = velocity[loop];
                        referencePoint.Body3D.angularVelocity = angularVelocity[loop];
                    }
                }
            }

            isPaused = false;
        }
    }
}
