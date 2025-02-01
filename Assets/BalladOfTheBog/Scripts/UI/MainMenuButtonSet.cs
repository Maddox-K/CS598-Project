using UnityEngine;
using UnityEngine.UI;

public class MainMenuButtonSet : MenuButtonSet
{
    [SerializeField] private Button _overrideFirstSelected;

    void OnEnable()
    {
        _firstSelected.Select();
    }

    void Start()
    {
        if (GameManager.instance._selectedProfileId == null)
        {
            _firstSelected = _overrideFirstSelected;
            _firstSelected.Select();
        }
    }
}
