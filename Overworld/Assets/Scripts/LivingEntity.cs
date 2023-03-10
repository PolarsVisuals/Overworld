using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LivingEntity : MonoBehaviour
{
    public float health;

    public Image healthbar;
    public TextMeshProUGUI healthText;
    public Enemy enemyScript;

    private float currentHealth;

    bool dead;

    private void Start()
    {
        currentHealth = health;
        dead = false;
    }

    private void Update()
    {
        if(currentHealth <= 0 && !dead)
        {
            if (gameObject.GetComponent<Enemy>())
            {
                GetComponent<Enemy>().animator.SetTrigger("Death");
                GetComponent<CapsuleCollider>().enabled = false;
                GetComponent<Enemy>().canMove = false;
                GetComponent<Enemy>().dead = true;
                Destroy(gameObject, 5f);
            }
            else
            {
                Destroy(gameObject);
            }

            dead = true;
        }
        else if(currentHealth > health)
        {
            currentHealth = health;
        }
    }

    private void LateUpdate()
    {
        if(healthbar != null)
        {
            healthbar.fillAmount = currentHealth / health;
        }
        if(healthText != null)
        {
            healthText.text = currentHealth.ToString();
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth = currentHealth - amount;
    }
}
