using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlot : MonoBehaviour
{
    [SerializeField] private string _profileId = "";

    [SerializeField] private GameObject _noDataContent;
    [SerializeField] private GameObject _hasDataContent;
    [SerializeField] private TextMeshProUGUI _coinsCollectedText;

    [SerializeField] private SavesMenuController _savesController;

    private Button _saveSlotButton;

    private void Awake()
    {
        _saveSlotButton = this.GetComponent<Button>();
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
    }
}
