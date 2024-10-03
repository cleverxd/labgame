using UnityEngine;
using TMPro;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;
using UnityEngine.Audio;

public class SettingsSetup : MonoBehaviour
{
    public TMP_Text resolutionText;   // Reference to the TextMeshPro text component to display resolution
    public TMP_Text ssrText;   // Reference to the TextMeshPro text component to display resolution
    public TMP_Text audioText;   // Reference to the TextMeshPro text component to display resolution
    public TMP_Text fpsText;   // Reference to the TextMeshPro text component to display resolution
    public Volume volume;

    private ScreenSpaceReflections reflections;
    private Resolution[] resolutions; // Array to store available screen resolutions
    private int currentRefreshRate;
    private int currentResolutionIndex = 0; // Tracks the currently selected resolution

    private int currentFpsIndex = 3;
    private float currentAudioLevel = 0;
    private int reflectionsEnabled = 1;

    public AudioMixer mainAudioMixer;        // Reference to the Audio Mixer
    public string exposedParameter = "MasterVolume";

    void Start()
    {
        currentFpsIndex = PlayerPrefs.GetInt("TargetFrameRate", 3);
        
        SetFrameRate(currentFpsIndex);
        SetVolumeAtStart();
        
        resolutions = Screen.resolutions;
        currentRefreshRate = Screen.currentResolution.refreshRate;

        currentResolutionIndex = GetCurrentResolutionIndex();

        DisplayResolution();
        
        if (volume.profile.TryGet(out reflections))
        {
            reflections.enabled.value = true;
        }

        SetSSRAtStart();
    }

    // Changes the resolution based on direction (-1 for previous, 1 for next)
    public void ChangeResolution(int direction)
    {
        // Move to the next or previous resolution
        currentResolutionIndex += direction;

        // Ensure the index wraps around within valid limits
        if (currentResolutionIndex < 0)
            currentResolutionIndex = GetFilteredResolutions().Length - 1;
        else if (currentResolutionIndex >= GetFilteredResolutions().Length)
            currentResolutionIndex = 0;

        // Set the new resolution
        SetResolution(currentResolutionIndex);
    }

    // Sets the screen resolution and updates the display
    private void SetResolution(int resolutionIndex)
    {
        Resolution resolution = GetFilteredResolutions()[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen, resolution.refreshRate);
        DisplayResolution();
    }

    // Displays the current resolution in the TextMeshPro UI
    private void DisplayResolution()
    {
        Resolution resolution = GetFilteredResolutions()[currentResolutionIndex];
        resolutionText.text = resolution.width + " x " + resolution.height;
    }

    // Returns the index of the current screen resolution within the filtered list
    private int GetCurrentResolutionIndex()
    {
        Resolution[] filteredResolutions = GetFilteredResolutions();
        for (int i = 0; i < filteredResolutions.Length; i++)
        {
            if (filteredResolutions[i].width == Screen.currentResolution.width &&
                filteredResolutions[i].height == Screen.currentResolution.height &&
                filteredResolutions[i].refreshRate == Screen.currentResolution.refreshRate)
            {
                return i;
            }
        }
        return 0; // Default to 0 if no exact match is found
    }

    // Filters the resolutions to only include those with the current desktop refresh rate
    private Resolution[] GetFilteredResolutions()
    {
        // Filter the resolutions based on the current desktop's refresh rate
        return System.Array.FindAll(resolutions, res => res.refreshRate == currentRefreshRate);
    }

    public void ToggleSSR()
    {
        reflections.enabled.value = !reflections.enabled.value;
        ssrText.text = reflections.enabled.value ? "Enabled" : "Disabled";
        reflectionsEnabled = reflections.enabled.value ? 1 : 0;
        PlayerPrefs.SetInt("ReflectionsEnabled", reflectionsEnabled);
    }

    private void SetSSRAtStart()
    {
        reflectionsEnabled = PlayerPrefs.GetInt("ReflectionsEnabled", 1);
        reflections.enabled.value = reflectionsEnabled == 1 ? true : false;
        ssrText.text = reflections.enabled.value ? "Enabled" : "Disabled";
    }
    public void ChangeMasterVolume(int direction)
    {
        float currentVolume;
        mainAudioMixer.GetFloat(exposedParameter, out currentVolume);

        // Convert current dB value to a percentage for easier calculation
        float volumePercentage = (currentVolume + 48f) / 48f * 100f;

        // Change the volume by -5% or +5%
        volumePercentage += direction * 5f;

        // Clamp the percentage between 0% and 100%
        volumePercentage = Mathf.Clamp(volumePercentage, 0f, 100f);

        // Convert the percentage back to decibels (from 0% = -48dB to 100% = 0dB)
        currentVolume = (volumePercentage / 100f * 48f) - 48f;

        // Set the new volume in the AudioMixer
        mainAudioMixer.SetFloat(exposedParameter, currentVolume);

        // Display the volume percentage in TMP Text, rounded to nearest integer
        audioText.text = Mathf.RoundToInt(volumePercentage) + "%";

        PlayerPrefs.SetFloat("TargetAudioLevel", currentVolume);
    }

    private void SetVolumeAtStart()
    {
        currentAudioLevel = PlayerPrefs.GetFloat("TargetAudioLevel", 0f);
        mainAudioMixer.SetFloat(exposedParameter, currentAudioLevel);
        float volumePercentage = (currentAudioLevel + 48f) / 48f * 100f;
        volumePercentage = Mathf.Clamp(volumePercentage, 0f, 100f);
        audioText.text = Mathf.RoundToInt(volumePercentage) + "%";
    }
    private void SetFrameRate(int fps)
    {
        currentFpsIndex = fps;

        QualitySettings.vSyncCount = 0;

        if (currentFpsIndex == 0)
        {
            Application.targetFrameRate = 30;
            fpsText.text = "30 FPS";
        }  
        else if (currentFpsIndex == 1)
        {
            Application.targetFrameRate = 60;
            fpsText.text = "60 FPS";
        }
        else if (currentFpsIndex == 2)
        {
            Application.targetFrameRate = 90;
            fpsText.text = "90 FPS";
        }
        else if (currentFpsIndex == 3)
        {
            Application.targetFrameRate = 120;
            fpsText.text = "120 FPS";
        }

        PlayerPrefs.SetInt("TargetFrameRate", currentFpsIndex);
        PlayerPrefs.Save(); // Ensure the data is saved to disk
    }

    public void ChangeFramerate(int direction)
    {
        // Change the index based on direction (-1 or 1)
        currentFpsIndex += direction;

        // Ensure the index wraps around within valid limits
        if (currentFpsIndex < 0)
            currentFpsIndex = 3;
        else if (currentFpsIndex > 3)
            currentFpsIndex = 0;

        // Set the new frame rate based on the new index
        SetFrameRate(currentFpsIndex);
    }
}
