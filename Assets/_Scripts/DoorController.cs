using System.Collections;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    private PlayerController playerController;
    public KeyPlacer[] keyPlacers;
    public Transform[] doorPartsToMove;
    private bool isDoorUnlocked;

    public int keysNeed;
    public int currentKeysInsidePlacers;

    private float doorOpenStep = 2.3f;
    private Vector3[] curPositions;
    private Vector3[] targetPositions;
    public float openDuration = 1f; // Time taken to open the door

    private void Awake()
    {
        playerController = FindFirstObjectByType<PlayerController>();
    }

    private void Start()
    {
        curPositions = new Vector3[doorPartsToMove.Length];
        targetPositions = new Vector3[doorPartsToMove.Length];

        // Store the initial and target positions for all door parts
        for (int i = 0; i < doorPartsToMove.Length; i++)
        {
            curPositions[i] = doorPartsToMove[i].position;
            targetPositions[i] = new Vector3(curPositions[i].x, curPositions[i].y + doorOpenStep, curPositions[i].z);
        }

        ManipulateWithDoor();
    }

    public void ManipulateWithDoor()
    {
        currentKeysInsidePlacers = 0;

        for (int i = 0; i < keyPlacers.Length; i++)
        {
            if (keyPlacers[i].isUnlocked)
                currentKeysInsidePlacers++;
        }

        if (keysNeed == currentKeysInsidePlacers && !isDoorUnlocked)
        {
            isDoorUnlocked = true;
            StartCoroutine(OpenDoor());
        }
    }

    private IEnumerator OpenDoor()
    {
        float elapsedTime = 0f;

        while (elapsedTime < openDuration)
        {
            for (int i = 0; i < doorPartsToMove.Length; i++)
            {
                doorPartsToMove[i].position = Vector3.Lerp(curPositions[i], targetPositions[i], elapsedTime / openDuration);
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure all parts reach the target positions
        for (int i = 0; i < doorPartsToMove.Length; i++)
        {
            doorPartsToMove[i].position = targetPositions[i];
        }

        StartCoroutine(playerController.ClearAndRebuildNavmesh());
    }
}
