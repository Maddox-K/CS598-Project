using UnityEngine;
using System.Collections.Generic;

public class Quest
{
    public string questName;
    public List<QuestObjective> objectives = new List<QuestObjective>();
    public bool IsComplete;
    public QuestReward reward;

    public Quest(string name)
    {
        questName = name;
    }

    public void AddObjective(QuestObjective obj)
    {
        objectives.Add(obj);
    }

    public void StartQuest()
    {
        foreach (var obj in objectives)
        {
            if (!obj.isComplete)
            {
                obj.Initialize();
            }
        }

        Debug.Log($"Started quest: {questName}");
    }

    public void CheckForCompletion()
    {
        bool flag = true;

        foreach (QuestObjective objective in objectives)
        {
            if (!objective.isComplete)
            {
                flag = false;
                break;
            }
        }
        
        if (flag)
        {
            IsComplete = true;
            Debug.Log("Quest complete");
            QuestEvents.OnQuestCompleted?.Invoke(this);
        }
    }
}
