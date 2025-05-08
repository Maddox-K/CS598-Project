using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Controls;
using System.Linq;
using UnityEngine.EventSystems;

public class DialogueOptionButtonSet : MenuButtonSet
{
    void OnEnable()
    {
        InputSystem.onEvent += OnInputEvent;
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
            _firstSelected.Select();
            _firstSelected.transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
}
