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

    private PlayerController _playerController;

    private PopUpMenuControls pauseMenuControls;
    public InputAction escape;
    private string _currentSceneName;

    private void Awake()
    {
        pauseMenuControls = new PopUpMenuControls();
    }

    private void OnEnable()
    {
        escape = pauseMenuControls.PopUpMenu.Escape;
        escape.Enable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        escape.Disable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _currentSceneName = scene.name;
    }

    void Start()
    {
        _playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

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
            if (PauseMenu.activeInHierarchy)
            {
                ClosePauseMenu();
            }
            else
            {
                OpenPauseMenu();
            }
        }
    }

    //Call this by pressing escape
    private void OpenPauseMenu()
    {
        PauseMenu.SetActive(true);
        if (_playerController != null)
        {
            _playerController.interact.Disable();
        }
        GameManager.instance.Pause();
    }

    //We call this by eithe pressinf resume or esc
    private void ClosePauseMenu()
    {
        PauseMenu.SetActive(false);
        if (_playerController != null)
        {
            _playerController.interact.Enable();
        }
        GameManager.instance.Unpause();
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
        Time.timeScale = 1;
        if (_currentSceneName == "BattleTest")
        {
            EncounterManager.instance.StopAllCoroutines();
            EncounterManager.instance.SetBattleEnd();
        }
        else
        {
            GameManager.instance.SaveGame();
        }
        SceneManager.LoadScene("Main Menu");
        //ceneManager.LoadScene(""); This will need to vary since we can be in different scenes. So we mus detetct where we are
        //SceneManager.UnloadSceneAsync("Scene1");
    }


}
