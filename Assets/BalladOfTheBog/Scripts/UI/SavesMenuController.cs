using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SavesMenuController : MonoBehaviour
{
    // Save Slots
    private SaveSlot[] _saveSlots;
    private Button[] _saveSlotButtons;
    private bool isLoadingGame = false;

    // Other Menus
    [SerializeField] private Canvas mainMenu;
    [SerializeField] private Canvas savesMenu;

    [SerializeField] private Button returnToMainMenu;

    private void Awake()
    {
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
    }

    public void OnSaveSlotClicked(SaveSlot saveSlot)
    {
        foreach (SaveSlot save in _saveSlots)
        {
            save.SetInteractable(false);
        }
        returnToMainMenu.interactable = false;

        GameManager.instance.ChangeSelectedProfileId(saveSlot.GetProfileId());

        if (!isLoadingGame)
        {
            GameManager.instance.NewGame();
        }
        
        SceneManager.LoadScene(GameManager.instance.gameData.lastScene);

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
}
