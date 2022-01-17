using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Code by Mark Li Zonglin (2020)

///<summary>
/// Attach this component to the FinishPoint prefab
/// Essentially, FinishPoint is the goal area that the player characters try to reach 
/// The game is won when all player characters reaches the FinishPoint
/// The prefab is shaped like pit such that characters can only enter the pit, but they cannot exit it
/// --- Tasks carried out in this class: ---
/// Set nextLevel on start;
/// Keep track of the number of players inside the finish area;
/// Invoke victory when all players are inside the finish area.
///</summary>
public class FinishPoint : MonoBehaviour
{
    private int playersInside = 0; // the number of player characters that have already reached the FinishPoint
    public delegate void Action();
    public static event Action victory; // invoked after all player characters have reached the FinishPoint

    public string nextLevel; // string representing the name of the scene to be loaded next 
    public ParticleSystem scoreEffect; // reference to the ParticleSystem component to be played when a character enters the FinishPoint
    private AudioSource audioSource; // Audio to be played when a character enters the FinishPoint

    void Start()
    {
        /* Initialization */ 
        if(LevelManager.instance)
            LevelManager.instance.nextLevel = nextLevel;
        audioSource = GetComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider other)
    {
        //Increment playersInside when a character enters FinishPoint, and invoke the victory event after all of them have entered the FinishPoint
        if (LevelManager.instance && LevelManager.instance.gameOver) return;
        if(other.CompareTag("Player"))
        {
            scoreEffect.Play();
            audioSource.Play();
            playersInside++;
            if(playersInside >= Player.players.Count)
                victory?.Invoke();
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Although unlike, in case a characters exits from the FinishPoint, decrement playersInside by one
        if (other.CompareTag("Player"))
            playersInside--;
    }

}
