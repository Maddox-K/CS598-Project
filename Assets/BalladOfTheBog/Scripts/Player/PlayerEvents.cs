using System;

public static class PlayerEvents
{
    // actions
    public static event Action<int> ActivateControls;
    public static event Action<int> DeactivateControls;

    // action invocations
    public static void InvokeActivate(int type)
    {
        ActivateControls?.Invoke(type);
    }
    public static void InvokeDeactivate(int type)
    {
        DeactivateControls?.Invoke(type);
    }
}
