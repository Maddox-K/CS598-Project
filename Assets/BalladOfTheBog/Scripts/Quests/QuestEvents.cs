using System;

public static class QuestEvents
{
    // general quest actions
    public static Action<Quest> OnQuestCompleted;
    public static Action<QuestData> ActivateQuest;
    public static Action<string> TryStartSubsequentQuest;

    // actions for each quest type
    public static Action<string> OnItemCollected;
    public static Action<string> OnLocationVisited;
    public static Action<string> OnNPCTalkedTo;
    public static Action<string> OnPuzzleCompleted;
    public static Action<string> OnEncounterWon;

    // quest completion consequences
    public static Action<int> RewardCoins;
    public static Action<string[]> RewardItems;
    public static Action<string> ClearObstacle;
}
