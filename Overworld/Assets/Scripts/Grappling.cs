using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grappling : MonoBehaviour
{
    [Header("References")]
    private PlayerMovement pm;
    public Transform cam;
    public Transform gunTip;
    public LayerMask whatIsGrappable;
    public LineRenderer lr;
    public GameObject debug;

    [Header("Grappling")]
    public float maxGrappleDistance;
    public float grappleDelayTime;
    public float overshootYAxis;

    private Vector3 grapplePoint;

    [Header("Hooking")]
    public float pullForce = 10f;

    [Header("Cooldown")]
    public float grapplingCd;
    private float grapplingCdTimer;

    public Animator anim;

    public KeyCode grappleKey = KeyCode.Mouse1;

    private bool grappling;

    private void Start()
    {
        pm = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Grapple"))
        {
            StartGrapple();
        }

        if(grapplingCdTimer > 0)
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

        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, maxGrappleDistance, whatIsGrappable))
        {
            debug.SetActive(true);
        }
        else
        {
            debug.SetActive(false);
        }

    }

    private void StartGrapple()
    {
        if (grapplingCdTimer > 0) return;

        grappling = true;
        pm.canMove = false;

        anim.SetTrigger("Grapple");

        Ray ray = new Ray(cam.position, cam.forward);
        RaycastHit hit;

        //If the player hit something
        if(Physics.Raycast(ray, out hit, maxGrappleDistance, whatIsGrappable))
        {
            grapplePoint = hit.point;

            if (hit.transform.GetComponent<Rigidbody>() != null)
            {
                hit.transform.position = gunTip.position;
                Invoke(nameof(StopGrapple), grappleDelayTime);
            }
            else
            {
                Invoke(nameof(ExecuteGrapple), grappleDelayTime);
            }
        }
        //If the player hits nothing
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
        pm.canMove = true;

        Vector3 lowestPoint = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);

        float grapplePointRelativeYPos = grapplePoint.y - lowestPoint.y;
        float highestPointOnArc = grapplePointRelativeYPos + overshootYAxis;

        if (grapplePointRelativeYPos < 0) highestPointOnArc = overshootYAxis;

        pm.JumpToPosition(grapplePoint, highestPointOnArc);

        Invoke(nameof(StopGrapple), 1f);
    }

    public void StopGrapple()
    {
        grappling = false;
        pm.canMove = true;

        grapplingCdTimer = grapplingCd;

        lr.enabled = false;
    }
}
