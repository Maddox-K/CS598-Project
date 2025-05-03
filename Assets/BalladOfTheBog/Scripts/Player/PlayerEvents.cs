using System;

public static class PlayerEvents
{
    // actions
    public static event Action<int> ActivateControls;
    public static event Action<int> DeactivateControls;
    public static event Action<Door> OnDoorOpened;

    // action invocations
    public static void InvokeActivate(int type)
    {
        ActivateControls?.Invoke(type);
    }
    public static void InvokeDeactivate(int type)
    {
        DeactivateControls?.Invoke(type);
    }
    public static void InvokeDoorOpen(Door door)
    {
        OnDoorOpened?.Invoke(door);
    }
}
