using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class VechicleMovement : MonoBehaviour
{
    [SerializeField]
    private PlayerMovement playerMovementScript;
    private Rigidbody rb;
    [SerializeField]
    private Vector2 movement;
    [SerializeField]
    private float force;
    [SerializeField]
    private float maxVelocity;
    [SerializeField]
    private float torque;
    [SerializeField]
    private float maxAngularVelocity;
    [SerializeField]
    private Animator anim;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        //get input
        movement = playerMovementScript.actions.Vehicle.Drive.ReadValue<Vector3>();

        //set movement aniamtion
        MovementAnim();
    }
    void FixedUpdate()
    {
        //move
        if (Mathf.Abs(movement.y) > 0)
        {
            rb.AddForce(movement.y * transform.forward * force * Time.fixedDeltaTime, ForceMode.Force);

            //rotate
            if (Mathf.Abs(movement.x) > 0 && Mathf.Abs(movement.y) >= 0.5f)
            {
                rb.AddTorque(movement.x * transform.up * torque * Time.fixedDeltaTime, ForceMode.Force);
            }
        }
        //clamp velocity and angular velocity
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxVelocity);
        rb.angularVelocity = Vector3.ClampMagnitude(rb.angularVelocity, maxAngularVelocity);
    }

    /*
    movement animation is a blendtree with blendParameter binded directly to velocity.z of vertical input,
    with thresholds -1, 0, and 1 for backward, idle and forward states
    */
    private void MovementAnim()
    {
        //set movement animation
        anim.SetFloat("velocityZ", movement.y);
    }

}
