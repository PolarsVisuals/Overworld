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
