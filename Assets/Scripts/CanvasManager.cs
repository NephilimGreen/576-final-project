using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class CanvasManager : MonoBehaviour
{
    public GameObject playerUI;
    public GameObject hotbarPanel;

    public GameObject chestUI;
    public GameObject submitButton;
    public GameObject closeButton;
    public GameObject equationPanel;
    public GameObject chestPanel;

    public GameObject pauseMenu;
    public static bool isPaused = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    void PauseGame()
    {
        pauseMenu.SetActive(true);
        isPaused = true;
        Time.timeScale = 0.0f;
        // unlock the cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void ResumeGame()
    {
        pauseMenu.SetActive(false);
        chestUI.SetActive(false);
        isPaused = false;
        Time.timeScale = 1.0f;
        // lock the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // this is the function called by the Resume button on the Pause Menu UI
    public void ResumeButton()
    {
        ResumeGame();
    }

    // this is the function called by the Main Menu button on the Pause Menu UI
    public void MainMenuButton()
    {
        // TODO: change this to return to the main menu
        ResumeGame();
    }

    public void OpenChestMenu()
    {
        if (!isPaused)
        {
            isPaused = true;
            chestUI.SetActive(true);
            Time.timeScale = 0.0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
