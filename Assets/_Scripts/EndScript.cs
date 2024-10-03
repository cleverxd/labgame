using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class EndScript : MonoBehaviour
{
    public Light[] lights; // Array of lights
    public PlayerMovement playerMovement;
    public UIController uiController;
    public Timer timerScript;
    public float rotationResetDuration = 0.5f; // Duration for rotation reset
    public float lightTurnOffInterval = 0.15f; // Interval between turning off each light
    public Volume volume; // Post-processing volume
    

    private ColorAdjustments colorAdjustments;
    private bool colorAdjustmentsActive;
    private bool exposureAdjusted = false; // Prevents resetting after adjustment

    private void Start()
    {
        // Check if the volume has Color Adjustments
        if (volume.profile.TryGet(out colorAdjustments))
        {
            colorAdjustmentsActive = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Make sure it's the player triggering
        {
            playerMovement.canMove = false;
            playerMovement.canRotate = false;
            playerMovement.canJump = false;
            timerScript.StopTimer();
            // Start rotation reset
            StartCoroutine(ResetCameraAndPlayerRotation());

            // Start turning off lights one by one and adjusting post-exposure during 4th and 5th lights
            StartCoroutine(TurnOffLightsAndAdjustExposure());
        }
    }

    private IEnumerator ResetCameraAndPlayerRotation()
    {
        uiController.crosshair.SetActive(false);
        uiController.cards.SetActive(false);
        float elapsedTime = 0f;

        // Get initial rotations
        Quaternion initialPlayerRotation = playerMovement.transform.rotation;
        Quaternion initialCameraRotation = Camera.main.transform.localRotation;

        // Define target rotations
        Quaternion targetPlayerRotation = Quaternion.Euler(0, 90, 0);
        Quaternion targetCameraRotation = Quaternion.Euler(0, 0, 0);

        // Gradually interpolate from initial rotations to target rotations
        while (elapsedTime < rotationResetDuration)
        {
            // Interpolate player rotation
            playerMovement.transform.rotation = Quaternion.Slerp(initialPlayerRotation, targetPlayerRotation, elapsedTime / rotationResetDuration);

            // Interpolate camera rotation
            Camera.main.transform.localRotation = Quaternion.Slerp(initialCameraRotation, targetCameraRotation, elapsedTime / rotationResetDuration);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure final rotations are set exactly
        playerMovement.transform.rotation = targetPlayerRotation;
        Camera.main.transform.localRotation = targetCameraRotation;
    }

    private IEnumerator TurnOffLightsAndAdjustExposure()
    {
        float targetExposure = -20f;
        float initialExposure = colorAdjustmentsActive ? colorAdjustments.postExposure.value : 0f;

        for (int i = 0; i < lights.Length; i++)
        {
            lights[i].enabled = false; // Turn off the current light

            if (i < lights.Length - 1)
                lights[i].GetComponent<AudioSource>().Play();

            // Adjust post-exposure only during 4th and 5th light turning off, and only once
            if (colorAdjustmentsActive && (i == 3 || i == 4) && !exposureAdjusted)
            {
                float elapsedTime = 0f;
                float exposureTransitionDuration = lightTurnOffInterval; // Use the same interval for exposure transition

                while (elapsedTime < exposureTransitionDuration)
                {
                    // Linearly interpolate the post-exposure value from the initial to the target value
                    float newExposure = Mathf.Lerp(initialExposure, targetExposure, elapsedTime / exposureTransitionDuration);
                    colorAdjustments.postExposure.value = newExposure;

                    elapsedTime += Time.deltaTime;
                    yield return null; // Wait for the next frame
                }

                // Ensure final exposure value is applied after the transition
                colorAdjustments.postExposure.value = targetExposure;
                exposureAdjusted = true; // Prevent further changes after adjusting once
            }

            yield return new WaitForSeconds(lightTurnOffInterval); // Wait for the interval before turning off the next light
        }
        uiController.ShowWinLoseScreen();
        uiController.endText.SetActive(true);
    }

    public IEnumerator LoseByEnemy()
    {
        playerMovement.canMove = false;
        playerMovement.canRotate = false;
        playerMovement.canJump = false;

        float targetExposure = -20f;
        float initialExposure = colorAdjustmentsActive ? colorAdjustments.postExposure.value : 0f;
        float exposureTransitionDuration = 0.25f; // Adjust this for a smoother or faster transition
        float elapsedTime = 0f;

        while (elapsedTime < exposureTransitionDuration)
        {
            // Linearly interpolate the post-exposure value from the initial to the target value
            float newExposure = Mathf.Lerp(initialExposure, targetExposure, elapsedTime / exposureTransitionDuration);
            colorAdjustments.postExposure.value = newExposure;

            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        // Ensure the final exposure value is applied after the transition
        colorAdjustments.postExposure.value = targetExposure;
        exposureAdjusted = true; // Prevent further changes after adjusting once

        uiController.killedByEnemyText.SetActive(true);
        uiController.killedByEnemy = true;
        uiController.ShowWinLoseScreen();
    }
    public IEnumerator LoseByLaser()
    {
        playerMovement.canMove = false;
        playerMovement.canRotate = false;
        playerMovement.canJump = false;

        float targetExposure = -20f;
        float initialExposure = colorAdjustmentsActive ? colorAdjustments.postExposure.value : 0f;
        float exposureTransitionDuration = 0.25f; // Adjust this for a smoother or faster transition
        float elapsedTime = 0f;

        while (elapsedTime < exposureTransitionDuration)
        {
            // Linearly interpolate the post-exposure value from the initial to the target value
            float newExposure = Mathf.Lerp(initialExposure, targetExposure, elapsedTime / exposureTransitionDuration);
            colorAdjustments.postExposure.value = newExposure;

            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        // Ensure the final exposure value is applied after the transition
        colorAdjustments.postExposure.value = targetExposure;
        exposureAdjusted = true; // Prevent further changes after adjusting once

        uiController.killedByLaserText.SetActive(true);
        uiController.killedByLasers = true;
        uiController.ShowWinLoseScreen();
    }
}
