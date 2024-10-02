using System.Collections;
using Unity.AI.Navigation;
using UnityEngine;
using TMPro;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController characterController;

    [Header("Tuning")]
    public float walkSpeed = 3f;
    public float runSpeed = 5f;
    public float jumpHeight = 2f;
    public float rotationSpeed = 100f;
    private float gravity = -9.81f;
    private float verticalRotation = 0f;
    private Vector3 velocity;

    [Header("Bools")]
    public bool canMove = true;
    public bool canRotate = true;
    public bool canJump = false;
    public bool isRunning;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        isRunning = Input.GetKey(KeyCode.LeftShift);
        MovePlayer();
    }

    private void LateUpdate()
    {
        RotatePlayer();
    }

    void MovePlayer()
    {
        if (!canMove) return;

        Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        Vector3 moveDirection = transform.TransformDirection(moveInput.normalized);
        float speed = (isRunning && moveInput.z >= 0) ? runSpeed : walkSpeed;

        if (characterController.isGrounded)
        {
            velocity.y = 0;
            if (canJump && Input.GetButtonDown("Jump"))
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        characterController.Move((moveDirection * speed + velocity) * Time.deltaTime);
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

}
