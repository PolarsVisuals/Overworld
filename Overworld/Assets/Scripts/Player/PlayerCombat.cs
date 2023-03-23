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
    public List<AttackSO> combo;
    float lastTimeClicked;
    float lastComboEnd;
    int comboCounter;

    [SerializeField] float snapMin;
    [SerializeField] float snapMax;
    public EnemyCrossahir enemyCrosshair;

    public bool isDead;
 
    private GameObject currentWeaponInHand;
    private PlayerMovement playerMovement;

    private DamageDealer damageDealer;

    [SerializeField] List<Transform> enemies = new List<Transform>();
    [SerializeField] Transform targetedEnemy;

    private void Awake()
    {
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
            ExitAttack();

            targetedEnemy = GetClosestEnemy();
            float distFromEnemy = Vector3.Distance(transform.position, targetedEnemy.position);
            Debug.DrawLine(transform.position, targetedEnemy.position, Color.blue);


            if (Input.GetButtonDown("Attack") && playerMovement.grounded)
            {
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

                Attack();
            }

            if (distFromEnemy <= snapMax)
            {
                enemyCrosshair.enemyPos = targetedEnemy.position;
                enemyCrosshair.TurnOn();
            }
            else
            {
                enemyCrosshair.TurnOff();
            }

            if (Input.GetButtonDown("Jump"))
            {
                EndCombo();
            }
        }
    }

    void Attack()
    {
        if(Time.time - lastComboEnd > .3f && comboCounter <= combo.Count)
        {
            CancelInvoke("EndCombo");

            if(Time.time - lastTimeClicked >= .3f)
            {
                playerMovement.canMove = false;

                animator.runtimeAnimatorController = combo[comboCounter].animatorOV;
                animator.Play("Attack", 1, 0);

                damageDealer.damage = combo[comboCounter].damage;
                damageDealer.dealDamage = true;

                comboCounter++;
                lastTimeClicked = Time.time;

                if(comboCounter + 1 > combo.Count)
                {
                    comboCounter = 0;
                }
            }
        }
    }

    void ExitAttack()
    {
        if(animator.GetCurrentAnimatorStateInfo(1).normalizedTime > 0.9f && animator.GetCurrentAnimatorStateInfo(1).IsTag("Attack"))
        {
            playerMovement.canMove = true;
            Invoke("EndCombo",.5f);
        }
    }

    void EndCombo()
    {
        playerMovement.canMove = true;
        comboCounter = 0;
        lastComboEnd = Time.time;

        damageDealer.dealDamage = false;
    }

    Transform GetClosestEnemy()
    {
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
            if(enemy != null || enemy.GetComponent<Enemy>().activeTarget == true)
            {
                enemies.Add(enemy.transform);
            }
            else
            {
                return transform;
            }

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
