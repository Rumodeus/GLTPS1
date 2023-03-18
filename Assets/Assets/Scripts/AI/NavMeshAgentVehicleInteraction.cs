using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class NavMeshAgentVehicleInteraction : MonoBehaviour
{
    [SerializeField]
    private NavMeshAgent agent;
    public Animator agentAnim;
    [SerializeField]
    private Collider collider;

    [SerializeField]
    private NavMeshAgentFollowPlayer navMeshAgentFollowPlayerScript;
    [SerializeField]
    private Transform enterPosition;
    [SerializeField]
    private Transform constraintPosition;
    [SerializeField]
    private PlayerMovement playerMovementScript;
    [SerializeField]
    public bool enter;
    public bool exit;
    private bool positionConstraint;
    public bool rotationConstraint;

    private bool preOrientEnter;
    public bool preOrientExit;
    [SerializeField]
    private float rotationSpeed = 600.0f;
    private float elapsed = 0f;
    [SerializeField]
    private float duration = 0.25f;

    void Update()
    {
        CharacterMovementAnimation.Movement(agentAnim, agent.velocity, playerMovementScript.runSpeed);

        if (enter && !preOrientExit)
        {
            navMeshAgentFollowPlayerScript.enabled = false;
            collider.enabled = false;

            //orient agent position to enter ket
            agent.destination = enterPosition.position;
            if (Vector3.Distance(transform.position, enterPosition.position) < 0.2f)
            {
                //orient rotation to enter ket
                transform.rotation = Quaternion.RotateTowards(transform.rotation, enterPosition.rotation, rotationSpeed * Time.deltaTime);
                if (transform.rotation == enterPosition.rotation)
                {
                    agentAnim.SetTrigger("ket back");
                    agent.enabled = false;
                    preOrientEnter = true;
                    enter = false;
                }
            }
        }

        if (preOrientEnter)
        {
            if (transform.position != constraintPosition.position)
            {
                elapsed += Time.deltaTime;
                transform.position = Vector3.Lerp(transform.position, constraintPosition.position, elapsed / duration);
            }
            else
            {
                positionConstraint = true;
                rotationConstraint = true;
                preOrientEnter = false;
                elapsed = 0f;
            }
        }

        if (exit)
        {
            positionConstraint = false;
            rotationConstraint = false;

            if (preOrientExit)
            {
                if (transform.position != enterPosition.position)
                {
                    elapsed += Time.deltaTime;
                    transform.position = Vector3.Lerp(transform.position, enterPosition.position, elapsed / duration);
                }
                else
                {
                    agent.enabled = true;

                    navMeshAgentFollowPlayerScript.enabled = true;
                    collider.enabled = true;
                    preOrientExit = false;
                    exit = false;

                    elapsed = 0f;
                }
            }

        }

        if (positionConstraint)
        {
            transform.position = constraintPosition.position;
        }
        if (rotationConstraint)
        {
            transform.rotation = constraintPosition.rotation;
        }
    }
}
