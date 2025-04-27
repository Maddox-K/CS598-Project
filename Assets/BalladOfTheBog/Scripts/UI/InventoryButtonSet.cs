using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Controls;
using System.Linq;
using UnityEngine.EventSystems;

public class InventoryButtonSet : MenuButtonSet
{
    private InventoryController _controller;
    private GameObject _player;

    void OnEnable()
    {
        InputSystem.onEvent += OnInputEvent;

        ActivateIndicator();
    }

    void OnDisable()
    {
        InputSystem.onEvent -= OnInputEvent;
    }

    void OnInputEvent(InputEventPtr eventPtr, InputDevice device)
    {
        bool buttonPressed = device.allControls.Any(control => control is ButtonControl button && !(button.device is Mouse) && button.IsPressed());

        if (buttonPressed && EventSystem.current.currentSelectedGameObject == null)
        {
            ActivateIndicator();
        }
    }

    void Start()
    {
        _firstSelected.transform.GetChild(0).gameObject.SetActive(true);
    }

    private void ActivateIndicator()
    {
        if (_player == null)
        {
            _player = GameObject.FindGameObjectWithTag("Player");
            _controller = _player.GetComponent<InventoryController>();
        }

        if (_controller.isFulll[0])
        {
            _firstSelected.gameObject.transform.GetChild(1).GetComponent<Button>().Select();
            _firstSelected.gameObject.transform.GetChild(1).transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            _firstSelected.Select();
            _firstSelected.transform.GetChild(0).gameObject.SetActive(true);
        }
    }
}
