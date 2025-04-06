using UnityEngine;

public abstract class QuestObjective
{
    protected byte _objectiveType;
    public string description;
    public virtual string targetID { get; }
    public bool isComplete { get; protected set; }

    public abstract void Initialize();
    public abstract void Cleanup();

    public byte GetObjectiveType()
    {
        return _objectiveType;
    }
}
