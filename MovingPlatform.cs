using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Code by Mark Li Zonglin (2020)

///<summary> 
/// Add this script to an object with a collider component to make it behave like a moving platform) 
/// By default, if a player character stands on a moving object, it will not move with the object like in real life
/// To achieve the intended effect, this scripts makes the player character a child of the moving platform 
/// when the character stands on top of it, such that when the platform moves, the character moves, too
/// Usually the movement of the platforms are handled with Mecanim 
///</summary> 
public class MovingPlatform : MonoBehaviour
{
    private Collider m_collider; // Local reference to the collider. This can be any type of collider (e.g. Box collider, mesh collider, etc)

    void Start()
    {
        m_collider = GetComponent<Collider>(); // initialize the collider. 
    }

    void OnCollisionEnter(Collision collision)
    {
        // If the player is making contact with the platform and is above it, make the player object an child of the platform
        if(collision.collider.CompareTag("Player"))
        {
            if(collision.collider.transform.position.y > m_collider.bounds.max.y)
                collision.collider.transform.SetParent(transform);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        // when the player is no longer in contact with the platform, unparent it to stop it from being influenced by the platform
        if(collision.collider.CompareTag("Player"))
        {
            collision.collider.transform.parent = null;
        }
    }
    
}
