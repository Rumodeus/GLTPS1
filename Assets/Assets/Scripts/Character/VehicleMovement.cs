using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class VehicleMovement : MonoBehaviour
{
    [SerializeField]
    private CharacterManager characterManagerScript;
    private Rigidbody rb;
    [SerializeField]
    private Vector2 movement;

    [SerializeField]
    private float currentMovementRate;
    [SerializeField]
    private float increasedMovementRate;
    [SerializeField]
    private float force;
    [SerializeField]
    private float maxVelocity;

    private float scaledMaxVelocity;
    [SerializeField]
    private float torque;
    [SerializeField]
    private float maxAngularVelocity;
    [SerializeField]
    private Animator anim;

    [SerializeField]
    private bool isGrounded;
    [SerializeField]
    private Vector3 boxCastHalfExtents;
    [SerializeField]
    private Vector3 boxCastOffset;

    LayerMask layerMask;

    [SerializeField]
    VehicleFuelManager vehicleFuelManagerScript;
    [SerializeField]
    private SkinnedMeshRenderer ketBarrelMesh;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        //layerMask ignore player, vehicle, and ai
        layerMask = 1 << 6 | 1 << 7;
        layerMask = ~layerMask;
    }

    void Update()
    {
        //get input
        movement = characterManagerScript.PlayerInputInitialize.actions.Vehicle.Drive.ReadValue<Vector2>();

        //set speed
        if (characterManagerScript.PlayerInputInitialize.actions.Vehicle.Accelerate.IsPressed())
        {
            currentMovementRate = increasedMovementRate;
        }
        else
        {
            currentMovementRate = 1;
        }
        scaledMaxVelocity = maxVelocity * currentMovementRate;

        //set movement aniamtion
        MovementAnim();
    }
    void FixedUpdate()
    {
        isGrounded = Physics.CheckBox(transform.position + boxCastOffset, boxCastHalfExtents, transform.rotation, layerMask, QueryTriggerInteraction.Ignore);

        //move
        if (Mathf.Abs(movement.y) > 0 && isGrounded && vehicleFuelManagerScript.currentFuel > 0 && ketBarrelMesh.enabled)
        {
            rb.AddForce(movement.y * transform.forward * force * currentMovementRate * Time.fixedDeltaTime, ForceMode.Force);

            //rotate
            if (Mathf.Abs(movement.x) > 0)
            {
                if (movement.y >= 0.5f)
                {
                    rb.AddTorque(movement.x * transform.up * torque * Time.fixedDeltaTime, ForceMode.Force);
                }
                if (movement.y <= -0.5f)
                {
                    rb.AddTorque(-movement.x * transform.up * torque * Time.fixedDeltaTime, ForceMode.Force);
                }
            }
        }
        //clamp velocity and angular velocity
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, scaledMaxVelocity);
        rb.angularVelocity = Vector3.ClampMagnitude(rb.angularVelocity, maxAngularVelocity);
    }

    /*
    movement animation is a blendtree with blendParameter binded directly to rb.velocity scaled between 0 and 1,
    with thresholds -1, 0, and 1 for backward, idle and forward states
    */
    private void MovementAnim()
    {
        float scaledVelocityXZ = Vector3.Magnitude(rb.velocity) / maxVelocity;
        scaledVelocityXZ = scaledVelocityXZ * movement.y;

        //set movement animation
        anim.SetFloat("velocityZ", scaledVelocityXZ);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(transform.position + boxCastOffset, boxCastHalfExtents * 2);
    }
}
