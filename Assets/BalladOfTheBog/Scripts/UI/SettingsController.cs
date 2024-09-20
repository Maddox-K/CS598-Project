using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    [SerializeField] private Canvas MainMenu;
    [SerializeField] private Canvas SettingsMenu;

    [SerializeField] private Button returnToMainMenu;
    [SerializeField] private Button graphicsButton;
    [SerializeField] private Button soundButton;
    [SerializeField] private Button controlsButton;
    [SerializeField] private Button creditsButton;

    [SerializeField] private GameObject GraphicsPanel;
    [SerializeField] private GameObject SoundPanel;
    [SerializeField] private GameObject ControlsPanel;
    [SerializeField] private GameObject CreditsPanel;

    bool graphicsButtonClicked;
    bool soundButtonClicked;
    bool creditsButtonClicked;
    bool controlsButtonClicked;

    // Start is called before the first frame update
    void Start()
    {
        returnToMainMenu.onClick.AddListener(SwitchToMainMenu);

        graphicsButton.onClick.AddListener(() =>
        {
            GraphicsPanel.SetActive(true);
            SoundPanel.SetActive(false);
            ControlsPanel.SetActive(false);
            CreditsPanel.SetActive(false);
        });
        soundButton.onClick.AddListener(() =>
        {
            GraphicsPanel.SetActive(false);
            SoundPanel.SetActive(true);
            ControlsPanel.SetActive(false);
            CreditsPanel.SetActive(false);
        });
        controlsButton.onClick.AddListener(() =>
        {
            GraphicsPanel.SetActive(false);
            SoundPanel.SetActive(false);
            ControlsPanel.SetActive(true);
            CreditsPanel.SetActive(false);
        });
        creditsButton.onClick.AddListener(() =>
        {
            GraphicsPanel.SetActive(false);
            SoundPanel.SetActive(false);
            ControlsPanel.SetActive(false);
            CreditsPanel.SetActive(true);
        });


    }

    bool showMainMenu;
    private void SwitchToMainMenu()
    {
        if (!showMainMenu)
        {
            SettingsMenu.gameObject.SetActive(false);
            MainMenu.gameObject.SetActive(true);
        }
    }
}
