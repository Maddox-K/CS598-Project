using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button saveButton;
    [SerializeField] private Button mainMenuButton;

    [SerializeField] private GameObject PauseMenu;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Call this by pressing escape
    private void openPauseMenu()
    {
        PauseMenu.SetActive(true);
    }

    //We call this by eithe pressinf resume or esc
    private void closePauseMenu()
    {
        PauseMenu.SetActive(false);
    }

    private void returnToMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
        //ceneManager.LoadScene(""); This will need to vary since we can be in different scenes. So we mus detetct where we are
        SceneManager.UnloadSceneAsync("Scene1");
    }


}
