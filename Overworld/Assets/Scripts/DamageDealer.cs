using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    public string theTag;
    public float damage;

    public Collider hitBox;
    public bool dealDamage;

    private void Start()
    {
        dealDamage = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.tag);
        if (other.tag != theTag)
        {
            {
                if (dealDamage)
                {
                    if (other.gameObject.GetComponent<LivingEntity>())
                    {
                        Debug.Log("Hit " + other.name);
                        LivingEntity leScript = other.GetComponent<LivingEntity>();

                        leScript.TakeDamage(damage);
                    }
                }
            }
        }
    }
}

