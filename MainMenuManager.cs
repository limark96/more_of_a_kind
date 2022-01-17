using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// Code by Mark Li Zonlgin (2020)

/// <summary>
/// A script managing the main menu of the game. 
/// Has a number of functions to be called by UI events (e.g. button clicks)
/// Handles the logic for loading levels, level selection, and quitting
/// Stores references to the images used to preview the levels
/// </summary>
public class MainMenuManager : MonoBehaviour
{
    // =============== VARIABLES ============ // 
    public Image curtain; // manually set up the reference to the Image componenent used for crossfading 
    public Image previewImage; // Reference to the Image used in the level selection menu for previewing the level layout
    public Sprite[] previewSprites; // list of Sprites level previews


    private void Start()
    {
        Time.timeScale = 1f; // set time scale back to 1 cuz the game might be paused previously
        StartCoroutine(MoveCurtain(0f)); // raise the curtain. 
    }

    // =========== AUXILIARY FUNCTIONS ============= // 

    /// <summary>
    /// Loads a level according to the index with a crossfade transition
    /// </summary>
    /// <param name="index">the index representing the level to be loaded</param>
    public void LoadLevel(int index)
    {
        EventSystem.current.enabled = false; // disable current event system such that the player can no longer press another button after issuing the command
        StartCoroutine(MoveCurtain(1f, "Level_" + (index + 1)));
    }

    /// <summary>
    /// Set the Sprite for the level preview based on level selected
    /// The image gives the player an idea of what to expect in the level before entering it
    /// Invoked by UI events (buttons in the level select menu)
    /// </summary>
    /// <param name="index"></param>
    public void SetPreviewImage(int index)
    {
        previewImage.sprite = previewSprites[index];
    }

    /// <summary>
    /// Terminates the game. Invoked with button clicks from the "Quit" button in the main menu
    /// </summary>
    public void QuitGame()
    {
        Application.Quit(); 
    }


    /// <summary>
    /// To achieve the effect of fading in and fading out, an Panel colored in black is placed on the top of the Canvas 
    /// Increasing its opacity = fading out
    /// Decreasing its opacity = fading in 
    /// This is a helper routine  that gradually changes the opacity of the "curtain" over a period of 0.2 seconds
    /// </summary>
    /// <param name="alpha">The desired target opacity</param>
    /// <returns></returns>
    IEnumerator MoveCurtain(float alpha)
    {
        curtain.CrossFadeAlpha(alpha, 0.2f, true);
        yield return new WaitForSeconds(0.2f);
        yield return null;
    }

    /// <summary>
    /// This is a overloaded version of MoveCurtain where the coroutine will load a scene after reaching the desired opacity 
    /// </summary>
    /// <param name="alpha">The target opacity</param>
    /// <param name="sceneName">the name of the scene to be laoded after the fading is complete</param>
    /// <returns></returns>
    IEnumerator MoveCurtain(float alpha, string sceneName)
    {
        curtain.CrossFadeAlpha(alpha, 0.2f, true);
        yield return new WaitForSeconds(0.2f);
        SceneManager.LoadScene(sceneName);
        yield return null;
    }




}
