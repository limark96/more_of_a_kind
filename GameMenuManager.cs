using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// Code by Mark Li Zonglin (2020)

///<summary>
/// This component is attached to the Canvas
/// It hides and unhides the pause menu and the result screen based on certain events
/// Namely the pressing Esc to pause and when the player wins or loses a game
/// Note that this class isn't reponsible for processing events occured in the settings menu, which has it own deicated class
///</summary>
public class GameMenuManager : MonoBehaviour
{

    // Win Menu
    [Header("<Win Menu References>")]
    public GameObject WinMenuContainer;
    public Button WinNextButton;
    public Button WinPlayAgainButton; 
   
    // Lose Menu
     [Header("<Lose Menu References>")]
    public GameObject LoseMenuContainer;
    public Button LoseRetryButton;
    
    // Pause Menu
    [Header("<Pause Menu References>")]
    public GameObject PauseMenuContainer;
    public Button PauseRestartButton;
    public Button PauseContinueButton;

    public Button PauseButton; 

    void OnEnable()
    {
        // Subscribe to various events in the game
        FinishPoint.victory += OnVictory;
        Player.playerDeath += PlayerDeath;
        LevelManager.playerPause += OnPause;
        LevelManager.playerResume += OnResume;
    }

    void OnDisable()
    {
        // you must unsubscribe on disable otherwise it'll throw an error after scene transitions
        FinishPoint.victory -= OnVictory;
        Player.playerDeath -= PlayerDeath;
        LevelManager.playerPause -= OnPause;
        LevelManager.playerResume -= OnResume;
    }

    void Start()
    {
        WinPlayAgainButton.Select(); // This is also to prevent "Next" not visually appears to be selected despite actually being selected. 
    }

    // ======= EVENT HANDLERS ======== // 

    /// <summary>
    /// Wrapper function for _OnVictory()
    /// </summary>
    void OnVictory()
    {
        StartCoroutine(_OnVictory());
    }

    /// <summary>
    /// Invoked when the player wins a level
    /// Activates the results screen for winning
    /// </summary>
    /// <returns>The IEnumerator for the coroutine</returns>
    IEnumerator _OnVictory()
    {
        yield return new WaitForSeconds(0.3f);
        PauseButton.gameObject.SetActive(false);
        WinMenuContainer.SetActive(true);
        WinNextButton.Select();
        yield return null; 
    }


    /// <summary>
    /// Invoked when one of the player character dies (i.e. falls off the platforms)
    /// Activates the result screen for losing
    /// </summary>
    void PlayerDeath()
    {
        LoseMenuContainer.SetActive(true);
        PauseButton.gameObject.SetActive(false);
        LoseRetryButton.Select();
    }

    /// <summary>
    /// Called when the game is paused. 
    /// It activates the pause menu and hides the pause button
    /// </summary>
    void OnPause()
    {
        PauseMenuContainer.SetActive(true);
        PauseContinueButton.Select();
        PauseButton.gameObject.SetActive(false);
    }

    /// <summary>
    /// Called then when game is resumed from pausing 
    /// Hides the pause menu and reactivate the pause button
    /// </summary>
    void OnResume()
    {
        PauseRestartButton.Select();
        PauseMenuContainer.SetActive(false);
        PauseButton.gameObject.SetActive(true);
        /* 
            NOTE:
            If the continue button is selected when the player resumes (i.e. pause menu container de-activates),
            then in the next time the player pauses the game, although the continue button is functionally "selected", it won't visually appear to be selected.
            despite we explicitly call Select() in OnPause()
            However, if some other button is selected when the PauseMenu is deactivated, say, the restart button, then this phenomenon disappears. 
            So Select restart button in OnResume() is a wierd but more elegant fix to the problem I discovered when working on COMP3329.
        */
    }

}
