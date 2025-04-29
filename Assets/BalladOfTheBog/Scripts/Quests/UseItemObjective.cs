using UnityEngine;

public class UseItemObjective : QuestObjective
{
    public override string targetID => _targetItemID;
    private string _targetItemID;
    private Quest _parentQuest;

    public UseItemObjective(Quest quest, string itemID, bool complete)
    {
        _objectiveType = 5;
        _parentQuest = quest;
        _targetItemID = itemID;
        isComplete = complete;
        description = $"Use {itemID} in inventory";
    }

    public override void Initialize()
    {
        QuestEvents.OnItemUsed += OnItemUsed;
    }

    public override void Cleanup()
    {
        QuestEvents.OnItemUsed -= OnItemUsed;
    }

    private void OnItemUsed(string id)
    {
        if (id == _targetItemID && !isComplete)
        {
            isComplete = true;
            Debug.Log("Use item objective complete!");
            Cleanup();
            _parentQuest.CheckForCompletion();
        }
    }
}
