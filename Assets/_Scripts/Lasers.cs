using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lasers : MonoBehaviour
{
    public GameObject lasersHolder;

    private PlayerAudio playerAudio;
    private EndScript endScript;

    public bool allowToMoveTrough = false;

    private void Awake()
    {
        playerAudio = FindFirstObjectByType<PlayerAudio>();
        endScript = FindFirstObjectByType<EndScript>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !allowToMoveTrough) // Make sure it's the player triggering and he's not allow to walk trough
        {
            playerAudio.audioSource.PlayOneShot(playerAudio.playerDying);
            StartCoroutine(endScript.LoseByLaser());
        }
    }
}
