using UnityEngine;

[CreateAssetMenu(menuName = "Quests/Quest Data")]
public class QuestData : ScriptableObject
{
    public string questName;

    public ObjectiveData[] objectives;
}
