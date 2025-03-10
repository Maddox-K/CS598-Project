using UnityEngine;
using UnityEngine.UI;

public class MainMenuButtonSet : MenuButtonSet
{
    [SerializeField] private Button _overrideFirstSelected;

    void OnEnable()
    {
        _firstSelected.Select();
        _firstSelected.transform.GetChild(0).gameObject.SetActive(true);
    }

    void Start()
    {
        if (GameManager.instance._selectedProfileId == null)
        {
            _firstSelected = _overrideFirstSelected;
            _firstSelected.Select();
        }

        _firstSelected.transform.GetChild(0).gameObject.SetActive(true);
    }
}
