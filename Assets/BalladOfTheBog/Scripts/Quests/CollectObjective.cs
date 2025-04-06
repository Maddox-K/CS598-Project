using UnityEngine;

public class CollectObjective : QuestObjective, ICollectionObjective
{
    public override string targetID => _targetItemID;
    private string _targetItemID;
    public int requiredAmount { get; }
    public int currentAmount { get; set; }
    private Quest _parentQuest;

    public CollectObjective(Quest quest, string itemID, int amount, int currentAmount, bool complete)
    {
        _objectiveType = 0;
        _parentQuest = quest;
        _targetItemID = itemID;
        requiredAmount = amount;
        this.currentAmount = currentAmount;
        isComplete = complete;
        description = $"Collect {requiredAmount} {_targetItemID}s";
    }

    public override void Initialize()
    {
        QuestEvents.OnItemCollected += OnItemCollected;
    }

    public override void Cleanup()
    {
        QuestEvents.OnItemCollected -= OnItemCollected;
    }

    private void OnItemCollected(string itemID)
    {
        if (itemID == _targetItemID && !isComplete)
        {
            currentAmount++;
            Debug.Log($"Collected {currentAmount}/{requiredAmount} {itemID}");

            if (currentAmount >= requiredAmount)
            {
                isComplete = true;
                Debug.Log("Collect objective complete!");
                Cleanup();
                _parentQuest.CheckForCompletion();
            }
        }
    }
}
