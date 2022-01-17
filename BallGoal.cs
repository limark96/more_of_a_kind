using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Code by Mark Li Zonglin (2020)

/// <summary>
/// This component is used in Ball_Goal_Prefabs
/// It's built for a puzzle mehcanics
/// The player can unlocling gates by pushing a ball to make it fall into a hole on the ground
/// This scripts triggers the gate from opening
/// <summary/>
[RequireComponent(typeof(Collider))]
public class BallGoal : MonoBehaviour
{
    private GameObject ball = null; // local reference to the detected ball 
    public  ParticleSystem scoreEffect; // manual setup. triggers a particle effect when the ball enters the hole
    private AudioSource audioSource; // sound effect played when a ball enters the hole
    private bool scored = false; // whether a ball has already entered the goal
    
    /// Inside the editor, the OnGoal event can be set up to call TriggerDoor() in <see cref="ThreeDoorTrigger"/>
    public UnityEvent OnGoal; // invoked when an ball enters the goal
   

    void Awake()
    {
        audioSource = GetComponent<AudioSource>(); // initialize the SFX
    }

    void Update()
    {
        /* if a ball has been detected, forcifully move it to the center of the Ball Goal */
        if(ball)
            ball.transform.position = Vector3.MoveTowards(ball.transform.position, transform.position, 0.1f);
    }

    void OnTriggerEnter(Collider other)
    {
        /* if a ball is detected, set the ball to the detected object */
        if(!scored && other.CompareTag("Ball")) 
        {
            other.GetComponent<Rigidbody>().isKinematic = true; 
            ball = other.gameObject;
            scoreEffect.Play(); 
            audioSource.Play();
            OnGoal?.Invoke();
            scored = true; 
        }
    }
}
