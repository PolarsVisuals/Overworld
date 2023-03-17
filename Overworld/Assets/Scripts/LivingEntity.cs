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
    public SkinnedMeshRenderer[] meshs;

    private float currentHealth;

    public bool dead;

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
            if(gameObject.GetComponent<PlayerCombat>())
            {
                GetComponent<PlayerMovement>().animator.SetTrigger("Death");
                GetComponent<PlayerMovement>().canMove = false;
                GetComponent<PlayerMovement>().canJump = false;
                GetComponent<PlayerCombat>().isDead = true;
                GetComponent<Grappling>().canGrapple = false;
                Destroy(gameObject, 10f);
            }

            dead = true;
        }
        else if(currentHealth > health)
        {
            currentHealth = health;
        }
        else if (currentHealth < 0)
        {
            currentHealth = 0;
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
        if (GetComponent<Enemy>())
        {
            //SpawnFloatingText(amount);
        }
        StartCoroutine(FlickerMesh());
    }

    IEnumerator FlickerMesh()
    {
        foreach (SkinnedMeshRenderer mesh in meshs)
        {
            mesh.enabled = false;
        }
        yield return new WaitForSeconds(0.1f);
        foreach (SkinnedMeshRenderer mesh in meshs)
        {
            mesh.enabled = true;
        }
    }
}
