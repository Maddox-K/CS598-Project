using UnityEngine;

public class CompletePuzzleObjective : QuestObjective
{
    public override string targetID => _targetPuzzleID;
    private string _targetPuzzleID;
    private Quest _parentQuest;

    public CompletePuzzleObjective(Quest quest, string itemID, bool complete)
    {
        _objectiveType = 3;
        _parentQuest = quest;
        _targetPuzzleID = itemID;
        isComplete = complete;
        description = $"Complete the puzzle: {itemID}";
    }

    public override void Initialize()
    {
        QuestEvents.OnPuzzleCompleted += OnPuzzleCompleted;
    }

    public override void Cleanup()
    {
        QuestEvents.OnPuzzleCompleted -= OnPuzzleCompleted;
    }

    private void OnPuzzleCompleted(string id)
    {
        if (id == _targetPuzzleID && !isComplete)
        {
            isComplete = true;
            Debug.Log("Complete puzzle objective complete!");
            Cleanup();
            _parentQuest.CheckForCompletion();
        }
    }
}
