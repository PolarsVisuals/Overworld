using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

public class Grappling : MonoBehaviour
{
    private ThirdPersonController thirdPersonController;
    public Transform cam;
    public Transform gunTip;
    public LayerMask whatIsGrapplable;
    public LineRenderer lr;

    public float maxGrappleDistance;
    public float grappleDelayTime;
    public float overshootYAxis;

    private Vector3 grapplePoint;

    public float grapplingCd;
    private float grapplingCdTimer;

    public KeyCode grappleKey = KeyCode.Mouse1;

    private bool grappling;

    private void Start()
    {
        thirdPersonController = GetComponent<ThirdPersonController>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(grappleKey))
        {
            StartGrapple();
        }

        if (grapplingCdTimer > 0)
        {
            grapplingCdTimer -= Time.deltaTime;
        }
    }

    private void LateUpdate()
    {
        if (grappling)
        {
            lr.SetPosition(0, gunTip.position);
        }
    }

    private void StartGrapple()
    {
        if (grapplingCdTimer > 0)
        {
            return;
        }

        grappling = true;

        thirdPersonController.canMove = false;

        RaycastHit hit;
        if(Physics.Raycast(cam.position, cam.forward, out hit, maxGrappleDistance, whatIsGrapplable))
        {
            grapplePoint = hit.point;

            Invoke(nameof(ExecuteGrapple), grappleDelayTime);
        }
        else
        {
            grapplePoint = cam.position + cam.forward * maxGrappleDistance;

            Invoke(nameof(StopGrapple), grappleDelayTime);
        }

        lr.enabled = true;
        lr.SetPosition(1, grapplePoint);
    }

    private void ExecuteGrapple()
    {
        thirdPersonController.canMove = false;

        StartCoroutine(LerpPosition(grapplePoint, 1f));

        Invoke(nameof(StopGrapple), 1f);
    }

    IEnumerator LerpPosition(Vector3 targetPosition, float duration)
    {
        float time = 0;
        Vector3 startPosition = transform.position;
        while (time < duration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition;
    }

    public void StopGrapple()
    {
        thirdPersonController.canMove = true;

        grappling = false;

        grapplingCdTimer = grapplingCd;

        lr.enabled = false;
    }
}
