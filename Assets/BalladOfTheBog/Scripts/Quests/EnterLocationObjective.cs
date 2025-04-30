using UnityEngine;

public class EnterLocationObjective : QuestObjective
{
    public override string targetID => _targetLocationID;
    private string _targetLocationID;
    private Quest _parentQuest;

    public EnterLocationObjective(Quest quest, string itemID, bool complete)
    {
        _objectiveType = 2;
        _parentQuest = quest;
        _targetLocationID = itemID;
        isComplete = complete;
        description = $"Enter the location: {itemID}";
    }

    public override void Initialize()
    {
        QuestEvents.OnLocationVisited += OnLocationVisited;
    }

    public override void Cleanup()
    {
        QuestEvents.OnLocationVisited -= OnLocationVisited;
    }

    private void OnLocationVisited(string id)
    {
        if (id == _targetLocationID && !isComplete)
        {
            isComplete = true;
            Debug.Log("Enter location objective complete!");
            Cleanup();
            _parentQuest.CheckForCompletion();
        }
    }
}
