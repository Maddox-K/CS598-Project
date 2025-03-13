using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Controls;
using System.Linq;
using UnityEngine.EventSystems;

public class MainMenuButtonSet : MenuButtonSet
{
    [SerializeField] private Button _overrideFirstSelected;

    void OnEnable()
    {
        _firstSelected.Select();
        _firstSelected.transform.GetChild(0).gameObject.SetActive(true);

        InputSystem.onEvent += OnInputEvent;
    }

    void OnDisable()
    {
        InputSystem.onEvent -= OnInputEvent;
    }

    void OnInputEvent(InputEventPtr eventPtr, InputDevice device)
    {
        bool buttonPressed = device.allControls.Any(control => control is ButtonControl button && button.IsPressed());

        if (buttonPressed && EventSystem.current.currentSelectedGameObject == null)
        {
            _firstSelected.Select();
            _firstSelected.transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    void Start()
    {
        if (GameManager.instance._selectedProfileId == null)
        {
            _firstSelected.transform.GetChild(0).gameObject.SetActive(false);
            _firstSelected = _overrideFirstSelected;
            _firstSelected.Select();
        }

        _firstSelected.transform.GetChild(0).gameObject.SetActive(true);
    }
}
