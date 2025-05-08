using System.Collections.Generic;

[System.Serializable]
public class GameData
{
    // General Game Data
    public long lastUpdated;

    // Scene Management
    public string lastScene;
    public bool changingScenes;

    // Player Data
    public float[] playerPosition = new float[3];
    public float[] playerRotation = new float[2];
    public int coinCount;

    // Enemies, Encounters and NPCs
    public Dictionary<string, bool> enemiesEncountered;
    public string lastEnemyEncountered; // id of last enemy encountered
    public Dictionary<string, float[]> npcPositions;

    // Collectibles
    public Dictionary<string, bool> coinsCollected;
    public Dictionary<int, string> inventorySlotData;

    // Quests
    public Dictionary<string, (ObjectiveData[], bool[], bool, QuestReward)> quests;
    public string currentQuest;
    public bool startedGameplay;

    // Cutscenes
    public Dictionary<string, bool> cutScenes;

    // Doors
    public Dictionary<string, bool> doorsUnlocked;

    public GameData()
    {
        // Scene Management
        lastScene = "CutSceneOne";
        changingScenes = false;

        // Player Data
        playerPosition[0] = 209.5f;
        playerPosition[1] = -20.0f;
        for (int i = 0; i < 2; i++)
        {
            playerRotation[i] = 0.0f;
        }
        coinCount = 0;

        // Enemies, Encounters and NPCs
        enemiesEncountered = new Dictionary<string, bool>();
        lastEnemyEncountered = null;
        npcPositions = new Dictionary<string, float[]>();

        // Collectibles
        coinsCollected = new Dictionary<string, bool>();
        inventorySlotData = new Dictionary<int, string>();

        // Quests
        quests = new Dictionary<string, (ObjectiveData[], bool[], bool, QuestReward)>();
        currentQuest = null;
        startedGameplay = false;

        // Cutscenes
        cutScenes = new Dictionary<string, bool>();

        // Doors
        doorsUnlocked = new Dictionary<string, bool>();
    }
}