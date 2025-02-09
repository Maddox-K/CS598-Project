using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SavesMenuController : MonoBehaviour
{
    // Menu Management
    private bool isLoadingGame = false;

    // Save Slots
    private SaveSlot[] _saveSlots;
    private Button[] _saveSlotButtons;
    private SaveSlot _pendingOverwrite;

    // Other Menus
    [SerializeField] private Canvas mainMenu;
    [SerializeField] private Canvas savesMenu;
    [SerializeField] private Canvas confirmationMenu;

    // Buttons
    [SerializeField] private Button returnToMainMenu;
    [SerializeField] private Button cancelButton;
    [SerializeField] private Button confirmButton;

    // Animation
    [SerializeField] private Animator _startGameAnimator;

    private void Awake()
    {
        _pendingOverwrite = null;

        _saveSlots = savesMenu.GetComponentsInChildren<SaveSlot>();

        if (_saveSlots != null)
        {
            _saveSlotButtons = new Button[_saveSlots.Length];
            for (int i = 0; i < _saveSlots.Length; i++)
            {
                _saveSlotButtons[i] = _saveSlots[i].GetComponent<Button>();
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        returnToMainMenu.onClick.AddListener(SwitchToMainMenu);
        cancelButton.onClick.AddListener(CancelNewGame);
        confirmButton.onClick.AddListener(ConfirmNewGame);
    }

    public void OnSaveSlotClicked(SaveSlot saveSlot)
    {
        foreach (SaveSlot save in _saveSlots)
        {
            save.SetInteractable(false);
        }
        returnToMainMenu.interactable = false;

        // loading a save file
        if (isLoadingGame)
        {
            GameManager.instance.ChangeSelectedProfileId(saveSlot.GetProfileId());
            StartCoroutine(LoadGameScene());
        }
        // clicking new game on a save file with existing data
        else if (saveSlot.hasData)
        {
            confirmationMenu.gameObject.SetActive(true);
            savesMenu.gameObject.SetActive(false);
            _pendingOverwrite = saveSlot;
        }
        // clicking new game on an empty save file
        else
        {
            GameManager.instance.ChangeSelectedProfileId(saveSlot.GetProfileId());
            GameManager.instance.NewGame();
            StartCoroutine(LoadGameScene());
        }
    }

    private IEnumerator LoadGameScene()
    {
        _startGameAnimator.SetTrigger("EndScene");

        yield return new WaitForSeconds(2.0f);

        SceneManager.LoadScene(GameManager.instance.gameData.lastScene);
    }

    public void ConfirmNewGame()
    {
        GameManager.instance.ChangeSelectedProfileId(_pendingOverwrite.GetProfileId());
        GameManager.instance.NewGame();
        LoadGameScene();
    }

    public void ActivateMenu(bool isLoadingGame)
    {
        this.isLoadingGame = isLoadingGame;

        Dictionary<string, GameData> profilesGameData = GameManager.instance.GetAllProfilesGameData();

        foreach (SaveSlot saveSlot in _saveSlots)
        {
            GameData profileData;
            profilesGameData.TryGetValue(saveSlot.GetProfileId(), out profileData);
            saveSlot.SetData(profileData);
            if (profileData == null && isLoadingGame)
            {
                saveSlot.SetInteractable(false);
            }
            else
            {
                saveSlot.SetInteractable(true);
            }
        }

        Navigation nav = new Navigation();
        nav.mode = Navigation.Mode.Explicit;
        if (!_saveSlotButtons[0].interactable)
        {
            if (!_saveSlotButtons[1].interactable)
            {
                nav.selectOnDown = _saveSlotButtons[2];
            }
            else
            {
                nav.selectOnDown = _saveSlotButtons[1];
            }
            returnToMainMenu.navigation = nav;
        }
    }

    private void SwitchToMainMenu()
    {
        savesMenu.gameObject.SetActive(false);
        mainMenu.gameObject.SetActive(true);
    }

    private void CancelNewGame()
    {
        savesMenu.gameObject.SetActive(true);
        foreach (SaveSlot save in _saveSlots)
        {
            save.SetInteractable(true);
        }
        returnToMainMenu.interactable = true;

        if (_pendingOverwrite != null)
        {
            _pendingOverwrite = null;
        }

        confirmationMenu.gameObject.SetActive(false);
    }
}
