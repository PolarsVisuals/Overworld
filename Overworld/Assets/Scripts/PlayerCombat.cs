using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] float timePassed;
    float clipLength;
    float clipSpeed;

    [SerializeField] GameObject weapon;
    [SerializeField] GameObject weaponHolder;

    public GameObject currentWeaponInHand;

    public Animator animator;

    private bool isAttacking;
    [SerializeField] float attackDelay;

    private void Awake()
    {
        isAttacking = false;
        attackDelay = 0;
    }

    private void Start()
    {
        currentWeaponInHand = Instantiate(weapon, weaponHolder.transform);
    }

    private void Update()
    {
        if (Input.GetButtonDown("Attack") && GetComponent<PlayerMovement>().grounded == true && timePassed >= attackDelay)
        {
            //GetComponent<ThirdPersonController>().canMove = false;
            //GetComponent<ThirdPersonController>().canJump = false;
            
            InitiateAttack();
        }

        if (Input.GetButtonDown("Grapple") && isAttacking == false)
        {
            Debug.Log("Used Grapple");
        }

        if (isAttacking)
        {
            timePassed += Time.deltaTime;

            //Returns an error as it gets the default state first, but doesnt affect anything
            AnimatorClipInfo[] currentAnimClipInfo = animator.GetCurrentAnimatorClipInfo(1);
            //Access the current length of the clip
            clipLength = currentAnimClipInfo[0].clip.length;

            clipSpeed = animator.GetCurrentAnimatorStateInfo(1).speed;

            if(timePassed >= attackDelay && Input.GetButtonDown("Attack"))
            {
                animator.SetTrigger("attack");
            }
            if(timePassed >= clipLength / clipSpeed)
            {
                animator.applyRootMotion = false;
                animator.SetTrigger("canMove");
                //GetComponent<ThirdPersonController>().canMove = true;
                //GetComponent<ThirdPersonController>().canJump = true;
                isAttacking = false;
            }
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
}
