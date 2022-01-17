using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Code by Mark Li Zonglin (2020)

/// <summary>
/// Attach this component to the player prefab in order for the player to contol it 
/// </summary>
public class Player : MonoBehaviour
{
    // ======== PUBLC VARIABLES ======= // 
    public static List<Player> players = new List<Player>(); // for referencing the other player characters
    public delegate void Action();
    public static event Action playerDeath; // invoked when the character is killed
    [Header("SFX Events")]
    public UnityEvent onCollision; // for invoking sound effects on collision 
    public ParticleSystem dust; // emitted when the character takes a step

    // ======== PRIVATE VARIABLES ====== // 
    private Rigidbody rb; // The character uses a Rigibody and CapsuleCollider rather than a CharacterController
    private Animator animator; 
    private CapsuleCollider m_collider;
    private float m_radius; // radius of the capsule collider

    void Awake()
    {
        /* Initialization */
        animator = transform.GetChild(0).GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        m_collider = GetComponent<CapsuleCollider>();
        m_radius = m_collider.radius;
    }

    void Start()
    {
        players.Add(this); // add the object itself to a list of all players
    }

    /// <summary>
    /// The main play loop for the character 
    /// </summary>
    void Update()
    {
        /* Make the character fall faster */
        AdditionalGravity();
        /* set the parameter in animator to sync the walking animation with the speed of its movement */
        animator.SetFloat("Speed", rb.velocity.magnitude);
        /* Kill the character if it fall below a certain height (-15 units) */
        if(LevelManager.instance && !LevelManager.instance.gameOver && transform.position.y < -15f)
            playerDeath?.Invoke();
        /* If the player is moving fast enough, make the dust emit a puff of white smoke */
        if(rb.velocity.magnitude > 1f)
        {
             var em = dust.emission;
             em.enabled = true;  
        }  
        else
        {
            var em = dust.emission;
             em.enabled = false;  
        }            
    }


    /// <summary>
    /// Moves and orients the character according to the direction provided
    /// This is called by <seealso cref="PlayerControl"/>, which handles the player input
    /// </summary>
    /// <param name="heading">the direction the charactter should be moving to</param>
    public void RotateAndMove(Vector3 heading)
    {
        if(heading.magnitude <= 0f)
        {
            rb.angularVelocity = Vector3.zero; // prevent the character from spinning around
            return;
        }
        Vector3 new_dir = Vector3.RotateTowards(transform.forward, heading, Time.fixedDeltaTime * 10f, 0f);
        transform.rotation = Quaternion.LookRotation(new_dir); 
        rb.AddForce(heading * 20f);
    }

    /// <summary>
    /// Called in Update()
    /// Applies additional downward force when the player is not grounded to make it fall faster
    /// </summary>
    private void AdditionalGravity()
    {
        RaycastHit[] hits = Physics.SphereCastAll(transform.position + new Vector3(0f, -0.1f, 0f), m_radius, Vector3.down, 1f);
        bool grounded = false; 
        foreach(RaycastHit hit in hits)
        {
            if(!hit.collider.CompareTag("Player"))
            {
                grounded = true; 
                return;
            }
        }
        if(!grounded) rb.velocity += new Vector3(0f, Physics.gravity.y * 2.5f * Time.deltaTime, 0f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // If the character makes contacts with another object while it moving fast, invoke onCollision
        // The AudioSource attached to the character subscribes to onCollision and plays a colliding sound effect when this occurs
        if(rb.velocity.magnitude > 1f)
            onCollision?.Invoke();
    }

}
