using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// Code by Mark Li Zonglin (2020)

/// <summary>
/// Handles winning, losing, level loading, and pausing
/// </summary>
public class LevelManager : MonoBehaviour
{
    public static LevelManager instance; // the singleton instance of the class
    [HideInInspector] public string nextLevel; // Set by the FinishPoint class on Start(). 
    [HideInInspector] public bool gameOver = false; // keeps track of whether the game is over 
    
    // Pausing
    PlayerInputs inputs; // for using Unity's new InputSystem 
    private bool paused = false; // whether the game is currently paused
    public delegate void Action();
    public static event Action playerPause; // invoked whenever the player pauses the game
    public static event Action playerResume; // invoked whenver the player unpauses the game

    public Image curtain; // manual-setup. This is an Image object in the Canvas used fading in and fading out

    void Awake()
    {
        if (!instance) instance = this; else if (instance != this) Destroy(gameObject); // singleton pattern
        /* Set up player input */
        inputs = new PlayerInputs();
        inputs.GamePlay.Pause.performed += ctx => Pause();
    }

    void Start()
    {
        /* Fade-in from black */
        curtain.CrossFadeAlpha(0f, 1f, true);
        /* Initialize game states */
        Time.timeScale = 1f; 
        paused = false; 
        gameOver = false; 
    }

    void OnEnable()
    {
        inputs.Enable(); // enable player input
        /* Subscribe to relevant events */
        Player.playerDeath += OnPlayerDeath; // called when a player dies
        FinishPoint.victory += OnVictory; // called when victory is achieved, as determined by FinishPoint
    }

    // called when the scene is destroyed
    void OnDisable()
    {
        inputs.Disable(); 
        /* Unsubscribe to the event to avoid errors */
        Player.playerDeath -= OnPlayerDeath;
        FinishPoint.victory -= OnVictory;
    }

    // called when player pauses the game
    public void Pause()
    {
        Time.timeScale = paused ? 1f : 0f;
        if(paused)
            playerResume?.Invoke();
        else
            playerPause?.Invoke();
        paused = !paused;
    }


    // ======== LOADING SCENES  ======= // 

    // Called Externally by UI. When the "retry" button is pressed in pause menu or result screen
    public void LoadCurrentLevel()
    {
        StartCoroutine(_LoadLevel(SceneManager.GetActiveScene().name)); // reload current scene
    }

    // Called externally by UI. When player presses "next" in the result screen
    public void LoadNextLevel()
    {
        StartCoroutine(_LoadLevel(nextLevel));
    }

    // Called externally by UI. When the player presses "Quit" in the pause menu
    public void Quit()
    {
        StartCoroutine(_LoadLevel("MainTitle"));
    }

    // Invoked by the avoid 
    IEnumerator _LoadLevel(string sceneName)
    {
        EventSystem.current.enabled = false; // disable event system to prevent further commands from being issued.
        inputs.Disable(); // disable player inputs to prevent pausing again. 
        curtain.CrossFadeAlpha(1f, 1f, true);
        yield return new WaitForSecondsRealtime(1f);
        SceneManager.LoadSceneAsync(sceneName);
        yield return null;
    }



    // =========== Subscribed functions ============ //

    void OnPlayerDeath()
    {
        gameOver = true;
        inputs.Disable();
    }

    void OnVictory()
    {
        gameOver = true;
        inputs.Disable();
    }
    
}
