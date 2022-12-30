using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;
    private Animator anim;
    [SerializeField]
    private Transform child;
    [SerializeField]
    private Vector2 horizontalMovement;
    private Vector3 velocityXZ;
    private Vector3 velocityY;
    private float verticalMovement;

    [SerializeField]
    private bool isGrounded;
    private float speed;
    [SerializeField]
    private float walkSpeed = 2.0f;
    [SerializeField]
    private float runSpeed = 4.0f;
    [SerializeField]
    private float sprint = 0f;
    [SerializeField]
    private float gravity = -9.81f;
    [SerializeField]
    private float rotationSpeed = 600.0f;
    [SerializeField]
    Quaternion rotationDir;
    public InputActions actions;

    private float jumpHeight = 1.0f;

    [SerializeField]
    private Transform camera;


    void Awake()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();
        actions = new InputActions();
        actions.Player.Enable();
        actions.Player.Jump.performed += Jump;
    }

    void Update()
    {
        //get horizontal input
        horizontalMovement = actions.Player.Move.ReadValue<Vector2>();

        isGrounded = Physics.Raycast(transform.position, new Vector3(0, -1, 0), controller.skinWidth + 0.001f);
        //apply gravity
        if (isGrounded && verticalMovement < 0)
        {
            verticalMovement = 0f;
        }
        else
        {
            verticalMovement += gravity * Time.deltaTime;
        }

        //lerp speed values for blending walk/run animation states
        if (actions.Player.Sprint.ReadValue<float>() > 0)
        {
            speed = Mathf.Lerp(speed, runSpeed, 10.0f * Time.deltaTime);
        }
        else
        {
            speed = Mathf.Lerp(speed, walkSpeed, 10.0f * Time.deltaTime);
        }

        velocityXZ = Vector3.ClampMagnitude(new Vector3(horizontalMovement.x, 0, horizontalMovement.y), 1.0f) * speed;
        velocityXZ = transform.TransformDirection(velocityXZ);
        velocityY = new Vector3(0, verticalMovement, 0);

        //move character based on input
        controller.Move((velocityXZ + velocityY) * Time.deltaTime);

        //pass velocityXZ to drive movement animation
        CharacterMovementAnimation.Movement(anim, velocityXZ, runSpeed);        

        //rotate child in direction of movement      
        if (Vector3.Magnitude(velocityXZ) != 0)
        {
            Debug.Log("rotate");
            rotationDir = Quaternion.LookRotation(velocityXZ);
            child.rotation = Quaternion.RotateTowards(child.rotation, rotationDir, rotationSpeed * Time.deltaTime);
        }

        //rotate parent to camera
        if (Vector3.Magnitude(velocityXZ) != 0)
        {
            Vector3 cam = new Vector3(camera.TransformDirection(Vector3.forward).x, 0, camera.TransformDirection(Vector3.forward).z);
            Quaternion rotationDir = Quaternion.LookRotation(cam);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotationDir, rotationSpeed * Time.deltaTime);
        }

    }
    public void Jump(InputAction.CallbackContext context)
    {
        //jump
        if (context.performed && isGrounded)
        {
            verticalMovement = Mathf.Sqrt(jumpHeight * -3.0f * gravity);
        }
    }
}
