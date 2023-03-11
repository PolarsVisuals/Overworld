using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCombat : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Animator animator;
    [SerializeField] GameObject weapon;
    [SerializeField] GameObject weaponHolder;
    [SerializeField] Spawner spawner;

    [Header("Attacking")]
    [SerializeField] float comboCount;
    [SerializeField] float attackCooldown;
    [SerializeField] float snapMin;
    [SerializeField] float snapMax;
    public float attackCdTimer;

    [Header("HUD")]
    public Image attackForeground;
    bool smoothing;

    bool canAttack;
    public bool isAttacking;
    public bool isDead;
 
    private GameObject currentWeaponInHand;
    private PlayerMovement playerMovement;

    private DamageDealer damageDealer;

    [SerializeField] List<Transform> enemies = new List<Transform>();
    [SerializeField] Transform targetedEnemy;

    private void Awake()
    {
        isAttacking = false;
        canAttack = true;

        attackCdTimer = attackCooldown;
        smoothing = false;

        isDead = false;
    }

    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();

        currentWeaponInHand = Instantiate(weapon, weaponHolder.transform);

        damageDealer = currentWeaponInHand.GetComponent<DamageDealer>();
    }

    private void Update()
    {
        if (!isDead)
        {
            targetedEnemy = GetClosestEnemy();

            float distFromEnemy = Vector3.Distance(transform.position, targetedEnemy.position);
            Debug.DrawLine(transform.position, targetedEnemy.position, Color.blue);

            if (Input.GetButtonDown("Attack") && playerMovement.grounded && canAttack)
            {
                canAttack = false;

                if (distFromEnemy <= snapMax)
                {
                    if (distFromEnemy >= snapMin)
                    {
                        playerMovement.SnapToPosition(targetedEnemy.position, snapMin);
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

            if (attackCdTimer < attackCooldown)
            {
                attackCdTimer += Time.deltaTime;
            }

            if (smoothing)
            {
                float prevFill = attackForeground.fillAmount;
                float currFill = attackCdTimer / attackCooldown;
                if (currFill > prevFill) prevFill = Mathf.Min(prevFill + 0.05f, currFill);
                else if (currFill < prevFill) prevFill = Mathf.Max(prevFill - 0.05f, currFill);
                attackForeground.fillAmount = prevFill;
            }
        }
    }

    private void StartAttack()
    {
        isAttacking = true;

        damageDealer.dealDamage = true;

        playerMovement.canMove = false;
        playerMovement.canJump = false;  

        animator.SetTrigger("attack1");

        comboCount = 1;
    }

    public void InitiateCombo()
    {
        canAttack = true;

        damageDealer.dealDamage = false;
    }

    public void ExecuteCombo()
    {
        damageDealer.dealDamage = true;
        if (comboCount == 1)
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

        damageDealer.dealDamage = false;

        playerMovement.canMove = true;
        playerMovement.canJump = true;
    }

    IEnumerator AttackCooldown()
    {
        canAttack = false;
        attackCdTimer = 0;
        attackForeground.fillAmount = 0;
        smoothing = true;
        yield return new WaitForSeconds(attackCooldown);
        smoothing = false;
        canAttack = true;
    }

    Transform GetClosestEnemy()
    {
        //Clears the current list
        enemies.Clear();

        List<GameObject> enemyList;

        if (spawner == null)
        {
            Debug.Log("No spawner found");
            return transform;
        }
        else
        {
            enemyList = spawner.enemies;
            if (enemyList.Count == 0)
            {
                Debug.Log("No enemies found");
                return transform;
            }
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
