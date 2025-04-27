using System;

public static class InventoryEvents
{
    // actions
    public static event Action OnInventoryClose;

    // action invocations
    public static void InvokeInventoryClose()
    {
        OnInventoryClose?.Invoke();
    }
}
