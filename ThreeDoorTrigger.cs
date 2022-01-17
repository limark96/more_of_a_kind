using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Code by Mark Li Zonglin 

/// <summary>
/// A special type of Trigger that invokes an event when TriggerDoor is being called 3 times
/// It is used as a building block for a puzzle 
/// </summary>
public class ThreeDoorTrigger : MonoBehaviour
{
    public UnityEvent OnAllDoorsTriggered; // invoked by TriggerDoor()
    private int numOfDoorsTriggered = 0; 

    /// <summary>
    /// This function can be called by subscribing it to a Unity Event in the editor
    /// It can subscribe to the OnGoal event in <seealso cref="BallGoal"/> 
    /// When it has been invoked 3 times, the OnAllDoorsTriggered is invoked
    /// Function in other Monobehaviours can subscribe to this event 
    /// In the game, a gate is raised when OnAllDoorsTriggered is invoked
    /// </summary>
    public void TriggerDoor()
    {
        numOfDoorsTriggered++;
        if (numOfDoorsTriggered == 3)
            OnAllDoorsTriggered?.Invoke();
    }
}
