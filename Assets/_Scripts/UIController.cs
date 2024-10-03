using UnityEngine;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    [Header("Sub elements")]
    public GameObject endText;
    public GameObject killedByEnemyText;
    public GameObject killedByLaserText;
    public GameObject pauseText;
    public GameObject darker;

    [Header("Gameplay elements")]
    public GameObject crosshair;
    public GameObject cards;

    [Header("Menus")]
    public GameObject loseWinMenu;
    public GameObject pauseMenu;
    public GameObject settings;

    [HideInInspector] public bool killedByEnemy;
    [HideInInspector] public bool killedByLasers;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!settings.activeSelf)
                TogglePauseScreen();
            else
                ToggleSettings();
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

    public void ToggleSettings()
    {
        if (killedByEnemy)
        {
            settings.SetActive(!settings.activeSelf);
            loseWinMenu.SetActive(!loseWinMenu.activeSelf);
            killedByEnemyText.SetActive(loseWinMenu.activeSelf);
        }
        else if (killedByLasers)
        {
            settings.SetActive(!settings.activeSelf);
            loseWinMenu.SetActive(!loseWinMenu.activeSelf);
            killedByLaserText.SetActive(loseWinMenu.activeSelf);
        }
        else
        {
            settings.SetActive(!settings.activeSelf);
            pauseMenu.SetActive(!pauseMenu.activeSelf);
            pauseText.SetActive(pauseMenu.activeSelf);
        }

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
