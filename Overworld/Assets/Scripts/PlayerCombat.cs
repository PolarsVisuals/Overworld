using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] float timePassed;
    float clipLength;
    float clipSpeed;

    [SerializeField] GameObject weapon;
    [SerializeField] GameObject weaponHolder;

    GameObject currentWeaponInHand;

    private Animator animator;
    private StarterAssetsInputs starterAssetsInputs;

    private bool isAttacking;
    private bool comboAttacking;
    [SerializeField] float attackDelay;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
        isAttacking = false;
        comboAttacking = false;
        attackDelay = 0;
    }

    private void Start()
    {
        currentWeaponInHand = Instantiate(weapon, weaponHolder.transform);
    }

    private void Update()
    {
        if (starterAssetsInputs.attack1 && GetComponent<ThirdPersonController>().Grounded == true && timePassed >= attackDelay && comboAttacking == false)
        {
            GetComponent<ThirdPersonController>().canMove = false;
            GetComponent<ThirdPersonController>().canJump = false;

            InitiateAttack(0);

            starterAssetsInputs.attack1 = false;
        }

        if (starterAssetsInputs.attack2 && GetComponent<ThirdPersonController>().Grounded == true && isAttacking == false)
        {
            GetComponent<ThirdPersonController>().canMove = false;
            GetComponent<ThirdPersonController>().canJump = false;

            InitiateAttack(1);

            starterAssetsInputs.attack2 = false;
        }

        if (isAttacking)
        {
            timePassed += Time.deltaTime;

            //Returns an error as it gets the default state first, but doesnt affect anything
            AnimatorClipInfo[] currentAnimClipInfo = animator.GetCurrentAnimatorClipInfo(1);
            //Access the current length of the clip
            clipLength = currentAnimClipInfo[0].clip.length;

            clipSpeed = animator.GetCurrentAnimatorStateInfo(1).speed;

            if(timePassed >= clipLength / clipSpeed && starterAssetsInputs.attack1 && comboAttacking == false)
            {
                animator.SetTrigger("attack1");
            }
            if(timePassed >= clipLength / clipSpeed)
            {
                animator.applyRootMotion = false;
                animator.SetTrigger("canMove");
                GetComponent<ThirdPersonController>().canMove = true;
                GetComponent<ThirdPersonController>().canJump = true;
                isAttacking = false;
                comboAttacking = false;
            }
        }
    }

    public void InitiateAttack(int attackNo)
    {
        animator.applyRootMotion = true;
        timePassed = 0f;
        if(attackNo == 0)
        {
            animator.SetTrigger("attack1");
        }
        if (attackNo == 1)
        {
            animator.SetTrigger("attack2");
            comboAttacking = true;
        }
        animator.SetFloat("Speed", 0f);

        isAttacking = true;
        attackDelay = 0.75f;
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
