using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] GameObject weapon;
    [SerializeField] GameObject weaponHolder;
    [SerializeField] GameObject sheathHolder;

    GameObject currentWeaponInHand;
    GameObject currentWeaponInSheath;

    private Animator animator;
    private StarterAssetsInputs starterAssetsInputs;

    private bool weaponDrawn;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
        weaponDrawn = false;
    }

    private void Start()
    {
        currentWeaponInSheath = Instantiate(weapon, sheathHolder.transform);
    }

    private void Update()
    {
        if (starterAssetsInputs.draw)
        {
            Debug.Log("Running");
            if (!weaponDrawn)
            {
                animator.SetTrigger("drawWeapon");
                weaponDrawn = true;
            }
            else
            {
                animator.SetTrigger("sheathWeapon");
                weaponDrawn = false;
            }

            starterAssetsInputs.draw = false;
        }
    }

    public void DrawWeapon()
    {
        currentWeaponInHand = Instantiate(weapon, weaponHolder.transform);
        Destroy(currentWeaponInSheath);
    }

    public void SheathWeapon()
    {
        currentWeaponInSheath = Instantiate(weapon, sheathHolder.transform);
        Destroy(currentWeaponInHand);
    }
}
