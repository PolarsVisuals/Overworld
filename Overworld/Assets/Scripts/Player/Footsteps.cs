using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footsteps : MonoBehaviour
{
    public AudioClip LandingAudioClip;
    public AudioClip[] FootstepAudioClips;
    [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

    public GameObject particles;

    PlayerCombat playerCombat;

    private void Start()
    {
        playerCombat = GetComponentInParent<PlayerCombat>();
        if(playerCombat == null)
        {
            Debug.Log("Cant find");
        }
    }


    private void OnFootstep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            if (FootstepAudioClips.Length > 0)
            {
                var index = Random.Range(0, FootstepAudioClips.Length);
                AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.position, FootstepAudioVolume);
                Instantiate(particles, transform.position, Quaternion.LookRotation(Vector3.up));
            }
        }
    }

    private void OnLand(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            AudioSource.PlayClipAtPoint(LandingAudioClip, transform.position, FootstepAudioVolume);
        }
    }

    public void PlaySwordSwing()
    {
        playerCombat.PlaySwordSwing();
    }
}
