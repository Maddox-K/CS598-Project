using System;

public static class PauseEvents
{
    // Actions
    public static event Action<int> EnablePopupControls;
    public static event Action<int> DisablePopupControls;

    // Action Invocations
    public static void InvokeEnablePopup(int type)
    {
        EnablePopupControls?.Invoke(type);
    }

    public static void InvokeDisablePopup(int type)
    {
        DisablePopupControls?.Invoke(type);
    }
}
