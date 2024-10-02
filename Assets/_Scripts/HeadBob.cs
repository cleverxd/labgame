using UnityEngine;

public class HeadBob : MonoBehaviour
{
    public CharacterController characterController; // Reference to the player's CharacterController
    public PlayerMovement playerMovement;       // Reference to the PlayerController script for checking running state
    public float walkBobSpeed = 7f;                 // Speed of head bobbing while walking
    public float runBobSpeed = 14f;                 // Speed of head bobbing while running
    public float bobAmountHorizontal = 0.05f;       // Amount of horizontal bobbing
    public float bobAmountVertical = 0.05f;         // Amount of vertical bobbing
    public float smoothingFactor = 6f;              // Controls how smooth the transition is

    private float defaultPosY = 0f;                 // Default Y position of the camera
    private float defaultPosX = 0f;                 // Default X position of the camera
    private float timer = 0f;                       // Timer for head bobbing effect
    private Vector3 targetPosition;                 // Target position for interpolation

    private void Start()
    {
        // Store the default camera position at the start
        defaultPosY = transform.localPosition.y;
        defaultPosX = transform.localPosition.x;
        targetPosition = transform.localPosition;   // Initialize target position
    }

    private void Update()
    {
        if (characterController.isGrounded && characterController.velocity.magnitude > 0.1f)
        {
            // Use walk or run bob speed depending on the player's movement state
            float currentBobSpeed = playerMovement.isRunning ? runBobSpeed : walkBobSpeed;

            // Increment the timer for smooth bobbing effect
            timer += Time.deltaTime * currentBobSpeed;

            // Calculate the bobbing effect using sine wave
            float horizontalOffset = Mathf.Sin(timer) * bobAmountHorizontal;
            float verticalOffset = Mathf.Sin(timer * 2) * bobAmountVertical;

            // Calculate the target position for the bobbing effect
            targetPosition = new Vector3(defaultPosX + horizontalOffset, defaultPosY + verticalOffset, transform.localPosition.z);
        }
        else
        {
            // Reset the timer and position when player is not moving
            timer = 0f;
            targetPosition = new Vector3(defaultPosX, defaultPosY, transform.localPosition.z);
        }

        // Smoothly interpolate between current position and the target position
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * smoothingFactor);

        // Ensure that the target position does not contain NaN values
        if (float.IsNaN(targetPosition.x) || float.IsNaN(targetPosition.y))
        {
            targetPosition = new Vector3(defaultPosX, defaultPosY, transform.localPosition.z);
        }
    }
}
