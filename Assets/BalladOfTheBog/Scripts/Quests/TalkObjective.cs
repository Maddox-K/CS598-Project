using UnityEngine;

public class TalkObjective : QuestObjective
{
    public override string targetID => _targetNPCID;
    private string _targetNPCID;
    private Quest _parentQuest;

    public TalkObjective(Quest quest, string itemID, bool complete)
    {
        _objectiveType = 1;
        _parentQuest = quest;
        _targetNPCID = itemID;
        isComplete = complete;
        description = $"Talk to {itemID}";
    }

    public override void Initialize()
    {
        QuestEvents.OnNPCTalkedTo += OnNPCTalkedTo;
    }

    public override void Cleanup()
    {
        QuestEvents.OnNPCTalkedTo -= OnNPCTalkedTo;
    }

    private void OnNPCTalkedTo(string id)
    {
        if (id == _targetNPCID && !isComplete)
        {
            isComplete = true;
            Debug.Log("Talk to NPC objective complete!");
            Cleanup();
            _parentQuest.CheckForCompletion();
        }
    }
}
