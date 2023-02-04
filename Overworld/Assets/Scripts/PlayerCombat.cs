using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

public class PlayerCombat : MonoBehaviour
{
    private Animator animator;
    private StarterAssetsInputs starterAssetsInputs;

    public bool weaponDrawn;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
        weaponDrawn = false;
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
}
