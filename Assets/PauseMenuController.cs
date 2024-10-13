using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField] private GameObject PauseMenu;

    [SerializeField] private Button playButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button saveButton;
    [SerializeField] private Button mainMenuButton;

    [SerializeField] private GameObject SettingsMenu;

    [SerializeField] private Button returnToPause;

    private PopUpMenuControls pauseMenuControls;
    private InputAction escape;

    private void Awake()
    {
        pauseMenuControls = new PopUpMenuControls();
    }

    private void OnEnable()
    {
        escape = pauseMenuControls.PopUpMenu.Escape;
        escape.Enable();
    }

    private void OnDisable()
    {
        escape.Disable();
    }

    void Start()
    {
        playButton.onClick.AddListener(ClosePauseMenu);
        settingsButton.onClick.AddListener(OpenSettingsMenu);
        returnToPause.onClick.AddListener(CloseSettingsMenu);
        //saveButton.onClick.AddListener();
        mainMenuButton.onClick.AddListener(ReturnToMainMenu);
    }

    void Update()
    {
        if(escape.WasPressedThisFrame())
        {
            OpenPauseMenu(); 
        }
        else if(escape.WasPressedThisFrame() && PauseMenu.activeInHierarchy)
        {
            ClosePauseMenu();
        }

    }

    //Call this by pressing escape
    private void OpenPauseMenu()
    {
        PauseMenu.SetActive(true);
    }

    //We call this by eithe pressinf resume or esc
    private void ClosePauseMenu()
    {
        PauseMenu.SetActive(false);
    }

    private void SaveGame()
    {

    }

    private void OpenSettingsMenu()
    {
        PauseMenu.SetActive(false);
        SettingsMenu.SetActive(true);
    }

    private void CloseSettingsMenu()
    {
        PauseMenu.SetActive(true);
        SettingsMenu.SetActive(false);
    }

    private void ReturnToMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
        //ceneManager.LoadScene(""); This will need to vary since we can be in different scenes. So we mus detetct where we are
        SceneManager.UnloadSceneAsync("Scene1");
    }


}
