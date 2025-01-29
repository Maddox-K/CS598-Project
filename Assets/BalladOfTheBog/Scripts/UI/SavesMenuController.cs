using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SavesMenuController : MonoBehaviour
{
    // Save Slots
    private SaveSlot[] _saveSlots;
    private bool isLoadingGame = false;

    // Other Menus
    [SerializeField] private Canvas mainMenu;
    [SerializeField] private Canvas savesMenu;

    [SerializeField] private Button returnToMainMenu;

    private void Awake()
    {
        _saveSlots = savesMenu.GetComponentsInChildren<SaveSlot>();
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
            GameData profileData = null;
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
    }

    private void SwitchToMainMenu()
    {
        savesMenu.gameObject.SetActive(false);
        mainMenu.gameObject.SetActive(true);
    }
}
