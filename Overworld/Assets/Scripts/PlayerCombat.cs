using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

public class PlayerCombat : MonoBehaviour
{
    float timePassed;
    float clipLength;
    float clipSpeed;

    [SerializeField] GameObject weapon;
    [SerializeField] GameObject weaponHolder;
    [SerializeField] GameObject sheathHolder;

    GameObject currentWeaponInHand;
    GameObject currentWeaponInSheath;

    private Animator animator;
    private StarterAssetsInputs starterAssetsInputs;

    private bool weaponDrawn;
    private bool isAttacking;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
        weaponDrawn = false;
        isAttacking = false;
    }

    private void Start()
    {
        currentWeaponInSheath = Instantiate(weapon, sheathHolder.transform);
    }

    private void Update()
    {
        if (starterAssetsInputs.draw)
        {
            //Debug.Log("Running");
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

        if (starterAssetsInputs.shoot)
        {
            GetComponent<ThirdPersonController>().canMove = false;

            InitiateAttack();

            starterAssetsInputs.shoot = false;
        }

        if(isAttacking)
        {
            timePassed += Time.deltaTime;
            clipLength = animator.GetCurrentAnimatorClipInfo(1)[0].clip.length;
            clipSpeed = animator.GetCurrentAnimatorStateInfo(1).speed;

            if(timePassed >= clipLength / clipSpeed && starterAssetsInputs.shoot)
            {
                animator.SetTrigger("attack");
            }
            if(timePassed >= clipLength / clipSpeed)
            {
                animator.applyRootMotion = false;
                animator.SetTrigger("canMove");
                GetComponent<ThirdPersonController>().canMove = true;
                isAttacking = false;
            }
        }
    }

    public void InitiateAttack()
    {
        animator.applyRootMotion = true;
        timePassed = 0f;
        animator.SetTrigger("attack");
        animator.SetFloat("Speed", 0f);

        isAttacking = true;
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

    public void NewEvent()
    {
        Debug.Log("Delete blank event.");
    }
}
