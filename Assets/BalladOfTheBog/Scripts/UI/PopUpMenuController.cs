using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PopUpMenuController : MonoBehaviour
{
    private string _currentSceneName;

    // Menus
    [SerializeField] private GameObject _pauseMenuHolder;
    private GameObject _settingsMenu;
    [SerializeField] private GameObject Inventory;

    // Buttons
    [SerializeField] private Button playButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button saveButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button mainMenuNoSave;
    [SerializeField] private GameObject[] _buttonHighlighters;

    // Input
    private PopUpMenuControls pauseMenuControls;
    public InputAction escape;
    public InputAction navigate;
    public InputAction tab;
    public InputAction rightButtonClick;

    // Animation
    [SerializeField] private Animator _sceneTransitionAnimator;

    // Sound
    private AudioSource _audioSource;
    [SerializeField] private AudioClip _buttonClip;

    private void Awake()
    {
        pauseMenuControls = new PopUpMenuControls();

        _audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        escape = pauseMenuControls.PopUpMenu.Escape;
        StartCoroutine(EnablePause());
        tab = pauseMenuControls.PopUpMenu.Tab;
        tab.Enable();

        PauseEvents.EnablePopupControls += EnableControls;
        PauseEvents.DisablePopupControls += DisableControls;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        escape.Disable();
        tab.Disable();

        PauseEvents.EnablePopupControls -= EnableControls;
        PauseEvents.DisablePopupControls -= DisableControls;
    }

    private void EnableControls(int type)
    {
        switch (type)
        {
            case 0:
                escape?.Enable();
                break;
            case 1:
                tab?.Enable();
                break;
            case 2:
                escape?.Enable();
                tab?.Enable();
                break;
        }
    }

    private void DisableControls(int type)
    {
        switch (type)
        {
            case 0:
                escape?.Disable();
                break;
            case 1:
                tab?.Disable();
                break;
            case 2:
                escape?.Disable();
                tab?.Disable();
                break;
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _currentSceneName = scene.name;
        if (scene.name == "BattleTest")
        {
            saveButton.interactable = false;
            mainMenuButton.interactable = false;
        }
    }

    void Start()
    {
        _settingsMenu = GameObject.FindGameObjectWithTag("SettingsMenu");
        _settingsMenu.SetActive(false);
        _settingsMenu.GetComponent<Canvas>().enabled = true;

        playButton.onClick.AddListener(ClosePauseMenu);
        settingsButton.onClick.AddListener(OpenSettingsMenu);

        if (!GameManager.instance.savingLoadingEnabled)
        {
            saveButton.interactable = false;
            mainMenuButton.interactable = false;
        }
        if (_currentSceneName != "BattleTest" && GameManager.instance.savingLoadingEnabled)
        {
            saveButton.onClick.AddListener(PauseManuSave);
            mainMenuButton.onClick.AddListener(ReturnToMainMenuAndSave);
        }

        mainMenuNoSave.onClick.AddListener(ReturnToMainMenu);
    }

    void Update()
    {
        if(escape.WasPressedThisFrame())
        {
            if (_pauseMenuHolder.activeInHierarchy)
            {
                ClosePauseMenu();
            }
            else
            {
                OpenPauseMenu();
            }
        }

        if (tab.WasPressedThisFrame())
        {
            if (Inventory.activeInHierarchy)
            {
                CloseInventory();
            }
            else if (!_pauseMenuHolder.activeInHierarchy)
            {
                OpenInventory();
            }
        }
    }

    //Call this by pressing escape/start
    private void OpenPauseMenu()
    {
        PlayButtonAudio();

        Inventory.SetActive(false);
        _pauseMenuHolder.SetActive(true);
        
        PlayerEvents.InvokeDeactivate(2);

        GameManager.instance.Pause();
    }

    //We call this by either pressing resume or esc
    private void ClosePauseMenu()
    {
        PlayButtonAudio();

        if (!tab.enabled)
        {
            tab.Enable();
        }

        _pauseMenuHolder.SetActive(false);

        for (int i = 0; i < _buttonHighlighters.Length; i++)
        {
            _buttonHighlighters[i].SetActive(false);
        }
        
        PlayerEvents.InvokeActivate(2);

        GameManager.instance.Unpause();
    }

    private void PauseManuSave()
    {
        PlayButtonAudio();

        Debug.Log("Pause Menu Save called");
        GameManager.instance.SaveGame(false);
    }

    private void OpenSettingsMenu()
    {
        PlayButtonAudio();

        tab.Disable();
        escape.Disable();

        for (int i = 0; i < _buttonHighlighters.Length; i++)
        {
            _buttonHighlighters[i].SetActive(false);
        }
        
        _pauseMenuHolder.SetActive(false);
        _settingsMenu.SetActive(true);
        Inventory.SetActive(false);
    }

    private void ReturnToMainMenuAndSave()
    {
        PlayButtonAudio();

        escape.Disable();
        tab.Disable();

        Time.timeScale = 1;
        
        GameManager.instance.SaveGame(false);

        StartCoroutine(AnimateReturn());
    }

    private void ReturnToMainMenu()
    {
        PlayButtonAudio();

        escape.Disable();
        tab.Disable();

        Time.timeScale = 1;
        if (_currentSceneName == "BattleTest")
        {
            EncounterManager.instance.StopAllCoroutines();
            EncounterManager.instance.SetBattleEnd(false);

            _sceneTransitionAnimator.gameObject.SetActive(true);
            _sceneTransitionAnimator.enabled = true;
        }
        
        StartCoroutine(AnimateReturn());
    }

    private IEnumerator AnimateReturn()
    {
        if (_sceneTransitionAnimator != null)
        {
            _sceneTransitionAnimator.SetTrigger("EndScene");
        }

        yield return new WaitForSeconds(1.2f);

        SceneManager.LoadScene("Main Menu");
    }

    private void PlayButtonAudio()
    {
        _audioSource.PlayOneShot(_buttonClip);
    }

    private void OpenInventory()
    {
        Inventory.SetActive(true);

        PlayerEvents.InvokeDeactivate(2);

        if (_currentSceneName == "BattleTest")
        {
            GameManager.instance.Pause();
        }
        else
        {
            PlayerEvents.InvokeDeactivate(0);
        }
    }

    private void CloseInventory()
    {
        PlayerEvents.InvokeActivate(2);

        if (_currentSceneName == "BattleTest")
        {
            GameManager.instance.Unpause();
        }
        else
        {
            PlayerEvents.InvokeActivate(0);
        }

        Inventory.SetActive(false);
    }

    private IEnumerator EnablePause()
    {
        yield return new WaitForSeconds(1.2f);

        escape.Enable();
    }
}
