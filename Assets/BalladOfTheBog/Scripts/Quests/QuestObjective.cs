using UnityEngine;

public abstract class QuestObjective
{
    public string description;
    public bool isComplete { get; protected set; }

    public abstract void Initialize();
    public abstract void Cleanup();
}
