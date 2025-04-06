using UnityEngine;
using System.Collections.Generic;
using System;

public class QuestManager : MonoBehaviour
{
    private List<Quest> activeQuests = new List<Quest>();
    private Quest _currentActiveQuest;

    void OnEnable()
    {
        QuestEvents.ActivateQuest += ActivateQuest;
    }

    void OnDisable()
    {
        QuestEvents.ActivateQuest -= ActivateQuest;
    }

    private void Start()
    {
        Quest quest = new Quest("Pick Up Letter");

        quest.AddObjective(new CollectObjective(quest, "Mail", 1));

        quest.StartQuest();
        activeQuests.Add(quest);
        _currentActiveQuest = quest;
    }

    private void ActivateQuest(QuestData questData)
    {
        Quest quest = new Quest(questData.questName);

        ObjectiveData[] objectives = questData.objectives;

        for (int i = 0; i < objectives.Length; i++)
        {
            byte objectiveType = objectives[i].objectiveType;
            string objectId = objectives[i].objectId;
            int amount = objectives[i].amountNeeded;

            switch (objectiveType)
            {
                case 0:
                    quest.AddObjective(new CollectObjective(quest, objectId, amount));
                    break;
            }
        }

        quest.StartQuest();
        activeQuests.Add(quest);
        _currentActiveQuest = quest;
    }
}
