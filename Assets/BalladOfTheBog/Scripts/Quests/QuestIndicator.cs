using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestIndicator : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _questText;
    [SerializeField] private TextMeshProUGUI _miscText;

    void OnEnable()
    {
        QuestEvents.OnCurrentQuestLoaded += LoadData;
        QuestEvents.OnNewQuestStarted += ShowQuestNotification;
        QuestEvents.ResetUIText += ResetUIText;
        QuestEvents.OnQuestCompleted += OnQuestCompleted;
    }

    void OnDisable()
    {
        QuestEvents.OnCurrentQuestLoaded -= LoadData;
        QuestEvents.OnNewQuestStarted -= ShowQuestNotification;
        QuestEvents.ResetUIText -= ResetUIText;
        QuestEvents.OnQuestCompleted -= OnQuestCompleted;
    }

    private void OnQuestCompleted(Quest quest)
    {
        if (_miscText != null)
        {
            StartCoroutine(ShowCongratsText());
        }
    }

    private void ResetUIText()
    {
        _questText.text = "No Current Objective";
    }

    private void ShowQuestNotification(Quest quest)
    {
        StartCoroutine(ShowText(quest));
    }

    private IEnumerator ShowText(Quest quest)
    {
        _questText.text = "New Objective";

        for (int i = 0; i < 3; i++)
        {
            _questText.color = Color.yellow;
            yield return new WaitForSeconds(0.5f);

            _questText.color = Color.red;
            yield return new WaitForSeconds(0.5f);
        }

        _questText.color = Color.white;
        LoadData(quest);
    }

    private IEnumerator ShowCongratsText()
    {
        _miscText.text = "Objective Complete!";

        yield return new WaitForSeconds(2.0f);

        _miscText.text = "";
    }

    public void LoadData(Quest quest)
    {
        _questText.text = quest.questName;
    }
}
