using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ThirdPersonCam : MonoBehaviour
{
    [Header("References")]
    public float rotationSpeed;
    public Grappling grappleScript;

    public Transform orientation;
    public Transform player;
    public Transform playerObj;
    public Rigidbody rb;

    [Header("Cinemachine")]
    public float sensX = 10f;
    public float sensY = 10f;
    public float TopClamp = 70.0f;
    public float BottomClamp = -30.0f;

    public GameObject CinemachineCameraTarget;

    private float cinemachineTargetYaw;
    private float cinemachineTargetPitch;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
    }

    private void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        //Rotate orientation
        Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
        orientation.forward = viewDir.normalized;

        //Rotate player object
        Vector3 inputDir = orientation.forward * v + orientation.right * h;

        if (player.GetComponent<Grappling>().grappling)
        {
            playerObj.forward = orientation.forward;
        }
        else
        {
            if (inputDir != Vector3.zero)
            {
                playerObj.forward = Vector3.Slerp(playerObj.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);
            }
        }
    }

    private void LateUpdate()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        cinemachineTargetYaw += mouseX * Time.fixedDeltaTime * sensX;
        cinemachineTargetPitch += mouseY * Time.fixedDeltaTime * -sensY;

        //Clamp values
        cinemachineTargetYaw = Mathf.Clamp(cinemachineTargetYaw, float.MinValue, float.MaxValue);
        cinemachineTargetPitch = Mathf.Clamp(cinemachineTargetPitch, BottomClamp, TopClamp);

        //Follow Target
        CinemachineCameraTarget.transform.rotation = Quaternion.Euler(cinemachineTargetPitch, cinemachineTargetYaw, 0.0f);
    }
}
