using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Grappling : MonoBehaviour
{
    [Header("References")]
    private PlayerMovement pm;
    public Transform cam;
    public Transform grappleTip;
    public LayerMask whatIsGrappable;
    public LineRenderer lr;

    [Header("Grappling")]
    public float maxGrappleDistance;
    public float grappleDelayTime;
    public float overshootYAxis;

    private Vector3 grapplePoint;
    public Vector3 grapplePointOffset;
    private Vector3 grappleLinePoint;

    public bool grappling;

    [Header("Hooking")]
    [SerializeField] float pullForce;
    Transform currentHookedObj;
    [SerializeField] bool isHooking;

    [Header("Cooldown")]
    public float grapplingCd;
    public float grapplingCdTimer;

    [Header("HUD")]
    public Image grappleForeground;
    public Image crosshair;
    public Sprite[] sprite;
    bool smoothing;

    public Animator anim;

    private void Start()
    {
        pm = GetComponent<PlayerMovement>();
        smoothing = false;
        grapplingCdTimer = grapplingCd;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Grapple") && !grappling)
        {
            //Debug.Log("Pressed Grapple");
            StartGrapple();
        }
        if(Input.GetButton("Grapple") && isHooking)
        {
            //Debug.Log("Pulling Object");
            PullObjectWithRaycast(currentHookedObj);
        }
        if(Input.GetButtonUp("Grapple") && isHooking)
        {
            Invoke(nameof(StopGrapple), 0f);
        }

        if (grapplingCdTimer < grapplingCd)
        {
            grapplingCdTimer += Time.deltaTime;
        }
    }

    private void LateUpdate()
    {
        //LineRenderer
        if (grappling)
        {
            lr.SetPosition(0, grappleTip.position);
        }

        if (isHooking)
        {
            lr.SetPosition(1, currentHookedObj.position);
        }

        //HUD
        if (smoothing)
        {
            float prevFill = grappleForeground.fillAmount;
            float currFill = grapplingCdTimer / grapplingCd;
            if (currFill > prevFill) prevFill = Mathf.Min(prevFill + 0.01f, currFill);
            else if (currFill < prevFill) prevFill = Mathf.Max(prevFill - 0.01f, currFill);
            grappleForeground.fillAmount = prevFill;
        }

        //Crosshair
        Ray ray = new Ray(cam.position, cam.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxGrappleDistance, whatIsGrappable))
        {
            if (hit.transform.GetComponent<Rigidbody>() == null)
            {
                crosshair.sprite = sprite[2];
            }
            else
            {
                crosshair.sprite = sprite[1];
            }
        }
        else
        {
            crosshair.sprite = sprite[0];
        }
    }

    private void StartGrapple()
    {
        if (grapplingCdTimer < grapplingCd) return;

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

            lr.enabled = true;
            lr.SetPosition(1, grappleLinePoint);
        }
        //If the player hits nothing
        else
        {
            grapplePoint = cam.position + cam.forward * maxGrappleDistance;

            Invoke(nameof(StopGrapple), grappleDelayTime);

            lr.enabled = true;
            lr.SetPosition(1, grapplePoint);
        }
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

    void ExecuteHook()
    {
        isHooking = true;
    }

    public void StopGrapple()
    {
        grappling = false;
        isHooking = false;

        pm.canMove = true;

        grappleForeground.fillAmount = 0;
        grapplingCdTimer = 0;
        smoothing = true;

        lr.enabled = false;
    }

    public void PullObjectWithRaycast(Transform hookedObj)
    {
        Vector3 pos = hookedObj.position;
        float distance = Vector3.Distance(hookedObj.position, grappleTip.position);
        Vector3 dir = (grappleTip.position - hookedObj.position).normalized;
        Debug.DrawRay(pos, dir * distance, Color.red, 0.1f);

        Ray ray = new Ray(hookedObj.position, dir* distance);


        Rigidbody rb = hookedObj.GetComponent<Rigidbody>();

        if (rb != null && !rb.isKinematic && rb.constraints == RigidbodyConstraints.None)
        {
            float force = pullForce;
            Debug.Log("Distance: " + distance + ", Force: " + force);

            rb.AddForce(ray.direction * force, ForceMode.Force);

            if (distance < 2f)
            {
                Debug.Log("Reached");
                rb.velocity = Vector3.zero;
                Invoke(nameof(StopGrapple), 0f);
            }
        }
    }
}
