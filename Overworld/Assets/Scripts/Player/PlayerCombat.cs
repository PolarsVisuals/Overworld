using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Animator animator;
    [SerializeField] GameObject weapon;
    [SerializeField] GameObject weaponHolder;

    [Header("Attacking")]
    [SerializeField] float comboCount;

    bool canAttack;
    bool isAttacking;
 
    private GameObject currentWeaponInHand;
    private PlayerMovement playerMovement;

    private void Awake()
    {
        isAttacking = false;
        canAttack = true;
    }

    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();

        currentWeaponInHand = Instantiate(weapon, weaponHolder.transform);
    }

    private void Update()
    {
        if (Input.GetButtonDown("Attack") && playerMovement.grounded && canAttack)
        {
            canAttack = false;

            if (isAttacking)
            {
                ExecuteCombo();
            }
            else
            {
                StartAttack();
            }
        }
    }

    private void StartAttack()
    {
        isAttacking = true;

        playerMovement.canMove = false;
        playerMovement.canJump = false;

        animator.SetTrigger("attack1");

        comboCount = 1;
    }

    public void InitiateCombo()
    {
        canAttack = true;
    }

    public void ExecuteCombo()
    {
        if(comboCount == 1)
        {
            animator.SetTrigger("attack2");
        }
        else if (comboCount == 2)
        {
            animator.SetTrigger("attack3");
        }

        comboCount++;
    }

    public void EndAttack()
    {
        Debug.Log("Ended");
        isAttacking = false;
        StartCoroutine(AttackCooldown());

        playerMovement.canMove = true;
        playerMovement.canJump = true;
    }

    IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(1);
        canAttack = true;
    }
}
