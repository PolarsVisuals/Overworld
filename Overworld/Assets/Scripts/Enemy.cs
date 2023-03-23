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
    public GameObject spawnCloud;

    [Header("Combat")]
    [SerializeField] float attackCD = 3f;
    [SerializeField] float attackRange = 1f;
    [SerializeField] float aggroRange = 4f;

    [Header("Sounds")]
    public AudioSource source;
    public AudioClip[] impacts;
    public AudioClip spawn;

    float timePassed;
    public bool canMove;
    public bool dead;
    public bool activeTarget;

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        canMove = false;
        dead = false;
        activeTarget = true;
        Instantiate(spawnCloud, transform.position, Quaternion.LookRotation(Vector3.up));
        StartCoroutine(Spawned());
        source.clip = spawn;
        source.Play();
    }
    IEnumerator Spawned()
    {
        yield return new WaitForSeconds(2.15f);
        canMove = true;
    }

    void Update()
    {
        if (dead)
        {
            activeTarget = false;
            agent.destination = transform.position;
            return;
        }

        timePassed += Time.deltaTime;

        Debug.Log("Enemy Vel " + agent.velocity.magnitude);
        animator.SetFloat("Speed", agent.velocity.magnitude);

        if(timePassed >= attackCD && !dead)
        {
            if(Vector3.Distance(player.transform.position, transform.position) <= attackRange)
            {
                canMove = false;
                animator.SetTrigger("Attack");

                timePassed = 0;
            }
        }

        if (Vector3.Distance(player.transform.position, transform.position) <= aggroRange && canMove)
        {
            //transform.LookAt(player.transform);
            agent.destination = player.transform.position;
        }

    }

    void StartDamage()
    {
        damageDealer.dealDamage = true;
    }

    void StopDamage()
    {
        canMove = true;
        damageDealer.dealDamage = false;
    }

    public void PlayImpact()
    {
        int clip = Random.Range(0, impacts.Length);
        source.clip = impacts[clip];
        source.Play();
    }

    public void SpawnImpact()
    {
        Instantiate(spawnCloud, transform.position, Quaternion.LookRotation(Vector3.up));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, aggroRange);
    }

}
