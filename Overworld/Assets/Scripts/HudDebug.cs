using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HudDebug : MonoBehaviour
{
    public TextMeshProUGUI playerSpeed;
    public TextMeshProUGUI playerVelocity;
    public TextMeshProUGUI playerDrag;
    public TextMeshProUGUI canMove;
    public TextMeshProUGUI canJump;
    public TextMeshProUGUI grounded;
    public TextMeshProUGUI grappleCooldown;
    public TextMeshProUGUI attackTimer;

    Rigidbody playerRigidbody;
    PlayerMovement pmScript;
    PlayerCombat pcScript;
    Grappling grapplingScript;


    private void Start()
    {
        playerRigidbody = GameObject.Find("Player").GetComponent<Rigidbody>();
        pmScript = GameObject.Find("Player").GetComponent<PlayerMovement>();
        pcScript = GameObject.Find("Player").GetComponent<PlayerCombat>();
        grapplingScript = GameObject.Find("Player").GetComponent<Grappling>();
    }

    private void Update()
    {
        playerSpeed.text = "Speed: " + playerRigidbody.velocity.magnitude;
        playerVelocity.text = "Velocity: " + playerRigidbody.velocity;
        playerDrag.text = "Drag: " + playerRigidbody.drag;

        if (!pmScript.canMove)
        {
            canMove.text = "Move: 0";
        }
        else
        {
            canMove.text = "Move: 1";
        }

        if (!pmScript.canJump)
        {
            canJump.text = "Jump: 0";
        }
        else
        {
            canJump.text = "Jump: 1";
        }

        if (!pmScript.grounded)
        {
            grounded.text = "Grounded: 0";
        }
        else
        {
            grounded.text = "Grounded: 1";
        }

        grappleCooldown.text = "Grapple Cooldown: " + grapplingScript.grapplingCdTimer;

        //attackTimer.text = "Attack Timer: " + pcScript.comboTimer;
    }
}
