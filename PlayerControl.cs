using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// Code by Mark Li Zonglin (2020)

/// <summary>
/// Moving the players based on player input
/// </summary>
public class PlayerControl : MonoBehaviour
{
    public static PlayerControl instance; // singleton instance
    PlayerInputs inputs; // for using Unity's InputSystem
    Vector2 input = Vector2.zero; // vector representing the user input (ASWD keys or joystick)
    
    void Awake()
    {
        /* Initialization */
        Player.players.Clear(); // reset the static list of players
        inputs = new PlayerInputs();
        inputs.GamePlay.Movement.performed += ctx => input = (ctx.ReadValue<Vector2>());
        inputs.GamePlay.Movement.canceled += ctx => input = Vector2.zero;
        if (!instance) instance = this; else if (instance != this) Destroy(gameObject); // singleton pattern
    }

    void OnEnable()
    {
        inputs.GamePlay.Enable();
        /* Subscribe to relevant events  */
        Player.playerDeath += OnPlayerDeath;
        LevelManager.playerPause += OnPause;
        LevelManager.playerResume += OnResume;
    }

    void OnDisable()
    {
        inputs.GamePlay.Disable();
         /* Unsubsribe */
        Player.playerDeath -= OnPlayerDeath;
        LevelManager.playerPause -= OnPause;
        LevelManager.playerResume -= OnResume;
    }


    void FixedUpdate()
    {
        // Rotate and move the players
        if (LevelManager.instance && LevelManager.instance.gameOver) return; 
        Vector3 heading = ComputeHeading();
        foreach(Player player in Player.players)
            player.RotateAndMove(heading);
        
    }

    /// <summary>
    /// Returns the heading, which is the vector in which the player character should move towards
    /// </summary>
    /// <returns>the heading</returns>
    private Vector3 ComputeHeading()
    {
        // PlayerControl is attached to the parent of the Camera,
        // and transform.forward points to the direction in which the camera is oriented towards
        // the return value accounts for both the player input and the orientation of the camera
        Vector3 forward = transform.forward; 
        forward.y = 0f;
        forward = Vector3.Normalize(forward);
        Vector3 right = Quaternion.Euler(new Vector3(0, 90, 0)) * forward;
        return Vector3.Normalize(right * input.x + forward * input.y);
    }

    
    // ======== SUBSCRIBED FUNCTIONS ======= // 
    
    void OnPlayerDeath()
    {
        Player.players.Clear(); // reset player list to empty
    }
    void OnUnloadLevel()
    {
        Player.players.Clear();
    }

    void OnPause()
    {
        inputs.Disable();
    }
    void OnResume()
    {
        inputs.Enable();
    }


}
