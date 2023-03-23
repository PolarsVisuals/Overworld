using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    string theTag;
    public float damage;

    public Collider hitBox;
    public bool dealDamage;

    private void Start()
    {
        dealDamage = false;
        theTag = gameObject.tag;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != theTag)
        {
            if (dealDamage)
            {
                if (other.gameObject.GetComponent<LivingEntity>())
                {
                    Debug.Log("Hit " + other.name);
                    LivingEntity leScript = other.GetComponent<LivingEntity>();

                    if (gameObject.GetComponentInParent<PlayerCombat>())
                    {
                        gameObject.GetComponentInParent<PlayerCombat>().PlaySwordHit();
                    }
                    if (gameObject.GetComponentInParent<Enemy>())
                    {
                        gameObject.GetComponentInParent<Enemy>().PlayImpact();
                        dealDamage = false;
                    }

                    leScript.TakeDamage(damage);
                }
            }
        }
    }
}


