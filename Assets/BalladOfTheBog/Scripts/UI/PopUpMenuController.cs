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

    // Player
    private PlayerController _playerController;
    private Rigidbody2D player;

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
        escape = pauseMenuControls.PopUpMenu.Escape;
        StartCoroutine(EnablePause());
        navigate = pauseMenuControls.PopUpMenu.Navigate;
        navigate.Enable();
        SceneManager.sceneLoaded += OnSceneLoaded;

        //May need to move this line back to start. (Weird bug)
        player = GameObject.Find("Player").GetComponent<Rigidbody2D>();

        tab = pauseMenuControls.PopUpMenu.Tab;
        tab.Enable();
    }

    private void OnDisable()
    {
        escape.Disable();
        navigate.Disable();
        SceneManager.sceneLoaded -= OnSceneLoaded;

        tab.Disable();
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

        _playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

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

        //player = GameObject.Find("Player").GetComponent<Rigidbody2D>();
        
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

        if (player.linearVelocity.magnitude > 0.1f)
        {
            CloseInventory();
        }

        //if (rightButtonClick.WasPerformedThisFrame())
        //{
        //    if (Inventory.activeInHierarchy)
        //    {
        //        CloseInventory();
        //    }
        //}
    }

    //Call this by pressing escape
    private void OpenPauseMenu()
    {
        PlayButtonAudio();

        Inventory.SetActive(false);
        _pauseMenuHolder.SetActive(true);
        if (_playerController != null)
        {
            _playerController.interact.Disable();
            _playerController.move.Disable();
        }
        GameManager.instance.Pause();
    }

    //We call this by eithe pressinf resume or esc
    private void ClosePauseMenu()
    {
        PlayButtonAudio();

        if (!tab.enabled)
        {
            tab.Enable();
        }

        _pauseMenuHolder.SetActive(false);
        if (_playerController != null)
        {
            _playerController.interact.Enable();
            _playerController.move.Enable();
        }
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

        Time.timeScale = 1;
        
        GameManager.instance.SaveGame(false);

        StartCoroutine(AnimateReturn());
    }

    private void ReturnToMainMenu()
    {
        PlayButtonAudio();

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

        if (_currentSceneName == "BattleTest")
        {
            if (_playerController != null)
            {
                _playerController.move.Disable();
                GameManager.instance.Pause();
            }
        }
        else
        {
            if (_playerController != null)
            {
                _playerController.interact.Disable();
            }
        }
    }

    private void CloseInventory()
    {
        if (_currentSceneName == "BattleTest")
        {
            _playerController.move.Enable();
            GameManager.instance.Unpause();
        }
        else
        {
            _playerController.interact.Enable();
        }

        Inventory.SetActive(false);
    }

    private IEnumerator EnablePause()
    {
        yield return new WaitForSeconds(1.1f);

        escape.Enable();
    }
}
