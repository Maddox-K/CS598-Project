using System.Collections.Generic;

[System.Serializable]
public class GameData
{
    // General Game Data
    public bool autoSave;
    public long lastUpdated;

    // Scene Management
    public string lastScene;

    // Player Data
    public float[] playerPosition = new float[3];
    public float[] playerRotation = new float[2];
    public int coinCount;

    // Enemies and Encounters
    public Dictionary<string, bool> enemiesEncountered;
    public string lastEnemyEncountered; // id of last enemy encountered

    // Collectibles
    public Dictionary<string, bool> coinsCollected;
    
    public GameData()
    {
        // General Game Data
        autoSave = true;

        // Scene Management
        lastScene = "Scene1";

        // Player Data
        for (int i = 0; i < 3; i++)
        {
            playerPosition[i] = 0.0f;
        }
        for (int i = 0; i < 2; i++)
        {
            playerRotation[i] = 0.0f;
        }
        coinCount = 0;

        // Enemies and Encounters
        enemiesEncountered = new Dictionary<string, bool>();
        lastEnemyEncountered = null;

        // Collectibles
        coinsCollected = new Dictionary<string, bool>();
    }
}