using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    // Menus
    [SerializeField] private Canvas MainMenu;
    [SerializeField] private Canvas SettingsMenu;

    // Menu Buttons
    [SerializeField] private Button returnToMainMenu;
    [SerializeField] private Button graphicsButton;
    [SerializeField] private Button soundButton;
    [SerializeField] private Button controlsButton;
    [SerializeField] private Button creditsButton;

    // Sub-Panels
    [SerializeField] private GameObject _generalPanel;
    [SerializeField] private GameObject _soundPanel;
    [SerializeField] private GameObject _controlsPanel;
    [SerializeField] private GameObject _creditsPanel;

    // General Panel Fields
    [SerializeField] private Button _autoSaveOn;
    [SerializeField] private GameObject _onCheck;
    [SerializeField] private Button _autoSaveOff;
    [SerializeField] private GameObject _offCheck;

    void Awake()
    {
        if (!PlayerPrefs.HasKey("AutoSave"))
        {
            PlayerPrefs.SetInt("AutoSave", 1);
        }

        if (PlayerPrefs.GetInt("AutoSave") == 0)
        {
            _offCheck.SetActive(true);
        }
        else
        {
            _onCheck.SetActive(true);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        returnToMainMenu.onClick.AddListener(SwitchToMainMenu);

        graphicsButton.onClick.AddListener(() =>
        {
            _generalPanel.SetActive(true);
            _soundPanel.SetActive(false);
            _controlsPanel.SetActive(false);
            _creditsPanel.SetActive(false);
        });
        soundButton.onClick.AddListener(() =>
        {
            _generalPanel.SetActive(false);
            _soundPanel.SetActive(true);
            _controlsPanel.SetActive(false);
            _creditsPanel.SetActive(false);
        });
        controlsButton.onClick.AddListener(() =>
        {
            _generalPanel.SetActive(false);
            _soundPanel.SetActive(false);
            _controlsPanel.SetActive(true);
            _creditsPanel.SetActive(false);
        });
        creditsButton.onClick.AddListener(() =>
        {
            _generalPanel.SetActive(false);
            _soundPanel.SetActive(false);
            _controlsPanel.SetActive(false);
            _creditsPanel.SetActive(true);
        });

        _autoSaveOn.onClick.AddListener(ActivateAutoSave);
        _autoSaveOff.onClick.AddListener(DeactivateAutoSave);
    }

    private void SwitchToMainMenu()
    {
        _generalPanel.SetActive(false);
        _soundPanel.SetActive(false);
        _controlsPanel.SetActive(false);
        _creditsPanel.SetActive(false);

        SettingsMenu.gameObject.SetActive(false);
        MainMenu.gameObject.SetActive(true);
    }

    private void ActivateAutoSave()
    {
        if (_offCheck.activeSelf)
        {
            StartCoroutine(DelayAutoSaveEdit(true));

            _offCheck.SetActive(false);
            _onCheck.SetActive(true);

            PlayerPrefs.SetInt("AutoSave", 1);
        }
    }

    private void DeactivateAutoSave()
    {
        if (_onCheck.activeSelf)
        {
            StartCoroutine(DelayAutoSaveEdit(false));

            _onCheck.SetActive(false);
            _offCheck.SetActive(true);

            PlayerPrefs.SetInt("AutoSave", 0);
        }
    }

    private IEnumerator DelayAutoSaveEdit(bool settingTrue)
    {
        returnToMainMenu.interactable = false;
        _autoSaveOn.interactable = false;
        _autoSaveOff.interactable = false;

        yield return new WaitForSeconds(0.25f);

        returnToMainMenu.interactable = true;
        _autoSaveOn.interactable = true;
        _autoSaveOff.interactable = true;

        if (settingTrue)
        {
            _autoSaveOn.Select();
        }
        else
        {
            _autoSaveOff.Select();
        }
    }
}
