using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : MonoBehaviour
{
    public float health;

    private float currentHealth;

    private void Start()
    {
        currentHealth = health;
    }

    private void Update()
    {
        if(currentHealth <= 0)
        {
            Destroy(gameObject);
        }
        else if(currentHealth > health)
        {
            currentHealth = health;
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth = currentHealth - amount;
    }
}
