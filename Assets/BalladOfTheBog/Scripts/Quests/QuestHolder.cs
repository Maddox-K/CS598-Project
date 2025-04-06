using UnityEngine;

public class QuestHolder : MonoBehaviour
{
    [SerializeField] private string _activatorQuestName;
    [SerializeField] private QuestData _thisQuestData;

    [SerializeField] private QuestData _nextQuestData;

    void OnEnable()
    {
        QuestEvents.OnQuestCompleted += OnQuestCompleted;
    }

    void OnDisable()
    {
        QuestEvents.OnQuestCompleted -= OnQuestCompleted;
    }

    private void OnQuestCompleted(string quest)
    {
        if (_activatorQuestName != null && quest == _activatorQuestName)
        {
            QuestEvents.ActivateQuest?.Invoke(_nextQuestData);
        }
    }
}
