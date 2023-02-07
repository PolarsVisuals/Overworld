using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    bool canDealDamage;
    List<GameObject> hasDealthDamage;

    [SerializeField] float weaponLength;
    [SerializeField] float weaponDamage;

    private void Start()
    {
        canDealDamage = false;
        hasDealthDamage = new List<GameObject>();
    }

    private void Update()
    {
        if (canDealDamage)
        {
            RaycastHit hit;
            int layerMask = 1 << 9;

            if (Physics.Raycast(transform.position, -transform.up, out hit, weaponLength, layerMask))
            {
                if (!hasDealthDamage.Contains(hit.transform.gameObject))
                {
                    print("damage");
                    hasDealthDamage.Add(hit.transform.gameObject);
                }
            }
        }
    }

    public void StartDealDamage()
    {
        canDealDamage = true;
        hasDealthDamage.Clear();
    }

    public void EndDealDamage()
    {
        canDealDamage = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position - transform.up * weaponLength);
    }
}
