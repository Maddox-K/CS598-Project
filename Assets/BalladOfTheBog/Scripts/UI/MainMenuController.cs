using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button savesButton;
    [SerializeField] private Button settigsButton;

    [SerializeField] private Canvas mainMenu;
    [SerializeField] private Canvas settingsMenu;
    [SerializeField] private Canvas savesMenu;

    private UnityAction OpenSettingsMenu;

    // Start is called before the first frame update
    void Start()
    {
        startButton.onClick.AddListener(LoadGameScene);
        settigsButton.onClick.AddListener(SwitchMenu);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LoadGameScene()
    {
        SceneManager.LoadScene("Scene1");
        SceneManager.UnloadSceneAsync("Main Menu");
    }

    bool showSettingsMenu;
    private void SwitchMenu()
    {
        if(!showSettingsMenu)
        {
            settingsMenu.gameObject.SetActive(true);
        }

        //settingsMenu.gameObject.SetActive(true);
    }

}
