using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyPlacerLasers : MonoBehaviour
{
    public Lasers lasers;

    private Material keyPlacerMat;

    public GameObject lasersHolder;
    public Material lockedMaterial;
    public Material unlockedMaterial;

    public bool isUnlocked = false;

    private void Start()
    {
        keyPlacerMat = GetComponent<MeshRenderer>().materials[0];

        keyPlacerMat = isUnlocked ? unlockedMaterial : lockedMaterial;
        GetComponent<MeshRenderer>().material = keyPlacerMat;
    }

    public void UnlockPlacerLasers()
    {
        isUnlocked = true;
        lasers.allowToMoveTrough = true;

        keyPlacerMat = isUnlocked ? unlockedMaterial : lockedMaterial;
        GetComponent<MeshRenderer>().material = keyPlacerMat;
        lasersHolder.SetActive(false);
    }
}
