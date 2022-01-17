using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Code by Mark Li Zonglin (2020)

/// <summary>
/// A script written for a prefab called "Switch"
/// It is basically a pressure-plate that the player can step on to trigger certain events
/// It can either be "holdable", meaning that the switch is reset after the player gets off it 
/// Or it can be "un-hodable", meaning that it will remained pushed down even after the player has got off it  
/// </summary>
public class Switch : MonoBehaviour
{
    [Tooltip("Whether the switch is only activated when there's a player standing on top of it.")] 
    public bool holdable = false;      // manually set up in Editor. Whether the switch should be holdable 
    public UnityEvent onPressed;       // invoked when the switch is pressed
    public UnityEvent onReleased;      // invoked when the switch is released
    private int numOfPlayerInside = 0; // number of player currently inside the collider (trigger).
    private bool activated = false;    // whether the switch is currently activated 
    Animator animator;                 // reference for the Animator component that plays the animation for the switch 
    AudioSource audioSource;           // reference for the AudioSource component that plays a sound when the player interacts with the switch 

    // Start is called before the first frame update
    void Start()
    {
        /* Initialization */
        numOfPlayerInside = 0;
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider other)
    {
        /* For when the player gets on top of the switch */
        if(other.CompareTag("Player"))
        {
            if((!holdable && !activated) || (holdable && numOfPlayerInside <= 0))
            {
                animator.SetTrigger("Down");
                activated = true;
                onPressed?.Invoke();
                audioSource.Play();
            }
            if(holdable)
                numOfPlayerInside++;
        }
    }

    void OnTriggerExit(Collider other)
    {
        /* For when the player gets off the switch */
        if(holdable && other.CompareTag("Player"))
        {
            numOfPlayerInside--;
            if(numOfPlayerInside <= 0)
            {
                onReleased?.Invoke();
                audioSource.Play();
                animator.SetTrigger("Up");
            }
        }
    }
}
