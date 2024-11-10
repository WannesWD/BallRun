using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }
    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    
    public void LevelSelect(int levelNr)
    {
        int sceneIdx = SceneManager.GetActiveScene().buildIndex + levelNr;


        if (sceneIdx < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(sceneIdx);
    }
}
