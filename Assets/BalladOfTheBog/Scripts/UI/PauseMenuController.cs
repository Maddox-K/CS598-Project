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
    [SerializeField] private Button mainMenuNoSave;

    [SerializeField] private GameObject SettingsMenu;

    [SerializeField] private Button returnToPause;

    private PlayerController _playerController;

    private PopUpMenuControls pauseMenuControls;
    public InputAction escape;
    public InputAction navigate;
    private string _currentSceneName;
    private GameObject _currentlySelected;

    private void Awake()
    {
        //_currentlySelected = EventSystem.current.currentSelectedGameObject;
        pauseMenuControls = new PopUpMenuControls();
    }

    private void OnEnable()
    {
        //EventSystem.current.SetSelectedGameObject(playButton.gameObject);
        escape = pauseMenuControls.PopUpMenu.Escape;
        escape.Enable();
        navigate = pauseMenuControls.PopUpMenu.Navigate;
        navigate.Enable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        
        escape.Disable();
        navigate.Disable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _currentSceneName = scene.name;
        if (scene.name == "BattleTest")
        {
            Image saveButtonImage = saveButton.GetComponent<Image>();
            Color newColor = saveButtonImage.color;
            newColor.a = 0.5f;
            saveButtonImage.color = newColor;

            Image exitSaveButtonImage = mainMenuButton.GetComponent<Image>();
            newColor = exitSaveButtonImage.color;
            newColor.a = 0.5f;
            exitSaveButtonImage.color = newColor;
        }
    }

    void Start()
    {
        _playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        playButton.onClick.AddListener(ClosePauseMenu);
        settingsButton.onClick.AddListener(OpenSettingsMenu);
        returnToPause.onClick.AddListener(CloseSettingsMenu);
        if (_currentSceneName != "BattleTest")
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
        //EventSystem.current.SetSelectedGameObject(_currentlySelected);
        PauseMenu.SetActive(false);
        if (_playerController != null)
        {
            _playerController.interact.Enable();
        }
        GameManager.instance.Unpause();
    }

    private void PauseManuSave()
    {
        Debug.Log("Pause Menu Save called");
        GameManager.instance.SaveGame(false);
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

    private void ReturnToMainMenuAndSave()
    {
        Time.timeScale = 1;
        if (_currentSceneName == "BattleTest")
        {
            EncounterManager.instance.StopAllCoroutines();
            EncounterManager.instance.SetBattleEnd();
        }
        else
        {
            GameManager.instance.SaveGame(false);
        }
        SceneManager.LoadScene("Main Menu");
        //ceneManager.LoadScene(""); This will need to vary since we can be in different scenes. So we mus detetct where we are
        //SceneManager.UnloadSceneAsync("Scene1");
    }

    private void ReturnToMainMenu()
    {
        Debug.Log("Return to Main Menu called");
        Time.timeScale = 1;
        if (_currentSceneName == "BattleTest")
        {
            EncounterManager.instance.StopAllCoroutines();
            EncounterManager.instance.SetBattleEnd();
        }
        SceneManager.LoadScene("Main Menu");
    }
}
