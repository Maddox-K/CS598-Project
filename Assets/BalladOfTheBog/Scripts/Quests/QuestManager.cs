using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;

public class QuestManager : MonoBehaviour, IDataPersistence
{
    private List<Quest> activeQuests = new List<Quest>();
    private Quest _currentActiveQuest;
    private AudioSource _audioSource;
    [SerializeField] private AudioClip _questCompleteSound;

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    void OnEnable()
    {
        QuestEvents.OnQuestCompleted += OnQuestCompleted;
        QuestEvents.ActivateQuest += ActivateQuest;
    }

    void OnDisable()
    {
        QuestEvents.OnQuestCompleted -= OnQuestCompleted;
        QuestEvents.ActivateQuest -= ActivateQuest;

        if (_currentActiveQuest != null)
        {
            foreach (QuestObjective obj in _currentActiveQuest.objectives)
            {
                obj.Cleanup();
            }
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
        if (_audioSource != null && _questCompleteSound != null)
        {
            _audioSource.PlayOneShot(_questCompleteSound);
        }

        _currentActiveQuest = null;

        QuestEvents.InvokeTextReset();

        QuestReward reward = quest.reward;
        if (reward != null)
        {
            Debug.Log("Reward found");
            if (reward.coins > 0)
            {
                QuestEvents.RewardCoins?.Invoke(reward.coins);
            }
            if (reward.items != null && reward.items.Length > 0 && reward.items[0] != "")
            {
                QuestEvents.RewardItems?.Invoke(reward.items);
            }
            if (reward.clearsObstacle)
            {
                Debug.Log("Attmepting to Clear Obstacle");
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
                case 1:
                    quest.AddObjective(new TalkObjective(quest, objectId, false));
                    break;
                case 2:
                    quest.AddObjective(new EnterLocationObjective(quest, objectId, false));
                    break;
                case 3:
                    quest.AddObjective(new CompletePuzzleObjective(quest, objectId, false));
                    break;
                case 5:
                    quest.AddObjective(new UseItemObjective(quest, objectId, false));
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
                    case 1:
                        _currentActiveQuest.AddObjective(new TalkObjective(_currentActiveQuest, objectId, questInfo.Item2[i]));
                        break;
                    case 2:
                        _currentActiveQuest.AddObjective(new EnterLocationObjective(_currentActiveQuest, objectId, questInfo.Item2[i]));
                        break;
                    case 3:
                        _currentActiveQuest.AddObjective(new CompletePuzzleObjective(_currentActiveQuest, objectId, questInfo.Item2[i]));
                        break;
                    case 5:
                        _currentActiveQuest.AddObjective(new UseItemObjective(_currentActiveQuest, objectId, questInfo.Item2[i]));
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

        if (_currentActiveQuest != null)
        {
            data.currentQuest = _currentActiveQuest.questName;
        }
        else
        {
            data.currentQuest = null;
        }

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
