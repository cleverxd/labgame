using System.Collections;
using Unity.AI.Navigation;
using UnityEngine;
using TMPro;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    private CharacterController characterController;

    [Header("Movement and rotation")]
    public float walkSpeed = 3f;
    public float runSpeed = 5f;
    public float rotationSpeed = 100f;
    public float jumpHeight = 0.3f; // Jump height
    private float gravity = -9.81f; // Gravity
    private float verticalRotation = 0f;
    private Vector3 velocity; // Velocity for movement, including gravity

    [Header("Player settings")]
    public bool canMove = true;
    public bool canRotate = true;
    public bool canJump = false;
    public bool canUseFlashlight = false;
    [HideInInspector] public bool isRunning;

    [Header("Audio")] 
    public AudioSource audioSource;   // AudioSource component to play the sounds
    public AudioClip footstepSound1;  // First footstep sound
    public AudioClip footstepSound2;  // Second footstep sound
    public AudioClip activateKeycardAllow;  // Allow keycard interact sound
    public AudioClip activateKeycardDeny;  // Deny keycard interact sound
    public AudioClip pickupKeycardSound;  // Keycard pickup sound

    [Header("Footsteps settings")]
    public float footstepIntervalWalk = 0.4f; // Walk Time interval between footsteps
    public float footstepIntervalRun = 0.3f; // Run Time interval between footsteps
    private float footstepTimer = 0f; // Timer to control footstep sounds
    private float footstepInterval;

    [Header("Interact")]
    public float interactionRange = 2f;

    [HideInInspector] public int cardsCollectedYellow = 0;
    [HideInInspector] public int cardsCollectedRed = 0;

    [Header("Other")]
    public Light flashlight;
    public TMP_Text yellowCards;
    public TMP_Text redCards;
    public NavMeshSurface navmesh; // Reference to the NavMeshSurface

    private void Awake()
    {
        Application.targetFrameRate = 120;
    }

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        footstepInterval = footstepIntervalWalk;
        UpdateCardsUI();
    }

    void Update()
    {
        isRunning = Input.GetKey(KeyCode.LeftShift);
        MovePlayer();
        Interact();
        Flashlight();

        // Handle footstep sounds
        HandleFootsteps();
    }

    private void HandleFootsteps()
    {
        // Check if the player is moving
        if (characterController.velocity.magnitude > 0 && canMove)
        {
            footstepTimer += Time.deltaTime; // Increment the timer

            // Play a footstep sound at intervals
            if (footstepTimer >= footstepInterval)
            {
                PlayFootstepSound();
                footstepTimer = 0f; // Reset the timer
            }
        }
        else
        {
            footstepTimer = 0f; // Reset the timer when the player stops
        }
    }

    private void PlayFootstepSound()
    {
        // Randomly choose between the two footstep sounds
        AudioClip footstepSound = (Random.Range(0, 2) == 0) ? footstepSound1 : footstepSound2;
        audioSource.PlayOneShot(footstepSound); // Play the chosen footstep sound
    }

    private void LateUpdate()
    {
        RotatePlayer();
    }

    void MovePlayer()
    {
        if (canMove)
        {
            Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
            Vector3 moveDirection = transform.TransformDirection(moveInput.normalized);
            float speed = (isRunning && moveInput.z >= 0) ? runSpeed : walkSpeed;
            footstepInterval = isRunning ? footstepIntervalRun : footstepIntervalWalk;

            // Handle jumping
            if (characterController.isGrounded)
            {
                // Reset vertical velocity when grounded
                velocity.y = 0;

                if (canJump && Input.GetButtonDown("Jump"))
                {
                    velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity); // Calculate jump velocity
                }
            }

            velocity.y += gravity * Time.deltaTime; // Apply gravity
            characterController.Move((moveDirection * speed + velocity) * Time.deltaTime);
        }
    }

    void RotatePlayer()
    {
        if (canRotate)
        {
            float horizontalRotation = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            float verticalInput = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

            // Calculate the target horizontal rotation
            Quaternion targetHorizontalRotation = Quaternion.Euler(0, horizontalRotation, 0);

            // Smoothly rotate the player horizontally using Slerp
            transform.rotation = Quaternion.Slerp(transform.rotation, transform.rotation * targetHorizontalRotation, Time.deltaTime * rotationSpeed);

            Camera mainCamera = Camera.main;

            // Handle vertical camera rotation
            verticalRotation -= verticalInput;
            verticalRotation = Mathf.Clamp(verticalRotation, -80f, 80f);

            // Smoothly rotate the camera vertically using Slerp
            Quaternion targetVerticalRotation = Quaternion.Euler(verticalRotation, 0, 0);
            mainCamera.transform.localRotation = Quaternion.Slerp(mainCamera.transform.localRotation, targetVerticalRotation, Time.deltaTime * rotationSpeed);
        }
    }


    private void Interact()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Camera mainCamera = Camera.main;
            Ray ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));
            Debug.DrawRay(ray.origin, ray.direction * interactionRange, Color.red, 1f);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, interactionRange))
            {
                if (hit.collider.CompareTag("KeyPlacerYellow"))
                {
                    KeyPlacer keyPlacer = hit.collider.GetComponent<KeyPlacer>();

                    if (cardsCollectedYellow > 0 && !keyPlacer.isUnlocked)
                    {
                        cardsCollectedYellow--;
                        audioSource.PlayOneShot(activateKeycardAllow);
                        UpdateCardsUI();
                        keyPlacer.UnlockPlacer();
                    }
                    else
                    {
                        if (!keyPlacer.isUnlocked)
                            audioSource.PlayOneShot(activateKeycardDeny);
                    }
                }
                else if (hit.collider.CompareTag("KeyPlacerRed"))
                {
                    KeyPlacer keyPlacer = hit.collider.GetComponent<KeyPlacer>();

                    if (cardsCollectedRed > 0 && !keyPlacer.isUnlocked)
                    {
                        cardsCollectedRed--;
                        audioSource.PlayOneShot(activateKeycardAllow);
                        UpdateCardsUI();
                        keyPlacer.UnlockPlacer();
                    }
                    else
                    {
                        if (!keyPlacer.isUnlocked)
                            audioSource.PlayOneShot(activateKeycardDeny);
                    }
                }
                else if (hit.collider.CompareTag("KeycardYellow"))
                {
                    hit.collider.gameObject.SetActive(false);
                    audioSource.PlayOneShot(pickupKeycardSound);
                    cardsCollectedYellow++;
                    UpdateCardsUI();
                }
                else if (hit.collider.CompareTag("KeycardRed"))
                {
                    hit.collider.gameObject.SetActive(false);
                    audioSource.PlayOneShot(pickupKeycardSound);
                    cardsCollectedRed++;
                    UpdateCardsUI();
                }
            }
        }
    }

    private void UpdateCardsUI()
    {
        yellowCards.text = "x" + cardsCollectedYellow;
        redCards.text = "x" + cardsCollectedRed;
    }

    private void Flashlight()
    {
        if (canUseFlashlight && canMove && canRotate)
        {
            if (Input.GetKeyDown(KeyCode.F))
                flashlight.enabled = !flashlight.enabled;
        }
    }

    public IEnumerator ClearAndRebuildNavmesh()
    {
        NavMeshAgent[] agents = FindObjectsOfType<NavMeshAgent>();
        foreach (var agent in agents)
        {
            agent.enabled = false;
        }

        // Clear the existing NavMesh
        NavMesh.RemoveAllNavMeshData();

        // Wait for a frame to ensure everything is processed
        yield return new WaitForEndOfFrame();

        // Rebuild the NavMesh
        navmesh.BuildNavMesh();

        // Re-enable the agents
        foreach (var agent in agents)
        {
            agent.enabled = true;
        }
    }
}
