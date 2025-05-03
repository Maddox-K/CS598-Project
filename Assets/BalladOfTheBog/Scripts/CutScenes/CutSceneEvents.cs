using System;

public static class CutSceneEvents
{
    public static event Action<string> OnLocationEntered;

    public static void InvokeLocationEntered(string id)
    {
        OnLocationEntered?.Invoke(id);
    }
}
