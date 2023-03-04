using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed;
    public float sprintSpeed;
    public float groundDrag;

    public bool isSprinting;
    public bool canMove;

    public bool isSnapping;
    public Vector3 targetPos;
    
    float horizontalInput;
    float verticalInput;
    Vector3 moveDirection;
    Rigidbody rb;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;

    public bool canJump;

    [Header("Grapple")]
    bool activeGrapple;

    [Header("Ground Check")]
    public LayerMask whatIsGround;
    public bool grounded;
    [Tooltip("Useful for rough ground")]
    public float GroundedOffset = -0.14f;
    [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
    public float GroundedRadius = 0.28f;

    public Transform orientation;

    [Header("Animation")]
    public Animator animator;
    public float SpeedChangeRate = 10.0f;

    private float animationBlend;

    // animation IDs
    private int animIDSpeed;
    private int animIDGrounded;
    private int animIDJump;
    private int animIDFreeFall;
    private int animIDMotionSpeed;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        AssignAnimationIDs();

        canJump = true;
        isSprinting = false;
        canMove = true;
        activeGrapple = false;

        isSnapping = false;
    }

    private void Update()
    {
        if(!canMove)
        {
            rb.velocity = Vector3.zero;
        }

        MyInput();
        GroundedCheck();
        SpeedControl();

        // handle drag
        if (grounded && !activeGrapple)
            rb.drag = groundDrag;
        else
            rb.drag = 0;
    }

    private void FixedUpdate()
    {
        if (isSnapping)
        {
            Snapping(targetPos);
        }

        MovePlayer();
    }

    private void GroundedCheck()
    {
        // set sphere position, with offset
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
            transform.position.z);
        grounded = Physics.CheckSphere(spherePosition, GroundedRadius, whatIsGround,
            QueryTriggerInteraction.Ignore);

        animator.SetBool(animIDGrounded, grounded);
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // when to jump
        if (Input.GetButtonDown("Jump") && canJump && grounded)
        {
            canJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        if (Input.GetButton("Sprint"))
        {
            isSprinting = true;
        }
        else
        {
            isSprinting = false;
        }
    }

    private void MovePlayer()
    {
        if (activeGrapple)
        {
            animator.SetBool(animIDFreeFall, true);
        }

        float targetSpeed = isSprinting ? sprintSpeed : walkSpeed;
        Vector2 moveInput = new Vector2(horizontalInput, verticalInput);

        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (moveInput == Vector2.zero || canMove == false)
        {
            targetSpeed = 0.0f;
        }

        animationBlend = Mathf.Lerp(animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
        if (animationBlend < 0.01f) animationBlend = 0f;

        // on ground
        if (grounded)
        {
            animator.SetBool(animIDJump, false);
            animator.SetBool(animIDFreeFall, false);

            rb.AddForce(moveDirection.normalized * targetSpeed * 10f, ForceMode.Force);
        }
            
        // in air
        else if (!grounded)
        {
            animator.SetBool(animIDFreeFall, true);

            rb.AddForce(moveDirection.normalized * targetSpeed * 10f * airMultiplier, ForceMode.Force);
        }            

        //Set Animation
        animator.SetFloat(animIDSpeed, animationBlend);
        animator.SetFloat(animIDMotionSpeed, 1);

    }

    private void SpeedControl()
    {
        if (activeGrapple) return;

        float targetSpeed = isSprinting ? sprintSpeed : walkSpeed;

        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // limit velocity if needed
        if (flatVel.magnitude > targetSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * targetSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

        animator.SetBool(animIDJump, true);
    }
    private void ResetJump()
    {
        canJump = true;
    }

    private bool enableMovementOnNextTouch;

    public void JumpToPosition(Vector3 targetPosition, float trajectoryHeight)
    {
        activeGrapple = true;

        velocityToSet = CalculateJumpVelocity(transform.position, targetPosition, trajectoryHeight);
        Invoke(nameof(SetVelocity), 0.1f);
    }

    public void SnapToPosition(Vector3 targetPosition)
    {
        ThirdPersonCam camScript = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<ThirdPersonCam>();

        targetPos = targetPosition;

        isSnapping = true;
    }

    void Snapping(Vector3 _targetPos)
    {
        // Move our position a step closer to the target.
        transform.position = Vector3.MoveTowards(transform.position, _targetPos, 1);

        // Check if the position of the cube and sphere are approximately equal.
        if (Vector3.Distance(transform.position, _targetPos) < 1f)
        {
            isSnapping = false;
        }
    }

    private Vector3 velocityToSet;
    private void SetVelocity()
    {
        enableMovementOnNextTouch = true;
        rb.velocity = velocityToSet;
    }

    public void ResetRestrictions()
    {
        activeGrapple = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (enableMovementOnNextTouch)
        {
            enableMovementOnNextTouch = false;
            ResetRestrictions();

            GetComponent<Grappling>().StopGrapple();
        }
    }

    public Vector3 CalculateJumpVelocity (Vector3 startPoint, Vector3 endPoint, float trajectoryHeight)
    {
        float gravity = Physics.gravity.y;
        float displacementY = endPoint.y - startPoint.y;
        Vector3 displacementXZ = new Vector3(endPoint.x - startPoint.x, 0f, endPoint.z - startPoint.z);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);
        Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * trajectoryHeight / gravity)
            + Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / gravity));

        return velocityXZ + velocityY;
    }

    private void AssignAnimationIDs()
    {
        animIDSpeed = Animator.StringToHash("Speed");
        animIDGrounded = Animator.StringToHash("Grounded");
        animIDJump = Animator.StringToHash("Jump");
        animIDFreeFall = Animator.StringToHash("FreeFall");
        animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
    }

    private void OnDrawGizmosSelected()
    {
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        if (grounded) Gizmos.color = transparentGreen;
        else Gizmos.color = transparentRed;

        // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
        Gizmos.DrawSphere(
            new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
            GroundedRadius);
    }
}
