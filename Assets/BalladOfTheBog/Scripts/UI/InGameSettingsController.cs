using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InGameSettingsController : MonoBehaviour
{
    // Menus
    private GameObject _settingsMenu;
    private GameObject _pauseMenuHolder;

    // Buttons
    [SerializeField] private Button _returnToPause;
    [SerializeField] private Button _generalButton;
    [SerializeField] private Button _soundButton;
    [SerializeField] private Button _controlsButton;
    [SerializeField] private Button _creditsButton;

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

    // Audio
    private AudioSource _audioSource;
    [SerializeField] private AudioClip _buttonClip;

    void Awake()
    {
        _settingsMenu = gameObject;

        if (PlayerPrefs.GetInt("AutoSave") == 0)
        {
            _offCheck.SetActive(true);
        }
        else
        {
            _onCheck.SetActive(true);
        }

        _audioSource = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _pauseMenuHolder = GameObject.FindGameObjectWithTag("PauseMenu").transform.GetChild(0).gameObject;

        _returnToPause.onClick.AddListener(ReturnToPauseMenu);

        _generalButton.onClick.AddListener(() =>
        {
            PlayButtonAudio();

            _generalPanel.SetActive(true);
            _soundPanel.SetActive(false);
            _controlsPanel.SetActive(false);
            _creditsPanel.SetActive(false);
        });
        _soundButton.onClick.AddListener(() =>
        {
            PlayButtonAudio();

            _generalPanel.SetActive(false);
            _soundPanel.SetActive(true);
            _controlsPanel.SetActive(false);
            _creditsPanel.SetActive(false);
        });
        _controlsButton.onClick.AddListener(() =>
        {
            PlayButtonAudio();

            _generalPanel.SetActive(false);
            _soundPanel.SetActive(false);
            _controlsPanel.SetActive(true);
            _creditsPanel.SetActive(false);
        });
        _creditsButton.onClick.AddListener(() =>
        {
            PlayButtonAudio();

            _generalPanel.SetActive(false);
            _soundPanel.SetActive(false);
            _controlsPanel.SetActive(false);
            _creditsPanel.SetActive(true);
        });

        _autoSaveOn.onClick.AddListener(ActivateAutoSave);
        _autoSaveOff.onClick.AddListener(DeactivateAutoSave);
    }

    private void ReturnToPauseMenu()
    {
        PlayButtonAudio();

        _generalPanel.SetActive(false);
        _soundPanel.SetActive(false);
        _controlsPanel.SetActive(false);
        _creditsPanel.SetActive(false);

        _pauseMenuHolder.SetActive(true);
        _settingsMenu.SetActive(false);

        PauseEvents.InvokeEnablePopup(0);
    }

    private void ActivateAutoSave()
    {
        PlayButtonAudio();

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
        PlayButtonAudio();

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
        _returnToPause.interactable = false;
        _autoSaveOn.interactable = false;
        _autoSaveOff.interactable = false;

        yield return new WaitForSecondsRealtime(0.25f);

        _returnToPause.interactable = true;
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

    private void PlayButtonAudio()
    {
        _audioSource.PlayOneShot(_buttonClip);
    }
}
