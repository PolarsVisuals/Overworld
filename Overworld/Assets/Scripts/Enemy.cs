using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] float health = 3;

    [Header("Combat")]
    [SerializeField] float attackCD = 3f;
    [SerializeField] float attackRange = 1f;
    [SerializeField] float aggroRange = 4f;

    GameObject player;
    PlayerCombat playerCombatScript;

    Animator animator;
    NavMeshAgent agent;
    float timePassed;

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        timePassed += Time.deltaTime;

        animator.SetFloat("speed", agent.velocity.magnitude / agent.speed);

        if(timePassed >= attackCD)
        {
            if(Vector3.Distance(player.transform.position, transform.position) <= attackRange)
            {
                animator.SetTrigger("attack");
                timePassed = 0;
            }
        }

        if (Vector3.Distance(player.transform.position, transform.position) <= aggroRange)
        {
            agent.SetDestination(player.transform.position);
        }

    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        animator.SetTrigger("damage");

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, aggroRange);
    }
}
