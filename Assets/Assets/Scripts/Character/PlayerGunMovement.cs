using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Animations;
using Cinemachine;
using UnityEngine.UI;
public class PlayerGunMovement : MonoBehaviour
{
    [SerializeField] private PlayerInputInitialize playerInputScript;
    [SerializeField] private PlayerMovement playerMovementScript;
    [SerializeField] private Animator anim;
    [SerializeField] private Transform gun;
    [SerializeField] private Transform backSocket;
    [SerializeField] private Transform frontSocket;
    private bool flagEquip = false;

    [SerializeField] private Transform camera;
    [SerializeField] private Transform spineBone;

    [SerializeField] private Vector3 CMFollowTargetOffsetPos;
    [SerializeField] private Vector3 CMLookAtTargetOffsetPos;
    [SerializeField] private Vector3 CMFollowTargetOffsetPosRight;
    [SerializeField] private Vector3 CMLookAtTargetOffsetPosRight;
    [SerializeField] private Vector3 CMFollowTargetOffsetPosLeft;
    [SerializeField] private Vector3 CMLookAtTargetOffsetPosLeft;
    [SerializeField] private Transform CMFollowTarget;
    [SerializeField] private Transform CMLookAtTarget;
    private Vector3 CMFollowTargetInitialLocalPos;
    private Vector3 CMLookAtTargetInitialLocalPos;
    [SerializeField] private Transform child;

    [SerializeField] private GameObject crosshair;

    [SerializeField] private CinemachineFreeLook freeLookCamera;
    [SerializeField] SetCameraSpeed setCameraSpeedScript;

    void Start()
    {
        CMFollowTargetInitialLocalPos = new Vector3(0, CMFollowTarget.transform.position.y, 0);
        CMLookAtTargetInitialLocalPos = new Vector3(0, CMLookAtTarget.transform.position.y, 0);
    }

    void Update()
    {
        if (playerInputScript.actions.Player.ADS.ReadValue<float>() > 0f && Time.timeScale == 1)
        {
            playerMovementScript.ADS = true;

            //rotate child to camera      
            child.rotation = Quaternion.Euler(0f, camera.rotation.eulerAngles.y, 0f);
            //rotate parent to camera
            transform.rotation = Quaternion.Euler(0f, camera.rotation.eulerAngles.y, 0f);

            //set player ADS animation
            if (flagEquip)
            {
                //set camera position
                CMFollowTarget.localPosition = CMFollowTargetOffsetPos;
                CMLookAtTarget.localPosition = CMLookAtTargetOffsetPos;

                //set freelook camera values
                setCameraSpeedScript.SetADSSpeed();

                //set gun to hand
                StopAllCoroutines();
                AttachToGunFrontSocket();
                anim.SetBool("ADS", true);
                crosshair.SetActive(true);
                flagEquip = false;
            }

            //switch shoulder camera
            if (playerInputScript.actions.Player.SwitchShoulder.ReadValue<float>() > 0)
            {
                CMFollowTargetOffsetPos = CMFollowTargetOffsetPosRight;
                CMLookAtTargetOffsetPos = CMLookAtTargetOffsetPosRight;
                //set camera position
                CMFollowTarget.localPosition = CMFollowTargetOffsetPos;
                CMLookAtTarget.localPosition = CMLookAtTargetOffsetPos;
            }
            else if (playerInputScript.actions.Player.SwitchShoulder.ReadValue<float>() < 0)
            {
                CMFollowTargetOffsetPos = CMFollowTargetOffsetPosLeft;
                CMLookAtTargetOffsetPos = CMLookAtTargetOffsetPosLeft;
                //set camera position
                CMFollowTarget.localPosition = CMFollowTargetOffsetPos;
                CMLookAtTarget.localPosition = CMLookAtTargetOffsetPos;
            }
        }
        else
        {
            if (!flagEquip && Time.timeScale == 1)
            {
                //reset camera position
                CMFollowTarget.localPosition = CMFollowTargetInitialLocalPos;
                CMLookAtTarget.localPosition = CMLookAtTargetInitialLocalPos;

                //set freelook camera values
                setCameraSpeedScript.SetDefaultSpeed();

                //set gun to back
                AttachToGunBackSocket();
                anim.SetBool("ADS", false);
                crosshair.SetActive(false);
                flagEquip = true;

                playerMovementScript.ADS = false;
            }
        }
    }

    void LateUpdate()
    {
        if (playerInputScript.actions.Player.ADS.ReadValue<float>() > 0f && Time.timeScale == 1)
        {
            spineBone.rotation = camera.rotation;
        }
    }
    public void AttachToGunBackSocket()
    {
        //set parent of gun to back
        gun.parent = backSocket;
        gun.localPosition = Vector3.zero;
        gun.localRotation = Quaternion.identity;
    }
    public void AttachToGunFrontSocket()
    {
        gun.parent = frontSocket;
        gun.localPosition = Vector3.zero;
        gun.localRotation = Quaternion.identity;
    }
}
