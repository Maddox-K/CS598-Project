using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;

public class QuestManager : MonoBehaviour, IDataPersistence
{
    private List<Quest> activeQuests = new List<Quest>();
    private Quest _currentActiveQuest;

    void OnEnable()
    {
        QuestEvents.OnQuestCompleted += OnQuestCompleted;
        QuestEvents.ActivateQuest += ActivateQuest;
    }

    void OnDisable()
    {
        QuestEvents.OnQuestCompleted -= OnQuestCompleted;
        QuestEvents.ActivateQuest -= ActivateQuest;

        foreach (QuestObjective obj in _currentActiveQuest.objectives)
        {
            obj.Cleanup();
        }
    }

    private void Start()
    {
        if (!GameManager.instance.savingLoadingEnabled)
        {
            StartFirstQuest();
        }
    }

    private void OnQuestCompleted(Quest quest)
    {
        _currentActiveQuest = null;

        QuestReward reward = quest.reward;
        if (reward != null)
        {
            if (reward.coins > 0)
            {
                QuestEvents.RewardCoins?.Invoke(reward.coins);
            }
            if (reward.items != null && reward.items[0] != "")
            {
                QuestEvents.RewardItems?.Invoke(reward.items);
            }
            if (reward.clearsObstacle)
            {
                QuestEvents.ClearObstacle?.Invoke(quest.questName);
            }
        }

        QuestEvents.TryStartSubsequentQuest?.Invoke(quest.questName);
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
            int currentAmount = objectives[i].currentAmount;

            switch (objectiveType)
            {
                case 0:
                    quest.AddObjective(new CollectObjective(quest, objectId, amount, currentAmount, false));
                    break;
            }
        }

        if (questData.reward != null)
        {
            quest.reward = questData.reward;
        }

        quest.StartQuest();
        activeQuests.Add(quest);
        _currentActiveQuest = quest;

        QuestEvents.InvokeOnNewQuestStarted(_currentActiveQuest);
    }

    public void LoadData(GameData data)
    {
        if (SceneManager.GetActiveScene().name == "BattleTest")
        {
            return;
        }

        if (data.currentQuest != null)
        {
            _currentActiveQuest = new Quest(data.currentQuest);

            (ObjectiveData[], bool[], bool, QuestReward) questInfo = data.quests[_currentActiveQuest.questName];
            ObjectiveData[] objectives = questInfo.Item1;

            for (int i = 0; i < objectives.Length; i++)
            {
                byte objectiveType = objectives[i].objectiveType;
                string objectId = objectives[i].objectId;
                int amount = objectives[i].amountNeeded;
                int currentAmount = objectives[i].currentAmount;

                switch (objectiveType)
                {
                    case 0:
                        _currentActiveQuest.AddObjective(new CollectObjective(_currentActiveQuest, objectId, amount, currentAmount, questInfo.Item2[i]));
                        break;
                }
            }

            if (questInfo.Item4 != null)
            {
                _currentActiveQuest.reward = questInfo.Item4;
            }

            _currentActiveQuest.StartQuest();
            activeQuests.Add(_currentActiveQuest);
            QuestEvents.InvokeOnCurrentQuestLoaded(_currentActiveQuest);
        }
        else if (!data.startedGameplay)
        {
            StartFirstQuest();
        }
    }

    public void SaveData(GameData data)
    {
        if (SceneManager.GetActiveScene().name == "BattleTest")
        {
            return;
        }

        data.currentQuest = _currentActiveQuest.questName;

        foreach (Quest quest in activeQuests)
        {
            if (!data.quests.ContainsKey(quest.questName))
            {
                SaveQuest(data, quest);
            }
            else if (!data.quests[quest.questName].Item3)
            {
                data.quests.Remove(quest.questName);

                SaveQuest(data, quest);
            }
        }
    }

    private void SaveQuest(GameData data, Quest quest)
    {
        ObjectiveData[] objectives = new ObjectiveData[quest.objectives.Count];
        bool[] completed = new bool[quest.objectives.Count];

        for (int i = 0; i < quest.objectives.Count; i++)
        {
            objectives[i] = new ObjectiveData();
            objectives[i].objectiveType = quest.objectives[i].GetObjectiveType();
            objectives[i].objectId = quest.objectives[i].targetID;
            if (quest.objectives[i] is ICollectionObjective x)
            {
                objectives[i].amountNeeded = x.requiredAmount;
                objectives[i].currentAmount = x.currentAmount;
            }
            else
            {
                objectives[i].amountNeeded = 0;
                objectives[i].currentAmount = 0;
            }

            completed[i] = quest.objectives[i].isComplete;
        }
        data.quests.Add(quest.questName, (objectives, completed, quest.IsComplete, quest.reward));
    }

    private void StartFirstQuest()
    {
        Quest quest = new Quest("Pick Up Letter");

        quest.AddObjective(new CollectObjective(quest, "Mail", 1, 0, false));

        quest.StartQuest();
        activeQuests.Add(quest);
        _currentActiveQuest = quest;

        String[] items = {"TomatoButton"};
        quest.reward = new QuestReward(2, items, false);

        GameManager.instance.gameData.startedGameplay = true;

        QuestEvents.InvokeOnNewQuestStarted(_currentActiveQuest);
    }
}
