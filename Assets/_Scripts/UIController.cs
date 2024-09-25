using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    [Header("Sub elements")]
    public GameObject endText;
    public GameObject killedByEnemyText;
    public GameObject pauseText;
    public GameObject darker;

    [Header("Gameplay elements")]
    public GameObject crosshair;
    public GameObject cards;

    [Header("Menus")]
    public GameObject loseWinMenu;
    public GameObject pauseMenu;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseScreen();
        }
    }

    public void TogglePauseScreen()
    {
        pauseMenu.SetActive(!pauseMenu.activeSelf);

        Cursor.lockState = pauseMenu.activeSelf ? CursorLockMode.Confined : CursorLockMode.Locked;
        Cursor.visible = pauseMenu.activeSelf;

        crosshair.SetActive(!pauseMenu.activeSelf);
        cards.SetActive(!pauseMenu.activeSelf);
        pauseText.SetActive(pauseMenu.activeSelf);
        darker.SetActive(pauseMenu.activeSelf);
        Time.timeScale = pauseMenu.activeSelf ? 0 : 1;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void RestartLevel()
    {
        Time.timeScale = 1;

        // Reload the currently active scene
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    public void ShowWinLoseScreen()
    {
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        crosshair.SetActive(false);
        cards.SetActive(false);
        darker.SetActive(true);
        loseWinMenu.SetActive(true);
    }
}
