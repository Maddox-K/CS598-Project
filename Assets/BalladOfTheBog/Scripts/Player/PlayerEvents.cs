using System;

public static class PlayerEvents
{
    // sound actions
    public static event Action OnObjectEaten;

    // control actions
    public static event Action<int> ActivateControls;
    public static event Action<int> DeactivateControls;

    // door actions
    public static event Action<Door> OnDoorOpened;
    public static event Action OnLockedDoorFailed;

    // encounter actions
    public static event Action<int> OnHealActivated;

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

    public static void InvokeHealActivated(int amount)
    {
        OnHealActivated?.Invoke(amount);
    }

    public static void InvokeFailedLockedDoor()
    {
        OnLockedDoorFailed?.Invoke();
    }

    public static void InvokeObjectEaten()
    {
        OnObjectEaten?.Invoke();
    }
}
