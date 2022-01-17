using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Code by Mark Li Zonglin (2020)

/// <summary>
/// An component attached to the root of a player model and handles events set up in the animation clips
/// It is mainly used to trigger foot step sound effects
/// Requires a AudioSource to also be attached to the GameObject
/// </summary> 
public class AnimationEventHandler : MonoBehaviour
{

    /*
        NOTE:
        UnityEvents that are invoked whenever the character takes a step
        Inside the editor, set up footL and footR so that they call the PlayOnce() function 
        of an AudioSource component
        The AudioSource component should contain the corresponding AudioClips for foot steps
    */

    public UnityEvent footL;
    public UnityEvent footR;

    
    /// <summary>
    /// Called when an animation event with name "FootL" is triggered
    /// </summary>
    void FootL()
    {
        footL?.Invoke();
    }

    /// <summary>
    /// Called when an animation event with name "FootR" is triggered
    /// </summary>
    void FootR()
    {
        footR?.Invoke();
    }

}
