using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    public float damage;

    BoxCollider hitBox;
    public bool dealDamage;

    private void Start()
    {
        hitBox = GetComponent<BoxCollider>();
        dealDamage = false;
    }

    private void Update()
    {
        if (dealDamage)
        {
            hitBox.isTrigger = true;
        }
        else
        {
            hitBox.isTrigger = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<LivingEntity>())
        {
            Debug.Log("Hit");
            LivingEntity leScript = other.GetComponent<LivingEntity>();

            leScript.TakeDamage(damage);
        }
    }
}
