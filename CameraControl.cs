using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEditor;
using UnityEngine;

// Code by Mark Li Zonglin (2020)

/// <summary>
/// Data structure for different types of camera shots
/// CU = Close up
/// LS = Long Shot
/// ELS = Extreme Long Shot
/// </summary>
public enum CameraMode { CU, LS, ELS }

/// <summary>
/// Used in the "Common" Prefab
/// Attach this to Cam_CTRL, which is the parent object of the Camera 
/// Move the camera based on the positions of the players.
/// Since this allows the player to control multiple characters, the camera tries to track the centre point of the characters 
/// Orbit the camera based on player input. 
/// </summary>
public class CameraControl : MonoBehaviour
{
    public static CameraControl instance; // singleton reference
    private PlayerInputs inputs; // for using Unity's InputSystem
    private Vector2 input; // represents camera movement (left and right arrow keys)
    private Vector3 velocity = Vector3.zero; // how fast the camera should be moving 
    private float smoothTime = 0.3f; // duration in which the camera movement is dampened
    private Camera m_camera; // reference to the active camera object
    
    // --- Zoom control--- //  
    private CameraMode m_cameraMode = CameraMode.LS; // by default, the camera is set to 
    // Note: if the characters becomes futher apart from each other, the Camera Control will switch to a different camra CameraMode in order to zoom out 

    private float fov_velocity = 0f; 

    // --- FOV (field of view) --- //
    // Notes: these can be overriden in editor
    public float LS_FOV = 13.5f; // defines the FOV when m_cameraMode ise set to LS
    public float ELS_FOV = 20f;  // defines the FOV when m_cameraMode ise set to ELS

    void Awake()
    {
        /* Singleton pattern */
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        /* Set up inputs */
        inputs = new PlayerInputs();
        inputs.GamePlay.Camera.performed += ctx => input = ctx.ReadValue<Vector2>();
        inputs.GamePlay.Camera.canceled += ctx => input = Vector2.zero;
        /* Initialize the reference to the Camera object */
        m_camera = transform.Find("Camera").GetComponent<Camera>(); // assuming that the active camera is an immediate child of this object
    }

    void OnEnable()
    {
        inputs.Enable(); // enable player input for camra controls, allowing the user to pan the camera left and right using the arrow keys. 
        FinishPoint.victory += OnVictory; // Subscribe to this event in order to change the camera angle when the player completes the leveL
    }

    void OnDisable()
    {
        inputs.Disable();
        FinishPoint.victory -= OnVictory;
    }

    void Update()
    {
        if(Player.players.Count == 0)
            return;
        
        /* Move the camera to follow the playrs */
        var bounds = ComputeBounds(Player.players);
        transform.position = Vector3.SmoothDamp(transform.position, bounds.center, ref velocity, smoothTime);
        m_cameraMode = m_cameraMode == CameraMode.CU ? CameraMode.CU : ComputeCurrentCameraMode(bounds); // CU is where the player wins
        m_camera.fieldOfView = Mathf.SmoothDamp(m_camera.fieldOfView, ComputeFOV(m_cameraMode), ref fov_velocity, 0.3f);
        /* Panning the camera*/
        RotateCamera();
    }

    /// <summary>
    /// Pan the camera left to right depending on player's input 
    /// </summary>
    private void RotateCamera()
    {
        Vector3 rot = transform.eulerAngles;
        if(m_cameraMode == CameraMode.CU)
            rot.y += 0.3f * Time.deltaTime * 50f;    
        else
            rot.y += input.x * Time.deltaTime * 50f;    
        transform.eulerAngles = rot;
    }

    /// <summary>
    /// Determine the desired Camera Mode depending on how far apart the player characters are
    /// Basically, if the characters more than 8 Unity units are apart from each other, the CameraMode used will be ELS
    /// ELS has a larger FOV, which is more zoomed out and should prevent the any characters from leaving the screen 
    /// By contrast, if the characters are less than 8 Unity units apart from each other, then Camera mode will be set to LS
    /// LS has a smaller FOV, allowing the player to take a close look at the characters
    /// </summary>
    /// <param name="bounds">A bounding box that enloses all the player characters in the scene </param>
    /// <returns> the desired CameraMode depending on how away the characters are</returns>
    public CameraMode ComputeCurrentCameraMode(Bounds bounds)
    {
        float max = bounds.size.x; 
        if(bounds.size.y > max)
            max = bounds.size.x; 
        if(bounds.size.z > max)
            max = bounds.size.z; 
        if(max > 8f) // threshold for ELS
            return CameraMode.ELS;
        else
            return CameraMode.LS;
    }

    /// <summary>
    /// Computes a bound that encapsulates all the player characters
    /// </summary>
    /// <param name="objs">A list of active player characters</param>
    /// <returns>A bound that encapsulates all player characters</returns>
    Bounds ComputeBounds(List<Player> objs)
    {
        var bounds = new Bounds(objs[0].transform.position, Vector3.zero);
        for(int i = 1; i < objs.Count; i++)
            bounds.Encapsulate(objs[i].transform.position);
        return bounds;
    }

    
    /// <summary>
    /// An auxiliary function that converts CameraModes to the corresponding FOV value
    /// </summary>
    /// <param name="cameraMode">The CameraMode to be converted</param>
    /// <returns> Return the FOV to be used based on the current camera mode. </returns>
    public float ComputeFOV(CameraMode cameraMode){
        switch(cameraMode)
        {
            case CameraMode.CU:
                return 8f;
            case CameraMode.LS:
                return LS_FOV; 
            case CameraMode.ELS:
                return ELS_FOV; 
            default:
                return LS_FOV; // fallback 
        }
    }

    /// <summary>
    /// Called when the player clears a level.
    /// The camera zooms into the characters closely. 
    /// </summary>
    void OnVictory()
    {
        m_cameraMode = CameraMode.CU;
    }

}
