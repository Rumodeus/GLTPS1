using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;
public class CharacterVehicleInteraction : MonoBehaviour
{
    [SerializeField]
    private PlayerMovement playerMovementScript;

    [SerializeField]
    private Animator vehicleAnim;
    [SerializeField]
    private Animator playerAnim;
    public CharacterController controller;
    [SerializeField]
    private NavMeshAgent agent;
    [SerializeField]
    private Transform child;
    [SerializeField]
    private Transform vehicleSeat;
    private bool enterable;
    private float steer;
    private float steerSmooth = 0f;
    [SerializeField]
    private float smoothInputSpeed = 0.2f;
    private float smoothInputVelocity = 0f;
    [SerializeField]
    private float rotationSpeed = 100.0f;
    public float elapsed = 0f;

    public bool preOrientEnter = false;
    public bool preOrientExit = false;
    private float maxSpeed = 4.0f;
    public bool constraint;

    [SerializeField]
    private Transform vehicleRightExit;

    [SerializeField]
    private Transform vehicleLeftExit;
    public bool exitRight;
    public bool exitLeft;
    [SerializeField]
    private float exitDuration = 0.25f;
    [SerializeField]
    private float enterDuration = 1.0f;


    void Start()
    {
        controller = GetComponent<CharacterController>();
        agent = GetComponent<NavMeshAgent>();
        playerMovementScript = GetComponent<PlayerMovement>();
        playerMovementScript.actions.Player.Interact.performed += Interact;
        playerMovementScript.actions.Vehicle.Exit.performed += Exit;


    }

    void Update()
    {

        if (agent.isActiveAndEnabled)
        {
            CharacterMovementAnimation.Movement(playerAnim, agent.velocity, maxSpeed);
        }
        if (constraint)
        {
            transform.position = vehicleSeat.position;
            transform.rotation = vehicleSeat.rotation;
        }

        if (preOrientEnter)
        {
            if (transform.position != vehicleSeat.position)
            {
                elapsed += Time.deltaTime;
                transform.position = Vector3.Lerp(transform.position, vehicleSeat.position, elapsed / enterDuration);
                transform.rotation = vehicleSeat.rotation;
            }
            else
            {
                preOrientEnter = false;
                elapsed = 0f;
            }

        }
        if (preOrientExit)
        {

            if (transform.position != vehicleRightExit.position && exitRight)
            {
                elapsed += Time.deltaTime;
                transform.position = Vector3.Lerp(vehicleSeat.position, vehicleRightExit.position, elapsed / exitDuration);
            }
            else
            {
                StartCoroutine(Wait(exitDuration));
            }

            if (transform.position != vehicleLeftExit.position && exitLeft)
            {
                elapsed += Time.deltaTime;
                transform.position = Vector3.Lerp(vehicleSeat.position, vehicleLeftExit.position, elapsed / exitDuration);
            }
            else
            {
                StartCoroutine(Wait(exitDuration));
            }
        }

        if (playerMovementScript.actions.Vehicle.Drive.enabled)
        {
            steer = playerMovementScript.actions.Vehicle.Drive.ReadValue<Vector2>().x;
            steerSmooth = Mathf.SmoothDamp(steerSmooth, steer, ref smoothInputVelocity, smoothInputSpeed);
            vehicleAnim.SetFloat("steer", steerSmooth);
            playerAnim.SetFloat("steer", steerSmooth);
        }

    }

    IEnumerator Wait(float duration)
    {
        yield return new WaitForSeconds(duration);
        Physics.IgnoreLayerCollision(6, 7, false);
        playerMovementScript.enabled = true;
        controller.enabled = true;
        preOrientExit = false;
        exitLeft = false;
        exitRight = false;
        elapsed = 0f;
    }

    public void Interact(InputAction.CallbackContext context)
    {
        if (context.performed && enterable
            && !playerAnim.GetCurrentAnimatorStateInfo(1).IsTag("Ket Seat Front")
            && !playerAnim.GetCurrentAnimatorStateInfo(1).IsTag("Ket Seat Back"))
        {
            //ignore collisions between layer 6 (vehicle) and layer 7 (player) 
            Physics.IgnoreLayerCollision(6, 7, true);

            //disable player movement
            playerMovementScript.enabled = false;
            //disable character controller
            controller.enabled = false;
            //enable navmesh agent 
            agent.enabled = true;
            preOrientEnter = false;

        }
        enterable = false;
    }

    public void Exit(InputAction.CallbackContext context)
    {
        if (context.performed && playerAnim.GetCurrentAnimatorStateInfo(1).IsName("Ket Steer"))
        {
            playerAnim.SetTrigger("ket exit");
        }
    }



    void OnTriggerEnter(Collider other)
    {
        enterable = true;
    }
    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Ket") && !preOrientEnter && !enterable)
        {
            //orient position to enter ket
            if (agent.isActiveAndEnabled)
            {
                agent.destination = other.transform.position;

                if (transform.position == agent.destination)
                {
                    //orient rotation to enter ket
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, other.gameObject.transform.rotation, rotationSpeed * Time.deltaTime);
                    child.rotation = Quaternion.RotateTowards(child.rotation, other.gameObject.transform.rotation, rotationSpeed * Time.deltaTime);
                    if (transform.rotation == other.gameObject.transform.rotation && child.rotation == other.gameObject.transform.rotation)
                    {
                        if (other.gameObject.name == "Right")
                        {
                            playerAnim.SetBool("ket right", true);
                        }
                        if (other.gameObject.name == "Left")
                        {
                            playerAnim.SetBool("ket left", true);
                        }
                        if (other.gameObject.name == "Back")
                        {

                        }
                        agent.enabled = false;
                        preOrientEnter = true;
                    }
                }
            }
        }

    }

    private void OnTriggerExit()
    {
        enterable = false;
    }

}
