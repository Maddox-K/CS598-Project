using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    // General Game Data
    public bool autoSave;

    // Scene Management
    public string lastScene;

    // Player Data
    public float[] playerPos = new float[3];
    public float[] playerRot = new float[2];
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
        lastScene = null;

        // Player Data
        //playerPosition = new Vector3(0, 0, 0);
        for (int i = 0; i < 3; i++)
        {
            playerPos[i] = 0.0f;
        }
        //playerRotation = Vector2.zero;
        for (int i = 0; i < 2; i++)
        {
            playerRot[i] = 0.0f;
        }
        this.coinCount = 0;

        // Enemies and Encounters
        enemiesEncountered = new Dictionary<string, bool>();
        lastEnemyEncountered = null;

        // Collectibles
        coinsCollected = new Dictionary<string, bool>();
    }
}