using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] float health = 100;

    Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
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
        Destroy(this.gameObject);
    }
}
