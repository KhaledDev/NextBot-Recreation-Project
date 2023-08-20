using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

/// <summary>
/// ------------------------------------------------------------------------------------
/// This Movement script is from the youtuber: "Dave / GameDevelopment". 
/// Channel: 'https://www.youtube.com/@davegamedevelopment'
/// This MovementScript video: 'https://www.youtube.com/watch?v=f473C43s8nE'
/// second part of the movement video: 'https://www.youtube.com/watch?v=xCxSjgYTw9c'
/// ------------------------------------------------------------------------------------
/// I recommened watching These video's to understand The movementscript.
/// This movement script is really good and I have been using it in most of my games.
/// 
/// I just modified the script a bit for when you jump it will remove the Speed limiter.
/// Also when pressing the keypad numbers enable the desired bot from the bot id_num from photon.
/// The parts I modified will be Commented.
/// </summary>

public class PlayerMovement : MonoBehaviourPunCallbacks
{
    [Header("Movement")]
    private float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;

    public float groundDrag;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump = true;

    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchYScale;
    float startYSacle;

    [Header("KeyBinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope = true;

    public Transform orientation;
    public GameObject CameraHolder;
    public GameObject[] Bots;
    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    public MovementState state;
    public enum MovementState
    {
        walking,
        sprinting,
        crouching,
        air
    }

    private void Start()
    {

        CameraHolder.SetActive(photonView.IsMine);

        if (!photonView.IsMine) return;
        

        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        startYSacle = transform.localScale.y;
    }

    private void Update()
    {
        if (!photonView.IsMine) return;
        //GroundCheck
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        MyInput();
        SpeedControl();
        StateHandler();

        //Handle Drag
        if (grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;

        /// <summary>
        /// Checks if the player pressed on of these numpad keys and enables the bot according to their photonview id
        /// </summary>
        if (Input.GetKeyDown("[1]"))
        {
            photonView.RPC("botEnable", RpcTarget.AllBuffered, 100);
        }

        if (Input.GetKeyDown("[2]"))
        {
            photonView.RPC("botEnable", RpcTarget.AllBuffered, 101);
        }

        if (Input.GetKeyDown("[3]"))
        {
            photonView.RPC("botEnable", RpcTarget.AllBuffered, 102);
        }

        if (Input.GetKeyDown("[4]"))
        {
            photonView.RPC("botEnable", RpcTarget.AllBuffered, 103  );
        }
    }

    private void FixedUpdate()
    {
        if (!photonView.IsMine) return;
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        //When to jump
        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            Jump();

            readyToJump = false;

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        //Start Crouch
        if (Input.GetKeyDown(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }

        //Stop Crouch
        if (Input.GetKeyUp(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, startYSacle, transform.localScale.z);
        }
    }

    void StateHandler()
    {
        //Mode - Sprinting
        if (grounded && Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
        }

        //Mode - Walking
        else if (grounded)
        {
            state = MovementState.walking;
            moveSpeed = walkSpeed;
        }

        //Mode - air
        else
        {
            state = MovementState.air;
        }

        //Mode - Crouching
        if (Input.GetKey(crouchKey))
        {
            state = MovementState.crouching;
            moveSpeed = crouchSpeed;
        }

    }

    void MovePlayer()
    {
        //Calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        //On slope
        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection() * moveSpeed * 20f, ForceMode.Force);

            if (rb.velocity.y > 0)
            {
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
                
            }
        }

        //On Ground
        if (grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        else if (!grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }

        //Turn off gravity on slope
        rb.useGravity = !OnSlope();
    }

    void SpeedControl()
    {

        //Limiting speed on slope
        if (OnSlope() && !exitingSlope)
        {
            if (rb.velocity.magnitude > moveSpeed)
            {
                rb.velocity = rb.velocity.normalized * moveSpeed;
            }
        }
        //Limiting speed on ground or in air
        else
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0, rb.velocity.z);

            //Limit velocuty if needed
            if (flatVel.magnitude > moveSpeed)
            {
                if(state != MovementState.air)
                {
                    Vector3 limitedVel = flatVel.normalized * moveSpeed;
                    rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
                }
                else
                {
                    /// <summary>
                    /// Here is also what i modified when the player is in the air it, Increases the speedlimit.
                    /// </summary>
                    
                    Vector3 limitedVel = flatVel.normalized * moveSpeed * 1.2f;
                    rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
                }        
            }
        }


    }

    void Jump()
    {
        exitingSlope = true;

        //reset y vel
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        rb.AddForce(moveDirection.normalized * 300f, ForceMode.Force);
    }

    void ResetJump()
    {
        readyToJump = true;

        exitingSlope = false;
    }

   
    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }


        return false;
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }
    
    
    /// <summary>
    /// This is the botEnable function. you input the number of the photonview of the bot.
    /// and it will search for it and enable it or disable it depending on it's situation.
    /// </summary>
    [PunRPC]
    public void botEnable(int PhotonNum)
    {

        PhotonView enable = PhotonView.Find(PhotonNum);

        if (!enable.transform.gameObject.activeInHierarchy)
        {
            enable.transform.gameObject.SetActive(true);
        }
        else
        {
            enable.transform.gameObject.SetActive(false);
        }

    }

}
