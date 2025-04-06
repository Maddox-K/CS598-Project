using UnityEngine;

public class CollectObjective : QuestObjective
{
    private string _targetItemID;
    private int _requiredAmount;
    private int _currentAmount;
    private Quest _parentQuest;

    public CollectObjective(Quest quest, string itemID, int amount)
    {
        _parentQuest = quest;
        _targetItemID = itemID;
        _requiredAmount = amount;
        description = $"Collect {_requiredAmount} {_targetItemID}s";
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
            _currentAmount++;
            Debug.Log($"Collected {_currentAmount}/{_requiredAmount} {itemID}");

            if (_currentAmount >= _requiredAmount)
            {
                isComplete = true;
                Cleanup();
                _parentQuest.CheckForCompletion();
                Debug.Log("Collect objective complete!");
            }
        }
    }
}
