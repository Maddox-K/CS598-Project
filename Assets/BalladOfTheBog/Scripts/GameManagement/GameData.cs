using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    // Player Data
    public Vector3 playerPosition;
    public int coinCount;

    // Enemies
    public Dictionary<string, bool> enemiesEncountered;
    public string lastEnemyEncountered;

    // Collectibles
    public Dictionary<string, bool> coinsCollected;
    
    public GameData()
    {
        playerPosition = new Vector3(0, 0, 0);
        this.coinCount = 0;

        enemiesEncountered = new Dictionary<string, bool>();
        lastEnemyEncountered = null;

        coinsCollected = new Dictionary<string, bool>();
    }
}