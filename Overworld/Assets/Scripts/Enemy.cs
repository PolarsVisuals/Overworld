using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Header("References")]
    public Animator animator;
    NavMeshAgent agent;
    GameObject player;
    public DamageDealer damageDealer;

    [Header("Combat")]
    [SerializeField] float attackCD = 3f;
    [SerializeField] float attackRange = 1f;
    [SerializeField] float aggroRange = 4f;

    float timePassed;
    public bool canMove;
    public bool dead;

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        canMove = true;
        dead = false;
    }

    void Update()
    {
        timePassed += Time.deltaTime;

        Debug.Log("Enemy Vel " + agent.velocity.magnitude);
        animator.SetFloat("Speed", agent.velocity.magnitude);

        if(timePassed >= attackCD && !dead)
        {
            if(Vector3.Distance(player.transform.position, transform.position) <= attackRange)
            {
                canMove = false;
                animator.SetTrigger("Attack");
                damageDealer.dealDamage = true;
                timePassed = 0;
            }
        }

        if (Vector3.Distance(player.transform.position, transform.position) <= aggroRange && canMove)
        {
            //transform.LookAt(player.transform);
            agent.destination = player.transform.position;
        }

    }

    void EndAttack()
    {
        canMove = true;
        Debug.Log("Enemy ended attack");
        damageDealer.dealDamage = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, aggroRange);
    }
}
