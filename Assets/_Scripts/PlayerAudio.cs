using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    private CharacterController characterController;
    private PlayerMovement playerMovement;

    [Header("Tuning")]
    public AudioSource audioSource;

    [Header("Player footsteps")]
    public AudioClip footstepSound1;
    public AudioClip footstepSound2;
    public float footstepIntervalWalk = 0.4f;
    public float footstepIntervalRun = 0.3f;
    private float footstepsInterval = 0f;
    private float footstepTimer = 0f;

    [Header("Player dying")]
    public AudioClip playerDying;

    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        HandleFootsteps();
    }

    private void HandleFootsteps()
    {
        footstepsInterval = playerMovement.isRunning ? footstepIntervalRun : footstepIntervalWalk;
        if (characterController.velocity.magnitude > 0)
        {
            footstepTimer += Time.deltaTime;
            if (footstepTimer >= footstepsInterval)
            {
                PlayFootstepSound();
                footstepTimer = 0f;
            }
        }
        else
        {
            footstepTimer = 0f;
        }
    }

    private void PlayFootstepSound()
    {
        AudioClip footstepSound = (Random.Range(0, 2) == 0) ? footstepSound1 : footstepSound2;
        audioSource.PlayOneShot(footstepSound);
    }
}
