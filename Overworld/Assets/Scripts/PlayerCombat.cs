using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

public class PlayerCombat : MonoBehaviour
{
    float timePassed;
    float clipLength;
    float clipSpeed;

    [SerializeField] GameObject weapon;
    [SerializeField] GameObject weaponHolder;

    GameObject currentWeaponInHand;

    private Animator animator;
    private StarterAssetsInputs starterAssetsInputs;

    private bool isAttacking;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
        isAttacking = false;
    }

    private void Start()
    {
        currentWeaponInHand = Instantiate(weapon, weaponHolder.transform);
    }

    private void Update()
    {

        if (starterAssetsInputs.attack && GetComponent<ThirdPersonController>().Grounded == true)
        {
            GetComponent<ThirdPersonController>().canMove = false;
            GetComponent<ThirdPersonController>().canJump = false;

            InitiateAttack();

            starterAssetsInputs.attack = false;
        }

        if(isAttacking)
        {
            timePassed += Time.deltaTime;

            //Returns an error as it gets the default state first, but doesnt affect anything
            AnimatorClipInfo[] currentAnimClipInfo = animator.GetCurrentAnimatorClipInfo(1);
            //Access the current length of the clip
            clipLength = currentAnimClipInfo[0].clip.length;

            clipSpeed = animator.GetCurrentAnimatorStateInfo(1).speed;

            if(timePassed >= clipLength / clipSpeed && starterAssetsInputs.attack)
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

    public void InitiateAttack()
    {
        animator.applyRootMotion = true;
        timePassed = 0f;
        animator.SetTrigger("attack");
        animator.SetFloat("Speed", 0f);

        isAttacking = true;
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
