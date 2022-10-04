using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FirstPersonController : MonoBehaviour
{
    // References ...
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private CharacterController characterController;

    // Player settings
    [SerializeField] private float cameraSensitivity;
    [SerializeField] private float WalkSpeed;
    [SerializeField] private float RunSpeed;
    [SerializeField] private float moveInputDeadZone;
    [SerializeField] private float moveInputRunZone;


    // Touch detection
    private int leftFingerId, rightFingerId;
    private float halfScreenWidth;

    // Camera control
    private Vector2 lookInput;
    private float cameraPitch;

    // Player movement
    private Vector2 moveTouchStartPosition;
    private Vector2 moveInput;


    // jumping
    [Header("Gravity & Jumping")]
    public float StickToGroundForce = 10f;
    public float Gravity = 10f;
    private float VerticalVelocity;
    public float JumpForce=10f;

    public Transform groundCheck;
    public LayerMask groundLayers;
    public float GroundCheckRadious;
    private bool isGrounded;

    // walking sounds
    [Header("Walking & Running Sounds Effects")]
    public bool isWalking;
    public bool isRunning;
    [SerializeField] private AudioSource m_AudioSource;
    [SerializeField] private AudioClip Walksound;
    [SerializeField] private AudioClip Runsound;


    // jumping sound
    [Header("Jumping Sounds Effects")]
    [SerializeField] private AudioSource jump_audiosource;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip landingSound;


    //public Text testtxt;
    //public Text testtxt2;
    //public Text testtxt3;

    // Start is called before the first frame update
    void Start()
    {
        // id = -1 means the finger is not being tracked
        leftFingerId = -1;
        rightFingerId = -1;

        // only calculate once
        halfScreenWidth = Screen.width / 2;

        //// calculate the movement input dead zone
        moveInputDeadZone = Mathf.Pow(Screen.height / moveInputDeadZone, 2);
        moveInputRunZone = Mathf.Pow(Screen.height / moveInputRunZone, 2);


        isWalking = false;
        isRunning = false;
        m_AudioSource.mute = true;

    }

    // Update is called once per frame
    void Update()
    {
        // Handles input
        GetTouchInput();

        if (rightFingerId != -1)
        {
            // Ony look around if the right finger is being tracked
            Debug.Log("Rotating");
            LookAround();
        }

        if (leftFingerId != -1)
        {
            // Ony move if the left finger is being tracked
            Debug.Log("Moving");
            Move();
        }
        else
        {
            isWalking = false;
            isRunning = false;
        }


        VerticalMove();


        //Play Sounds
        if (isWalking == true && isRunning == false)
        {
            m_AudioSource.clip = Walksound;
            if (m_AudioSource.isPlaying == false)
            {
                m_AudioSource.Play();
            }
            m_AudioSource.mute = false;
        }
        else if (isRunning == true && isWalking == false)
        {
            m_AudioSource.clip = Runsound;
            if (m_AudioSource.isPlaying == false)
            {
                m_AudioSource.Play();
            }
            m_AudioSource.mute = false;
        }
        else
        {
            m_AudioSource.mute = true;
        }


    }


    void GetTouchInput()
    {
        // Iterate through all the detected touches
        for (int i = 0; i < Input.touchCount; i++)
        {

            Touch t = Input.GetTouch(i);

            // jump
            if (t.tapCount ==2)
            {
                Jump();
                //testtxt3.text = "Double Clicked!";
            }
            
            // Check each touch's phase
            switch (t.phase)
            {
                case TouchPhase.Began:

                    if (t.position.x < halfScreenWidth && leftFingerId == -1)
                    {
                        // Start tracking the left finger if it was not previously being tracked
                        leftFingerId = t.fingerId;

                        // Set the start position for the movement control finger
                        moveTouchStartPosition = t.position;
                    }
                    else if (t.position.x > halfScreenWidth && rightFingerId == -1)
                    {
                        // Start tracking the rightfinger if it was not previously being tracked
                        rightFingerId = t.fingerId;
                    }

                    break;
                case TouchPhase.Ended:
                case TouchPhase.Canceled:

                    if (t.fingerId == leftFingerId)
                    {
                        // Stop tracking the left finger
                        leftFingerId = -1;
                        Debug.Log("Stopped tracking left finger");
                    }
                    else if (t.fingerId == rightFingerId)
                    {
                        // Stop tracking the right finger
                        rightFingerId = -1;
                        Debug.Log("Stopped tracking right finger");
                    }

                    break;
                case TouchPhase.Moved:

                    // Get input for looking around
                    if (t.fingerId == rightFingerId)
                    {
                        lookInput = t.deltaPosition * cameraSensitivity * Time.deltaTime;
                    }
                    else if (t.fingerId == leftFingerId)
                    {

                        // calculating the position delta from the start position
                        moveInput = t.position - moveTouchStartPosition;
                    }

                    break;
                case TouchPhase.Stationary:
                    // Set the look input to zero if the finger is still
                    if (t.fingerId == rightFingerId)
                    {
                        lookInput = Vector2.zero;
                    }
                    break;
            }
        }
    }

    void LookAround()
    {

        // vertical (pitch) rotation
        cameraPitch = Mathf.Clamp(cameraPitch - lookInput.y, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(cameraPitch, 0, 0);

        // horizontal (yaw) rotation
        transform.Rotate(transform.up, lookInput.x);
    }

    void Move()
    {

        // Don't move if the touch delta is shorter than the designated dead zone
        if (moveInput.sqrMagnitude <= moveInputDeadZone)
        {
            isWalking = false;
            isRunning = false;
           // testtxt.text = "DEAD" + "   " + moveInput.sqrMagnitude;
        }
        else if ( moveInput.sqrMagnitude >= moveInputRunZone) // run if touch is larger than run zone
        {
            // Multiply the normalized direction by the speed
            Vector2 movementDirection = moveInput.normalized * RunSpeed * Time.deltaTime;
            // Move relatively to the local transform's direction
            characterController.Move(transform.right * movementDirection.x + transform.forward * movementDirection.y);

            if (isGrounded)
            {
                isWalking = false;
                isRunning = true;
            }
            else
            {
                isWalking = false;
                isRunning = false;
            }

          //  testtxt.text = "Run" + "   " + moveInput.sqrMagnitude;
        }
        else //if (moveInputDeadZone < moveInput.sqrMagnitude && moveInput.sqrMagnitude < moveInputRunZone)   // Walk if the touch is between dead and run zone
        {
            // Multiply the normalized direction by the speed
            Vector2 movementDirection = moveInput.normalized * WalkSpeed * Time.deltaTime;
            // Move relatively to the local transform's direction
            characterController.Move(transform.right * movementDirection.x + transform.forward * movementDirection.y);

            if (isGrounded)
            {
                isWalking = true;
                isRunning = false;
            }
            else
            {
                isWalking = false;
                isRunning = false;
            }

           // testtxt.text = "Walk" + "   " + moveInput.sqrMagnitude;
        }

       // testtxt2.text = "Dead:" + moveInputDeadZone + "///     Run:" + moveInputRunZone;
    }

    bool isShooted = false;

    private void FixedUpdate()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, GroundCheckRadious, groundLayers);

        if (isGrounded== true && isShooted==true)
        {
            //just reached the ground
            //play landing sound
            jump_audiosource.clip = landingSound;
            jump_audiosource.Play();


            //unmute walk sound
            //mute walking sound
            m_AudioSource.mute = false;


            isShooted = false;
        }

      //  testtxt.text = "is grounded??? " + isGrounded.ToString();
    }

    private void VerticalMove()
    {

        if (isGrounded && VerticalVelocity<=0)
        {
            VerticalVelocity = -StickToGroundForce * Time.deltaTime;
        }
        else
        {
            VerticalVelocity -= Gravity * Time.deltaTime;
        }

        Vector3 verticalMovement = transform.up * VerticalVelocity;
        characterController.Move(verticalMovement * Time.deltaTime);
    }

    public void Jump()
    {
        if (isGrounded)
        {
            VerticalVelocity = JumpForce;
            //testtxt2.text = "velocity::: " + VerticalVelocity.ToString();

            //mute walking sound
            m_AudioSource.mute = true;


            //play jump sound
            jump_audiosource.clip = jumpSound;
            jump_audiosource.Play();


            isShooted = true;
        }
        
    }
   

}
