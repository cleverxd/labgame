using UnityEngine;

public class KeyPlacer : MonoBehaviour
{
    private PlayerMovement controller;
    private DoorController doorController;
    private Material keyPlacerMat;

    public Transform doorTransform;
    public Material lockedMaterial;
    public Material unlockedMaterial;

    public bool isUnlocked = false;

    private void Start()
    {
        keyPlacerMat = GetComponent<MeshRenderer>().materials[0];
        controller = FindFirstObjectByType<PlayerMovement>();
        doorController = doorTransform.GetComponent<DoorController>();

        keyPlacerMat = isUnlocked ? unlockedMaterial : lockedMaterial;
        GetComponent<MeshRenderer>().material = keyPlacerMat;

    }

    public void UnlockPlacer()
    {
        isUnlocked = true;

        keyPlacerMat = isUnlocked ? unlockedMaterial : lockedMaterial;
        GetComponent<MeshRenderer>().material = keyPlacerMat;
        doorController.ManipulateWithDoor();
    }
}
