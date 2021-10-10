using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rbMove : MonoBehaviour
{
    public float speed = 100f;

    Rigidbody rb = new Rigidbody();
    Vector2 InputVelocity = Vector2.zero;

    //Serializefield exposes tagged variables, so you can access or view them in the inspector without making them public
    public float sensitivity = 10f;
    Camera cam = null;

    [SerializeField] AudioSource playerSoundSource = null;
    [SerializeField] GroundEnum.GroundType terrainType = GroundEnum.GroundType.Bush;
    [SerializeField] SoundPicker soundSource = null;
    bool moving = false;

    float storedY = 0f;
    float jumpForce = 5f;
    public bool isGrounded = true, falling = false;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        //Finds Rigidbody on gameobject this script is attached to
        rb = gameObject.GetComponent<Rigidbody>();

        //Finds the Main Camera in the scene via the following method:
            //cam = GameObject.FindWithTag("MainCamera")    This process can be quite taxing so never use Camera.main outside of a start or awake function
        cam = Camera.main;
    }

    //All Inputs for physics systems should run through Update, in order to ensure player inputs are not skipped
    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            isGrounded = false;
        }

        //Declare two local float variables and assign them as the raw values of the InputManager Axes Vertical and Horizontal
        float Horizontal = Input.GetAxisRaw("Horizontal");
        float Vertical = Input.GetAxisRaw("Vertical");

        //Multiply the Horizontal and Vertical axes by the gameobjects forward and right transforms, ensuring the forwards movement is always where the camera is pointing
        Vector2 xMove = new Vector2(Horizontal * transform.right.x, Horizontal * transform.right.z);
        Vector2 zMove = new Vector2(Vertical * transform.forward.x, Vertical * transform.forward.z);

        //Smoothes values to fit more of a circular shape, and assigns values accordingly
        InputVelocity = (xMove + zMove).normalized;

        //Assign new X and Y rotation floats to Raw Input Data from X and Y mouse axes, and multiply by a sensitivity value
        float yRot = Input.GetAxisRaw("Mouse X") * sensitivity;
        float xRot = Input.GetAxisRaw("Mouse Y") * sensitivity;

        //Rotate the PLAYERS Y-axis value and smooth with Time.deltaTime
        rb.rotation *= Quaternion.Euler(0, yRot * Time.deltaTime, 0);

        //Rotate the CAMERAS x-axis value and smooth with Time.deltaTime
        cam.transform.rotation *= Quaternion.Euler(-xRot * Time.deltaTime, 0, 0);

    }

    float timer = 0;
    //All Outcomes of physics systems (eg, movement) should be run inside of fixed update, as it runs on a time system, instead of frame by frame. This gives physics a far smoother outcome
    private void FixedUpdate()
    {
        //Multiply our previously assigned InputVelocity Vector and multiple by our speed variable. Smooth with Time.deltaTime
        InputVelocity = InputVelocity * speed * Time.deltaTime;
        
        //Assign the Rigidbody's velocity variable as the first value of InputVelocity (x), the rigidbodies current y velocity, and the second value of InputVelocity (y)
        if (InputVelocity != Vector2.zero && isGrounded)
        {
            rb.velocity = new Vector3(InputVelocity.x, rb.velocity.y, InputVelocity.y);
        }
        else if (!isGrounded)
        {
            if(storedY < transform.position.y)
            {
                falling = true;
            }
            if (falling)
            {
                timer += Time.deltaTime;
                rb.velocity = rb.velocity - Vector3.up * Time.deltaTime;
            }
            else
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        isGrounded = true;
        falling = false;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (falling)
        {
            isGrounded = true;
            falling = false;
        }
    }


}
