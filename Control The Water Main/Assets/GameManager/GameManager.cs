using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Cinemachine;

public class GameManager : MonoBehaviour
{
    bool isPaused = false;
    bool onTitleScreen = true;

    public string defaultScene = "REALAfrica";
    public CinemachineVirtualCamera playerCam;
    public GameObject pauseMenu;
    public GameObject TitleScreen;
    public GameObject HUD;

    void Start()
    {
        Cursor.visible = true;
        Time.timeScale = 0.00000000001f;
    }

    void Update()
    {
        if (Input.GetKeyDown("p") && !onTitleScreen)
        {
            TogglePause();
        }

        if (onTitleScreen || isPaused)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void StartGame()
    {
        onTitleScreen = false;
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        TitleScreen.SetActive(false);
        HUD.SetActive(true);
        playerCam.Priority = 1000;
    }

    public void Respawn()
    {
        Debug.Log("Respawn");
        SceneManager.LoadScene(defaultScene);
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0.00000000001f : 1;
        pauseMenu.SetActive(isPaused);
        if (isPaused)
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
