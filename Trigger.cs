using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Code by Mark Li Zonglin (2020)

///<summary>
/// Attach to any GameObjects with a collider component and can be used to call functions on other monobehaviours.
/// It is used to construct puzzles by trigging events upon the player overlapping with the collider 
///</summary>
public class Trigger : MonoBehaviour
{
    public UnityEvent onCollide;            // Invoked when there is a collision 

    [SerializeField] bool m_AnyTag = false; // whether the collision applies to objects with any tag or just the player

    void OnTriggerEnter(Collider other)
    {
        // Invoke onCollide when the collider overlaps with another object
        // only works when the IsTrigger is set to true for the Collider component 
        if(m_AnyTag || other.CompareTag("Player"))
            onCollide?.Invoke();
    }

    void OnCollisionEnter(Collision collision)
    {
        // Invoke onCollide when the collider comes in contact with another obejct
        // only works with IsTrigger is set to false
        if(m_AnyTag || collision.collider.CompareTag("Player"))
            onCollide?.Invoke();
    }

}
