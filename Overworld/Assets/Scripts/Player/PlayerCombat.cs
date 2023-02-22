using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public Animator animator;

    public int comboCount = 0;
    public float comboDelay = 0.75f;

    public float comboTimer = 0f;
    public float maxComboTime = 1f;

    public bool isAttacking;
    bool canAttack = true;

    [SerializeField] GameObject weapon;
    [SerializeField] GameObject weaponHolder;

    public GameObject currentWeaponInHand;

    private PlayerMovement playerMovement;

    private void Awake()
    {
        isAttacking = false;
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
            if (!isAttacking)
            {
                StartAttack();
            }
            else
            {
                if (comboTimer >= comboDelay)
                {
                    ContinueCombo();
                }
            }
        }

        if (isAttacking)
        {
            comboTimer += Time.deltaTime;
            if (comboTimer >= maxComboTime)
            {
                EndAttack();
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
        comboTimer = 0;
    }

    private void ContinueCombo()
    {
        Debug.Log("Combo Registered");

        if (comboCount == 1)
        {
            animator.SetTrigger("attack2");
            comboCount++;
            comboTimer = 0f;
        }

        else if (comboCount == 2)
        {
            animator.SetTrigger("attack3");
            comboCount++;
            comboTimer = 0f;
        }
    }

    private void EndAttack()
    {
        isAttacking = false;
        StartCoroutine(AttackCooldown());

        playerMovement.canMove = true;
        playerMovement.canJump = true;

        comboCount = 0;
        comboTimer = 0f;
    }

    IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(1);
        canAttack = true;
    }
}
