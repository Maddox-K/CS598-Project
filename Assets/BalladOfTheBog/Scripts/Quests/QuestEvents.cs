using UnityEngine;
using System;

public static class QuestEvents
{
    public static Action<string> OnQuestCompleted;
    public static Action<QuestData> ActivateQuest;
    
    public static Action<string> OnItemCollected;
    public static Action<string> OnLocationVisited;
    public static Action<string> OnNPCTalkedTo;
    public static Action<string> OnPuzzleCompleted;
    public static Action<string> OnEncounterWon;
}
