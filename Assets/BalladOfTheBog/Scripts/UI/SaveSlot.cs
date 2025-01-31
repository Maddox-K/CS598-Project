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
    public bool hasData;

    [SerializeField] private SavesMenuController _savesController;

    private Button _saveSlotButton;

    private void Awake()
    {
        _saveSlotButton = GetComponent<Button>();
    }

    void Start()
    {
        _saveSlotButton.onClick.AddListener(() => _savesController.OnSaveSlotClicked(this));
    }

    public void SetData(GameData data)
    {
        if (data == null)
        {
            hasData = false;
            _noDataContent.SetActive(true);
            _hasDataContent.SetActive(false);
        }
        else
        {
            hasData = true;
            _noDataContent.SetActive(false);
            _hasDataContent.SetActive(true);

            _coinsCollectedText.text = "COINS:" + data.coinCount.ToString();
        }
    }

    public string GetProfileId()
    {
        return _profileId;
    }

    public void SetInteractable(bool interactable)
    {
        _saveSlotButton.interactable = interactable;

        Navigation nav = new Navigation();
        if (!interactable)
        {
            nav.mode = Navigation.Mode.None;
        }
        else
        {
            nav.mode = Navigation.Mode.Automatic;
        }
        _saveSlotButton.navigation = nav;
    }
}
