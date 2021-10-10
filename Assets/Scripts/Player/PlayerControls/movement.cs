using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class movement : MonoBehaviour
{
    public bool crouchToggledOn = false, sprintToggledOn = false;

    public bool crouching = false, sprinting = false;

    public enum moveForm { baseMove, crouch, sprint}
    public moveForm move = moveForm.baseMove;

    public Vector3 movementValues = Vector3.zero;
    public Vector2 MovementInputVals = Vector2.zero;
    Vector2 lookValues = Vector2.zero;

    float walkSpeed = 10f;
    float sprintingSpeed = 25f, crouchingSpeed = 4f;

    bool inputPrimer = true;

    [SerializeField] float mouseSensitivity = 100f;

    Camera cam = null;
    Rigidbody rb = null;

    public bool isGrounded = true, falling = false;
    public float jumpForce = 1f;
    public float roofVal = 3f;



    void Start()
    {
        rb = GetComponent<Rigidbody>();

        cam = Camera.main;
    }

    //Receive movement-related inputs
    public void DetectMove(CallbackContext context)
    {
        MovementInputVals = context.ReadValue<Vector2>();
    }
    //receive looking-related inputs
    public void DetectLook(CallbackContext context)
    {
        lookValues = context.ReadValue<Vector2>();
    }
    public void DetectCrouchingKey(CallbackContext context)
    {
        switch (crouchToggledOn)
        {
            //Crouching Toggle check:
                //Toggle for crouch
            case true:
                if (context.started)
                {
                    inputPrimer = false;
                    crouching = !crouching;
                    sprinting = false;
                    SetMoveState();
                }
                if (context.canceled)
                {
                    inputPrimer = true;
                }
                break;

                //Hold for crouch
            case false:
                if (context.started)
                {
                    inputPrimer = false;
                    crouching = true;
                    sprinting = false;
                    SetMoveState();
                }
                if (context.canceled)
                {
                    inputPrimer = true;
                    crouching = false;
                    SetMoveState();
                }
                break;
        }
    }
    public void DetectSprintingKey(CallbackContext context)
    {
        //Check if sprint toggle enabled/disabled
        switch (sprintToggledOn)
        {
            //toggle for sprint
            case true:
                if (context.started)
                {
                    inputPrimer = false;
                    sprinting = !sprinting;
                    crouching = false;
                    SetMoveState();
                }
                if (context.canceled)
                {
                    inputPrimer = true;
                }
                break;

            //hold for sprint
            case false:
                if (context.started)
                {
                    inputPrimer = false;
                    sprinting = true;
                    crouching = false;
                    SetMoveState();
                }
                if (context.canceled)
                {
                    inputPrimer = true;
                    sprinting = false;
                    SetMoveState();
                }
                break;
        }
    }
    public void DetectJump(CallbackContext context)
    {
        if (context.started & inputPrimer)
        {
            inputPrimer = false;
            if (isGrounded)
            {
                JumpFunc();
                isGrounded = false;
            }
        }
        if (context.canceled)
        {
            inputPrimer = true;
        }
    }

    void Update()
    {
        //Create 2 new Vector 2, single axis components to find forward and move in direction
        Vector2 xMoveValues = new Vector2(MovementInputVals.x * transform.right.x, MovementInputVals.x * transform.right.z);
        Vector2 zMoveValues = new Vector2(MovementInputVals.y * transform.forward.x, MovementInputVals.y * transform.forward.z);

        //Normalize incoming composite axes and combine into a new Vector2
        Vector2 MovementDirection = (xMoveValues + zMoveValues).normalized;
        //Assign Movement Values with the x and y axes from Vector2 as x and z for movement vector3
        movementValues = new Vector3(MovementDirection.x, 0, MovementDirection.y);


        //Run CameraFunctions
        LookFunc();
    }

    
    void SetMoveState()
    {
        //Set the movement state between crouching, sprinting or walking
        if (crouching)
        {
            move = moveForm.crouch;
        }
        else if (sprinting)
        {
            move = moveForm.sprint;
        }
        else
        {
            move = moveForm.baseMove;
        }
    }

    float timer = 0f;
    private void FixedUpdate()
    {
        //Create a local float for movement speed
        float moveSpeedVal = 0;
        switch (move)
        {
            //Assign movement speed as walk, crouch or sprint based on state
            case moveForm.baseMove:
                moveSpeedVal = walkSpeed;
                break;
            case moveForm.crouch:
                moveSpeedVal = crouchingSpeed;
                break;
            case moveForm.sprint:
                moveSpeedVal = sprintingSpeed;
                break;
        }
        //Run movement functions and pass through desired movement speed
        MoveFunc(moveSpeedVal);
    }

    //Variables strictly for movement
    Vector3 storedVel = Vector3.zero;
    float storedY = 0;
    float haltTime = 0f;
    float haltDamping = 0f;

    void MoveFunc(float moveSpeed)
    {
        if (movementValues.magnitude > 1)
        {
            //if magnitude of movement values > 1, normalize them
            movementValues = movementValues.normalized;
        }

        if (MovementInputVals != Vector2.zero && isGrounded)
        {
            //Set haltTime (Stopping time on lerp) = 0. Set rb velocity to movementvalue * movespeed, and add the previous frames storedvelocity value for smoother turning.
            haltTime = 0;
            rb.velocity = (movementValues * moveSpeed) + storedVel;

            //While moving, Increase haltDamping value to a maximum of 2 (gives a slight slide)
            haltDamping += Time.deltaTime;
            haltDamping = Mathf.Clamp(haltDamping, 1f, 2f);
        }
        else if (!isGrounded)
        {
            
            if (storedY < transform.position.y || transform.position.y > roofVal)
            {
                falling = true;
                rb.velocity = (new Vector3(movementValues.x * moveSpeed, rb.velocity.y, movementValues.z * moveSpeed) + storedVel);
            }
            else
            {
                //rb.velocity = (new Vector3(movementValues.x * moveSpeed, -rb.velocity.y, movementValues.z * moveSpeed) + storedVel);
            }

            if (falling)
            {
                timer += Time.deltaTime;
                movementValues = new Vector3(movementValues.x * moveSpeed, -1 * timer, movementValues.z * moveSpeed);
                print(movementValues);
            }
            else
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
        }
        else
        {
            timer = 0;
            //when no input values incoming, Velocity reduces to zero over 1-2 seconds (dependant on haltDamping).  Resets haltDamping if velocity reaches 0
            haltTime += Time.deltaTime / ((int)haltDamping);
            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, haltTime);
            if (rb.velocity == Vector3.zero)
            {
                haltDamping = 0f;
            }
        }
        //Gets a 10th of current frames velocity
        storedVel = rb.velocity * 0.1f;
        storedY = gameObject.transform.position.y;

        //if moving faster than speed permitted, cap and reduce to moveSpeed value;
        if (rb.velocity.magnitude > moveSpeed)
        {
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, moveSpeed);
        }
    }

    void LookFunc()
    {
        //Get X and Y rotation from Inputs and multiply by sensitivty value
        float xRot = lookValues.y * mouseSensitivity / 1.5f;
        float yRot = lookValues.x * mouseSensitivity;

        //rotate player body around Y-axis
        rb.rotation *= Quaternion.Euler(0, yRot * Time.deltaTime, 0);
        //rotate camera around X-axis
        cam.transform.rotation *= Quaternion.Euler(-xRot * Time.deltaTime, 0, 0);
    }

    private void OnCollisionEnter(Collision collision)
    {
        isGrounded = true;
        falling = false;
    }
    void OnCollisionStay(Collision col)
    {
        if(falling)
        {
            falling = false;
            isGrounded = true;
        }
    }

    public void JumpFunc()
    {
        isGrounded = false;
    }


}
