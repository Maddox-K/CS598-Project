using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    // Main Menu Buttons
    [SerializeField] private Button continueButton;
    [SerializeField] private Button savesButton;
    [SerializeField] private Button newgameButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;

    // Canvases for each sub-menu
    [SerializeField] private Canvas mainMenu;
    [SerializeField] private Canvas settingsMenu;
    [SerializeField] private Canvas savesMenu;

    // Controllers
    private SavesMenuController _savesController = null;

    // Animation
    [SerializeField] private Animator _startGameAnimator;

    void Awake()
    {
        _savesController = GetComponent<SavesMenuController>();
    }

    void Start()
    {
        if (GameManager.instance._selectedProfileId == null)
        {
            continueButton.interactable = false;
            savesButton.interactable = false;
        }
        else
        {
            continueButton.onClick.AddListener(ContinueGameClicked);
            savesButton.onClick.AddListener(SwitchToSavesMenu);
        }
        
        newgameButton.onClick.AddListener(NewGameClicked);
        settingsButton.onClick.AddListener(SwitchToSettingsMenu);
        quitButton.onClick.AddListener(() => Application.Quit());
    }

    private void ContinueGameClicked()
    {
        continueButton.interactable = false;
        savesButton.interactable = false;
        newgameButton.interactable = false;
        settingsButton.interactable = false;
        quitButton.interactable = false;

        StartCoroutine(AnimateGameStart());
    }

    private IEnumerator AnimateGameStart()
    {
        _startGameAnimator.SetTrigger("EndScene");

        yield return new WaitForSeconds(2.0f);

        SceneManager.LoadScene(GameManager.instance.gameData.lastScene);
    }

    private void SwitchToSavesMenu()
    {
        savesMenu.gameObject.SetActive(true);
        if (_savesController != null)
        {
            _savesController.ActivateMenu(true);
        }

        DeactivateMainMenu();
    }

    private void NewGameClicked()
    {
        savesMenu.gameObject.SetActive(true);
        if (_savesController != null)
        {
            _savesController.ActivateMenu(false);
        }
        
        DeactivateMainMenu();
    }

    private void SwitchToSettingsMenu()
    {
        settingsMenu.gameObject.SetActive(true);
        DeactivateMainMenu();
    }

    private void DeactivateMainMenu()
    {
        mainMenu.gameObject.SetActive(false);
    }
}
