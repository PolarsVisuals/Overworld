using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public Animator animator;

    public int comboCount = 0;
    public bool isAttacking;
    public float attackTimer = 0f;
    public float maxComboTime = 1f;

    [SerializeField] GameObject weapon;
    [SerializeField] GameObject weaponHolder;

    public GameObject currentWeaponInHand;

    private void Awake()
    {
        isAttacking = false;
    }

    private void Start()
    {
        currentWeaponInHand = Instantiate(weapon, weaponHolder.transform);
    }

    private void Update()
    {
        if (Input.GetButtonDown("Attack") && GetComponent<PlayerMovement>().grounded == true)
        {
            if (!isAttacking)
            {
                GetComponent<PlayerMovement>().canMove = false;
                GetComponent<PlayerMovement>().canJump = false;
                StartAttack();
            }
            else
            {
                ContinueCombo();
            }
        }

        if (isAttacking)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer >= maxComboTime)
            {
                EndAttack();
            }
        }
    }

    private void StartAttack()
    {
        isAttacking = true;
        comboCount = 1;
        attackTimer = 0f;
        animator.SetTrigger("attack1");
    }

    private void ContinueCombo()
    {
        comboCount++;
        attackTimer = 0f;
        if (comboCount == 2)
        {
            animator.SetTrigger("attack2");
        }
        else if (comboCount == 3)
        {
            animator.SetTrigger("attack3");
        }
    }

    private void EndAttack()
    {
        GetComponent<PlayerMovement>().canMove = true;
        GetComponent<PlayerMovement>().canJump = true;

        isAttacking = false;
        comboCount = 0;
        attackTimer = 0f;
    }
}
