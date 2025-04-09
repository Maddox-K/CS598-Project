using UnityEngine;

// only game objects that are associated with quests will have this component attached to them
// this could be a quest that begins once this object is interacted with or that the game object begins the next quest once a prerequisite has been completed
public class QuestHolder : MonoBehaviour
{
    [SerializeField] private string _activatorQuestName;
    [SerializeField] private QuestData _thisQuestData;

    [SerializeField] private QuestData _nextQuestData; // only objects that initiate a next quest in a chain of quests will have one of these

    void OnEnable()
    {
        QuestEvents.TryStartSubsequentQuest += TryStartSubsequentQuest;
    }

    void OnDisable()
    {
        QuestEvents.TryStartSubsequentQuest -= TryStartSubsequentQuest;
    }

    private void TryStartSubsequentQuest(string questName)
    {
        if (_activatorQuestName != null && questName == _activatorQuestName) // only start subsequent quest if corresponding prerequisite quest was just completed
        {
            QuestEvents.ActivateQuest?.Invoke(_nextQuestData); // send quest data to quest manager for activation
        }
    }
}
