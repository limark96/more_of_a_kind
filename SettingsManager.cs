using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI; 

// Code by Mark Li Zonglin (2020)

/// <summary>
/// This script is attached to the settings menu
/// It handles the logic that allows the users to change various settings of the game
/// Including, SFX Volume, BGM Volume, Screen Resolution and Full Screen Mode 
/// </summary>
public class SettingsManager : MonoBehaviour
{
    // ---- References---- // 
    public AudioMixer masterMixer;  // Mnanually set up in the editor. The AudioMixer separate BGM and SFX and allows them to be adjusted separately
    public RectTransform bgmVolBar; // A rectangle representing the volume for BGM
    public RectTransform sfxVolBar; // A rectangle representing the volume for SFX
    public Dropdown resDropdown;    // Dropdown that allows the user to choose from various resolution options, such as 1270 x 720, 1920 x 1080, etc 
    public Dropdown fulDropdown;    // Dropdown that allows the user to choose from full screen and windowed mode. 
    
    // ---- Private Variables ---- // 
    private static float bgmVolRatio = 0.8f; 
    private static float sfxVolRatio = 0.2f; 

    // Start is called before the first frame update
    void Start()
    {
        // ---- Match the Settings Panels with the current state ---- //
        // Match volume
        masterMixer.GetFloat("bgmVol", out float bgmVol); 
        bgmVolBar.sizeDelta = new Vector2(bgmVol + 80f, 20f);        
        masterMixer.GetFloat("sfxVol", out float sfxVol); 
        sfxVolBar.sizeDelta = new Vector2(sfxVol + 80f, 20f);
        // Match Resolution
        resDropdown.value = GetResolutionIndex(Screen.width);
        // Match Full Screen 
        fulDropdown.value = Screen.fullScreen ? 0 : 1; 
    }

    /// <summary>
    /// Invoked by the OnClick() function in UIButtons 
    /// Allows UIButtons to adjust the BGM volumes
    /// </summary>
    /// <param name="increment">whether to increment or decrement the volume</param>
    public void ChangeBGMVolume(bool increment)
    {
        bgmVolRatio += increment ? 0.1f : -0.1f;
        bgmVolRatio = Mathf.Clamp(bgmVolRatio, 0f, 1f);
        masterMixer.SetFloat("bgmVol", bgmVolRatio <= 0f ? -80f : Mathf.Log10(bgmVolRatio) * 20);
        bgmVolBar.sizeDelta = new Vector2(bgmVolRatio * 100f, 20f); 
        // Note: when the volume is at -80Db, the audio is completely silent
    }

    /// <summary>
    /// Same function as ChangeBGMVolume except this one is for sound effects
    /// </summary>
    /// <param name="increment">whether to increment or decrement the volume</param>
    public void ChangeSFXVolume(bool increment)
    {
        sfxVolRatio += increment ? 0.1f : -0.1f;
        sfxVolRatio = Mathf.Clamp(sfxVolRatio, 0f, 1f);
        masterMixer.SetFloat("sfxVol", sfxVolRatio <= 0f ? -80f : Mathf.Log10(sfxVolRatio) * 20);
        sfxVolBar.sizeDelta = new Vector2(sfxVolRatio * 100f, 20f);
    }

    /// <summary>
    /// Invoked by UI events in the dropdown for resolutions
    /// Sets the resolution based on the choice made in the drop down
    /// </summary>
    /// <param name="value">an integer representing the resolution</param>
    public void SetResolution(int value)
    {
        // This cannot be tested during editor mode. Has to be tested in builds. 
        switch(value)
        {
            case 0:
                Screen.SetResolution(640, 360, Screen.fullScreen); // nHD
                break;
            case 1:
                Screen.SetResolution(1024, 576, Screen.fullScreen); // Youtube medium
                break;
            case 2:
                Screen.SetResolution(1280, 720, Screen.fullScreen); // HD
                break;
            case 3:
                Screen.SetResolution(1920, 1080, Screen.fullScreen); // Full HD
                break;
        }
    }

    /// <summary>
    /// An Auxiliary function that converts screen width to the corresponding index representing the screen resolution 
    /// It is to ensure that the dropdown actually displays the correct current resolution 
    /// </summary>
    /// <param name="width"></param>
    /// <returns></returns>
    private int GetResolutionIndex(int width)
    {
        if (width == 640)
            return 0;
        else if (width == 1024)
            return 1;
        else if (width == 1280)
            return 2;
        else if (width == 1920)
            return 3;
        else
            return 2; // default fall back
    }

    /// <summary>
    /// Invoked by thed dropdown in the menu for changing the screen mode
    /// Allow the user to change between full-screen and windowed mode 
    /// </summary>
    /// <param name="value">integer representing different screen mode</param>
    public void SetFullScreen(int value)
    {
        Screen.fullScreen = value == 0 ? true : false; 
    }


}
