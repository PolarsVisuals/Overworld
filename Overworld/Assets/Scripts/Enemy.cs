using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Header("References")]
    NavMeshAgent agent;
    GameObject player;
    public DamageDealer damageDealer;

    [Header("Combat")]
    [SerializeField] float attackCD = 3f;
    [SerializeField] float attackRange = 1f;
    [SerializeField] float aggroRange = 4f;

    float timePassed;

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        timePassed += Time.deltaTime;

        if(timePassed >= attackCD)
        {
            if(Vector3.Distance(player.transform.position, transform.position) <= attackRange)
            {
                damageDealer.dealDamage = true;
                timePassed = 0;
            }
        }

        if (Vector3.Distance(player.transform.position, transform.position) <= aggroRange)
        {
            //transform.LookAt(player.transform);
            agent.destination = player.transform.position;
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, aggroRange);
    }
}
