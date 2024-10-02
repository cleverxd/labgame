using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSetup : MonoBehaviour
{
    public int framerate = 120;
    private void Awake()
    {
        Application.targetFrameRate = framerate;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
