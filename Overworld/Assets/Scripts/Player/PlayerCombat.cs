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
    [SerializeField] float attackCooldown;

    [SerializeField] float snapRange;

    bool canAttack;
    bool isAttacking;
 
    private GameObject currentWeaponInHand;
    private PlayerMovement playerMovement;

    [SerializeField] List<Transform> enemies = new List<Transform>();
    [SerializeField] Transform targetedEnemy;

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
        targetedEnemy = GetClosestEnemy();

        float distFromEnemy = Vector3.Distance(transform.position, targetedEnemy.position);
        Debug.DrawLine(transform.position, targetedEnemy.position, Color.blue);

        if (Input.GetButtonDown("Attack") && playerMovement.grounded && canAttack)
        {
            canAttack = false;

            if(distFromEnemy <= snapRange)
            {
                if(distFromEnemy >= 2)
                {
                    playerMovement.SnapToPosition(targetedEnemy.position);
                }
                else
                {
                    ThirdPersonCam cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<ThirdPersonCam>();
                    cam.LookAtTarget(targetedEnemy.position);
                }
            }

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
        //Debug.Log("Ended");
        isAttacking = false;
        StartCoroutine(AttackCooldown());

        playerMovement.canMove = true;
        playerMovement.canJump = true;
    }

    IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    Transform GetClosestEnemy()
    {
        //Clears the current list
        enemies.Clear();

        //Makes a list of all enemies in the level
        List<GameObject> enemyList = GameObject.Find("Spawner").GetComponent<Spawner>().enemies;
        if(enemyList.Count == 0)
        {
            Debug.Log("No enemies found");
            return transform;
        }

        foreach(GameObject enemy in enemyList)
        {
            enemies.Add(enemy.transform);
        }

        Transform closestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        foreach (Transform potentialTarget in enemies)
        {
            Vector3 directionToTarget = potentialTarget.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                closestTarget = potentialTarget;
            }
        }

        return closestTarget;
    }
}
