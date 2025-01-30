using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlot : MonoBehaviour
{
    [SerializeField] private string _profileId = "";

    // content of save slot
    [SerializeField] private GameObject _noDataContent;
    [SerializeField] private GameObject _hasDataContent;
    [SerializeField] private TextMeshProUGUI _coinsCollectedText;

    [SerializeField] private SavesMenuController _savesController;

    // navigation settings to be restored when necessary
    [SerializeField] private Button _onUp;
    [SerializeField] private Button _onRight;
    [SerializeField] private Button _onLeft;
    private Navigation _enabledNavigation;

    private Button _saveSlotButton;

    private void Awake()
    {
        _saveSlotButton = GetComponent<Button>();

        _enabledNavigation = new Navigation();
        _enabledNavigation.mode = Navigation.Mode.Explicit;
        _enabledNavigation.selectOnUp = _onUp;
        _enabledNavigation.selectOnRight = _onRight;
        _enabledNavigation.selectOnLeft = _onLeft;
    }

    void Start()
    {
        _saveSlotButton.onClick.AddListener(() => _savesController.OnSaveSlotClicked(this));
    }

    public void SetData(GameData data)
    {
        if (data == null)
        {
            _noDataContent.SetActive(true);
            _hasDataContent.SetActive(false);
        }
        else
        {
            _noDataContent.SetActive(false);
            _hasDataContent.SetActive(true);

            _coinsCollectedText.text = "COINS:" + data.coinCount.ToString();
        }
    }

    public string GetProfileId()
    {
        return this._profileId;
    }

    public void SetInteractable(bool interactable)
    {
        _saveSlotButton.interactable = interactable;

        if (!interactable)
        {
            Navigation nav = new Navigation();
            nav.mode = Navigation.Mode.None;
            _saveSlotButton.navigation = nav;
        }
        else
        {
            _saveSlotButton.navigation = _enabledNavigation;
        }
    }
}
