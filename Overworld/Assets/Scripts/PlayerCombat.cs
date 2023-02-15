using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

public class PlayerCombat : MonoBehaviour
{
    public Camera cam;
    public LineRenderer lr;
    public LayerMask whatIsGrab = new LayerMask();
    public float maxDistance = 100;
    public Transform debug;

    private Vector3 hookshotPos;

    public bool isGrappling;

    [SerializeField] float timePassed;
    float clipLength;
    float clipSpeed;

    [SerializeField] GameObject weapon;
    [SerializeField] GameObject weaponHolder;

    GameObject currentWeaponInHand;

    private Animator animator;
    private StarterAssetsInputs starterAssetsInputs;

    private bool isAttacking;
    [SerializeField] float attackDelay;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
        isAttacking = false;
        attackDelay = 0;
    }

    private void Start()
    {
        currentWeaponInHand = Instantiate(weapon, weaponHolder.transform);
    }

    private void Update()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out RaycastHit hit, maxDistance, whatIsGrab);

        debug.position = hit.point;

        if (starterAssetsInputs.attack && GetComponent<ThirdPersonController>().Grounded == true && timePassed >= attackDelay)
        {
            GetComponent<ThirdPersonController>().canMove = false;
            GetComponent<ThirdPersonController>().canJump = false;
            
            InitiateAttack();

            starterAssetsInputs.attack = false;
        }

        if (starterAssetsInputs.grapple && isAttacking == false)
        {
            HandleHookshotStart();

            starterAssetsInputs.grapple = false;
        }
        
        if (isGrappling)
        {
            HandleHookshotMovement();
        }

        if (isAttacking)
        {
            timePassed += Time.deltaTime;

            //Returns an error as it gets the default state first, but doesnt affect anything
            AnimatorClipInfo[] currentAnimClipInfo = animator.GetCurrentAnimatorClipInfo(1);
            //Access the current length of the clip
            clipLength = currentAnimClipInfo[0].clip.length;

            clipSpeed = animator.GetCurrentAnimatorStateInfo(1).speed;

            if(timePassed >= attackDelay && starterAssetsInputs.attack)
            {
                animator.SetTrigger("attack");
            }
            if(timePassed >= clipLength / clipSpeed)
            {
                animator.applyRootMotion = false;
                animator.SetTrigger("canMove");
                GetComponent<ThirdPersonController>().canMove = true;
                GetComponent<ThirdPersonController>().canJump = true;
                isAttacking = false;
            }
        }
    }

    private void LateUpdate()
    {
        if (isGrappling)
        {
            lr.SetPosition(0, lr.transform.position);
        }
    }

    public void InitiateAttack()
    {
        animator.applyRootMotion = true;
        timePassed = 0f;
        animator.SetTrigger("attack");
        animator.SetFloat("Speed", 0f);

        isAttacking = true;
        attackDelay = 0.5f;
    }

    private void HandleHookshotStart()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 10, Color.green);

        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, whatIsGrab))
        {
            GetComponent<ThirdPersonController>().canMove = false;
            GetComponent<ThirdPersonController>().canJump = false;

            hookshotPos = hit.point;

            transform.LookAt(new Vector3(hit.point.x, transform.position.y, hit.point.z));

            lr.enabled = true;
            lr.SetPosition(1, hit.point);

            isGrappling = true;
        }

    }

    private void HandleHookshotMovement()
    {
        animator.SetTrigger("grapple");
        animator.SetFloat("Speed", 0f);
        animator.SetBool("FreeFall", true);

        Vector3 hookshotDir = (hookshotPos - transform.position).normalized;

        float min = 10f;
        float max = 40f;
        float speed = Mathf.Clamp(Vector3.Distance(transform.position, hookshotPos), min, max);
        float multipler = 2f;


        GetComponent<CharacterController>().Move(hookshotDir * speed * multipler * Time.deltaTime);

        if(Vector3.Distance(transform.position, hookshotPos) < 2f)
        {
            GetComponent<ThirdPersonController>().canMove = true;
            GetComponent<ThirdPersonController>().canJump = true;
            GetComponent<ThirdPersonController>().ResetGravity();

            animator.SetBool("FreeFall", false);

            lr.enabled = false;

            isGrappling = false;
        }
    }

    public void StartDealDamage()
    {
        currentWeaponInHand.GetComponentInChildren<DamageDealer>().StartDealDamage();
    }

    public void EndDealDamage()
    {
        currentWeaponInHand.GetComponentInChildren<DamageDealer>().EndDealDamage();
    }
}
