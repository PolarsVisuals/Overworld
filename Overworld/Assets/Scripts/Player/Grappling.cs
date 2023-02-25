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
    public Vector3 grapplePointOffset;
    private Vector3 grappleLinePoint;

    [Header("Hooking")]
    Transform currentHookedObj;
    [SerializeField] bool isHooking;

    [Header("Cooldown")]
    public float grapplingCd;
    public float grapplingCdTimer;

    public Animator anim;

    public bool grappling;

    private void Start()
    {
        pm = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Grapple") && !grappling)
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

        if (isHooking)
        {
            lr.SetPosition(1, currentHookedObj.position);
        }

        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, maxGrappleDistance, whatIsGrappable) && grapplingCdTimer <= 0)
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
            grappleLinePoint = hit.point;

            //Hook
            if (hit.transform.GetComponent<Rigidbody>() != null)
            {
                currentHookedObj = hit.transform;

                grapplePoint = hit.point;
                Invoke(nameof(ExecuteHook), grappleDelayTime);
            }           
            //Grapple
            else
            {
                grapplePoint = hit.point + grapplePointOffset;
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
        lr.SetPosition(1, grappleLinePoint);
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

    private void ExecuteHook()
    {
        isHooking = true;

        GrabObjectWithRaycast(currentHookedObj, grapplePoint);

        Invoke(nameof(StopGrapple), 1f);
    }

    public void StopGrapple()
    {
        grappling = false;
        isHooking = false;

        pm.canMove = true;

        grapplingCdTimer = grapplingCd;

        lr.enabled = false;
    }

    public void GrabObjectWithRaycast(Transform hookedObj, Vector3 grapplePoint)
    {
        Vector3 heading = grapplePoint - gunTip.position; //startpoint
        float distance = Vector3.Distance(gunTip.position, grapplePoint);
        Vector3 direction = heading / distance;

        Ray ray = new Ray(gunTip.position, direction * distance);
        Debug.DrawRay(gunTip.position, direction * distance, Color.red, 5);

        Rigidbody rb = currentHookedObj.GetComponent<Rigidbody>();

        if (rb != null && !rb.isKinematic && rb.constraints == RigidbodyConstraints.None)
        {
            float force = distance;
            Debug.Log("Distance: " + distance + ", Force: " + force);

            rb.AddForce(ray.direction * -force, ForceMode.Impulse);

            if (distance < 5f)
            {
                rb.velocity = Vector3.zero;
            }
        }
    }
}
